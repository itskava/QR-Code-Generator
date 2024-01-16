#nullable disable

using System.Drawing;
using System.Drawing.Imaging;

namespace QR_Code_Generator
{
    /// <summary>
    /// This class is responsible for the configuration of the QR-code.
    /// </summary>
    internal static class Configuration
    {
        // This field represents the selected encoding method. The binary method is default
        public static EncodingMethod EncodingMethod { get; set; } = EncodingMethod.Binary;

        // This field represents the selected correction level. The default level is M
        public static CorrectionLevel CorrectionLevel { get; set; } = CorrectionLevel.M;

        // This field represents the version of the QR-code
        public static int Version { get; set; }

        // This field represents the bit sequence that will be encoded into the QR-code
        public static string BitSequence { get; set; }

        public static Color BackColor { get; set; } = Color.White;

        public static Color ForeColor { get; set; } = Color.Black;

        public static bool IsHighestResolution { get; set; } = false;

        public static bool IsOptimizedMask { get; set; } = false;

        public static ImageFormat SaveFormat { get; set; } = ImageFormat.Png;

        public static string SavePath { get; set; } = "C:/Users/timof/CSharp/QR-Code-Generator/Images/"; // create a folder

        public static string FileName { get; set; } = "test2.png";
    }
}
