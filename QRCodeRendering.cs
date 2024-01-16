using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows;


namespace QR_Code_Generator
{
    internal static class QRCodeRendering
    {
        private static int _size;

        private static int _imageDimension;

        private static int _upscaleCoef;

        private static int _indentSize;

        private static Bitmap _image;

        public static void CreateQR()
        {
            GetImageData();

            int upscaledSize = _size * _upscaleCoef;
            _image = new Bitmap(_imageDimension, _imageDimension);

            byte[][] withoutIndent = new byte[upscaledSize][];

            for (int i = 0; i < upscaledSize; ++i)
            {
                withoutIndent[i] = new byte[upscaledSize];
            }

            for (int i = 0; i < _size; ++i)
            {
                for (int j = 0; j < _size; ++j)
                {
                    int upscaledRow = i * _upscaleCoef;
                    int upscaledColumn = j * _upscaleCoef;
                    
                    withoutIndent[upscaledRow][upscaledColumn] = InformationPlacement.QRCode[i][j];

                    if (withoutIndent[upscaledRow][upscaledColumn] == 1)
                    {
                        FillModule(withoutIndent, upscaledRow, upscaledColumn);
                    }
                }
            }

            byte[][] withIndent = new byte[_imageDimension][];

            for (int i = 0; i < _imageDimension; ++i)
            {
                withIndent[i] = new byte[_imageDimension];
            }

            int startRow = _indentSize / 2 - 1;
            int startColumn = _indentSize / 2 - 1;

            for (int i = startRow; i < startRow + upscaledSize; ++i)
            {
                for (int j = startColumn; j < startColumn + upscaledSize; ++j)
                {
                    withIndent[i][j] = withoutIndent[i - startRow][j - startColumn];
                }
            }

            for (int i = 0; i < _imageDimension; ++i)
            {
                for (int j = 0; j < _imageDimension; ++j)
                {
                    if (withIndent[i][j] == 1) _image.SetPixel(j, i, Configuration.ForeColor);
                    else _image.SetPixel(j, i, Configuration.BackColor);
                }
            }

            //_image.Save(Configuration.SavePath + Configuration.FileName, Configuration.SaveFormat);
        }

        private static void GetImageData()
        {
            _size = InformationPlacement.QRSize + 8; // Considering the indent
            _imageDimension = GetDimension();
            _upscaleCoef = GetUpscaleCoefficient();
            _indentSize = _imageDimension - _size * _upscaleCoef;
        }

        private static int GetDimension()
        {
            if (Configuration.IsHighestResolution) return 2048;

            int upscaledSize = _size * 10;
            int dimension = 256;

            while (upscaledSize > dimension)
            {
                dimension += 256;
            }

            return dimension;
        }

        private static int GetUpscaleCoefficient()
        {
            int upscaledCoefficient = 0;

            while (_size * (upscaledCoefficient + 1) < _imageDimension)
            {
                upscaledCoefficient++;
            }

            return upscaledCoefficient;
        }

        private static void FillModule(byte[][] array, int row, int column)
        {
            for (int i = row; i < row + _upscaleCoef; ++i)
            {
                for (int j = column; j < column + _upscaleCoef; ++j)
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
