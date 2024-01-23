 #nullable disable

using System.Drawing;

namespace QR_Code_Generator.Model
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

        // This field represents the background color of the QR-code
        public static Color Background { get; set; } = Color.White;

        // This field represents the foreground color of the QR-code
        public static Color Foreground { get; set; } = Color.Black;

        /* This fields represenets the resolution of the QR-code.
           If it's true, QR-code will have the highest possible resolution (2048x2048), 
           else - resolution will be set automatically */
        public static bool IsHighestResolution { get; set; } = false;

        /* This field represents the optimization of the QR-code.
           If it's true, then 8 masks will be applied to find out the QR-code
           with the smallest amount of the artifacts, else - a random mask will
           be applied. */
        public static bool IsOptimized { get; set; } = false;
    }
}
