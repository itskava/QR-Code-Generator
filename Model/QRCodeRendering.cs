using System.Drawing;

namespace QR_Code_Generator.Model
{
    /// <summary>
    /// This class is responsible for creating a bitmap image that is ready to be displayed or saved.
    /// </summary>
    internal static class QRCodeRendering
    {
        // The size of a QR-code without the upscaling
        private static int s_size;

        // The size of one image dimension
        private static int s_imageDimension;

        // The coefficient showing how many times the modules will be increased
        private static int s_upscaleCoefficient;

        // The size of an indent around the QR-code
        private static int s_indentSize;

        // This property is a bitmap image of a QR-code
        public static Bitmap BitmapImage { get; private set; }

        /// <summary>
        /// This method is used to create a bitmap that represents the QR-code
        /// </summary>
        public static void CreateQR()
        {
            GetImageData();

            int upscaledSize = s_size * s_upscaleCoefficient;
            BitmapImage = new Bitmap(s_imageDimension, s_imageDimension);

            byte[][] withoutIndent = new byte[upscaledSize][];

            for (int i = 0; i < upscaledSize; ++i)
            {
                withoutIndent[i] = new byte[upscaledSize];
            }

            for (int i = 0; i < s_size; ++i)
            {
                for (int j = 0; j < s_size; ++j)
                {
                    int upscaledRow = i * s_upscaleCoefficient;
                    int upscaledColumn = j * s_upscaleCoefficient;
                    
                    withoutIndent[upscaledRow][upscaledColumn] = InformationPlacement.QRCode[i][j];

                    if (withoutIndent[upscaledRow][upscaledColumn] == 1)
                    {
                        FillModule(withoutIndent, upscaledRow, upscaledColumn);
                    }
                }
            }

            byte[][] withIndent = new byte[s_imageDimension][];

            for (int i = 0; i < s_imageDimension; ++i)
            {
                withIndent[i] = new byte[s_imageDimension];
            }

            int startRow = s_indentSize / 2 - 1;
            int startColumn = s_indentSize / 2 - 1;

            for (int i = startRow; i < startRow + upscaledSize; ++i)
            {
                for (int j = startColumn; j < startColumn + upscaledSize; ++j)
                {
                    withIndent[i][j] = withoutIndent[i - startRow][j - startColumn];
                }
            }

            for (int i = 0; i < s_imageDimension; ++i)
            {
                for (int j = 0; j < s_imageDimension; ++j)
                {
                    if (withIndent[i][j] == 1) BitmapImage.SetPixel(j, i, Configuration.Foreground);
                    else BitmapImage.SetPixel(j, i, Configuration.Background);
                }
            }
        }

        /// <summary>
        /// This method is used to get the service data for a bitmap image
        /// </summary>
        private static void GetImageData()
        {
            s_size = InformationPlacement.QRSize + 8; // Considering the indent
            s_imageDimension = GetDimension();
            s_upscaleCoefficient = GetUpscaleCoefficient();
            s_indentSize = s_imageDimension - s_size * s_upscaleCoefficient;
        }

        /// <summary>
        /// This method is used to get the resolution of the future image
        /// </summary>
        private static int GetDimension()
        {
            if (Configuration.IsHighestResolution) return 2048;

            int upscaledSize = s_size * 10;
            int dimension = 256;

            while (upscaledSize > dimension)
            {
                dimension += 256;
            }

            return dimension;
        }

        /// <summary>
        /// This method is used to get the upscale coefficient
        /// </summary>
        private static int GetUpscaleCoefficient()
        {
            int upscaledCoefficient = 0;

            while (s_size * (upscaledCoefficient + 1) < s_imageDimension)
            {
                upscaledCoefficient++;
            }

            return upscaledCoefficient;
        }

        /// <summary>
        /// This method is used to fill the module in a bitmap image based on the upscale coefficient
        /// </summary>
        private static void FillModule(byte[][] array, int row, int column)
        {
            for (int i = row; i < row + s_upscaleCoefficient; ++i)
            {
                for (int j = column; j < column + s_upscaleCoefficient; ++j)
                {
                    if (array[i][j] == 0)
                    {
                        array[i][j] = 1;
                    }
                }
            }
        }
    }
}
