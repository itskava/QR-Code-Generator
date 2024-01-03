#nullable disable

using System;
using System.Collections.Generic;
using System.Text;

namespace QR_Code_Generator
{
    namespace QRcodeAlgorithm
    {
        internal class CorrectionBytesCreation
        {
            private static readonly byte[] m_CorrectionBytesAmountPerBlock =
            {
            10, 16, 26, 18, 24, 16, 18, 22, 22, 26,
            30, 22, 22, 24, 24, 28, 28, 26, 26, 26,
            26, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28
        };

            private static byte correctionBytesAmountPerBlock;

            private static readonly SortedDictionary<byte, byte[]> generatingPolynomials = new()
        {
            {7, new byte[] { 87, 229, 146, 149, 238, 102, 21 } },

            {10, new byte[] { 251, 67, 46, 61, 118, 70, 64, 94, 32, 45 } },

            {13, new byte[] { 74, 152, 176, 100, 86, 100, 106, 104, 130, 218, 206, 140, 78 } },

            {15, new byte[] { 8, 183, 61, 91, 202, 37, 51, 58, 58, 237, 140, 124, 5, 99, 105 } },

            {16, new byte[] { 120, 104, 107, 109, 102, 161, 76, 3, 91, 191,
                147, 169, 182, 194, 225, 120 } },

            {17, new byte[] { 43, 139, 206, 78, 43, 239, 123, 206, 214, 147,
                24, 99, 150, 39, 243, 163, 136 } },

            {18, new byte[] { 215, 234, 158, 94, 184, 97, 118, 170, 79, 187,
                152, 148, 252, 179, 5, 98, 96, 153 } },

            {20, new byte[] { 17, 60, 79, 50, 61, 163, 26, 187, 202, 180, 221,
                225, 83, 239, 156, 164, 212, 212, 188, 190 } },

            {22, new byte[] { 210, 171, 247, 242, 93, 230, 14, 109, 221, 53,
                200, 74, 8, 172, 98, 80, 219, 134, 160, 105, 165, 231 } },

            {24, new byte[] { 229, 121, 135, 48, 211, 117, 251, 126, 159, 180,
                169, 152, 192, 226, 228, 218, 111, 0, 117, 232, 87, 96, 227, 21} },

            {26, new byte[] { 173, 125, 158, 2, 103, 182, 118, 17, 145, 201,
                111, 28, 165, 53, 161, 21, 245, 142, 13, 102, 48, 227, 153, 145, 218, 70 } },

            {28, new byte[] { 168, 223, 200, 104, 224, 234, 108, 180, 110, 190,
                195, 147, 205, 27, 232, 201, 21, 43, 245, 87, 42, 195, 212, 119, 242, 37, 9, 123 } },

            {30, new byte[] { 41, 173, 145, 152, 216, 31, 179, 182, 50, 48, 110,
                86, 239, 96, 222, 125, 42, 173, 226, 193, 224, 130, 156, 37, 251, 216, 238, 40, 192, 180 } }
        };

            private static byte[] generatingPolynomial;

            private static readonly byte[] galoisField =
            {
            1, 2, 4, 8, 16, 32, 64, 128, 29, 58, 116, 232, 205, 135, 19, 38,
            76, 152, 45, 90, 180, 117, 234, 201, 143, 3, 6, 12, 24, 48, 96, 192,
            157, 39, 78, 156, 37, 74, 148, 53, 106, 212, 181, 119, 238, 193, 159, 35,
            70, 140, 5, 10, 20, 40, 80, 160, 93, 186, 105, 210, 185, 111, 222, 161,
            95, 190, 97, 194, 153, 47, 94, 188, 101, 202, 137, 15, 30, 60, 120, 240,
            253, 231, 211, 187, 107, 214, 177, 127, 254, 225, 223, 163, 91, 182, 113, 226,
            217, 175, 67, 134, 17, 34, 68, 136, 13, 26, 52, 104, 208, 189, 103, 206,
            129, 31, 62, 124, 248, 237, 199, 147, 59, 118, 236, 197, 151, 51, 102, 204,
            133, 23, 46, 92, 184, 109, 218, 169, 79, 158, 33, 66, 132, 21, 42, 84,
            168, 77, 154, 41, 82, 164, 85, 170, 73, 146, 57, 114, 228, 213, 183, 115,
            230, 209, 191, 99, 198, 145, 63, 126, 252, 229, 215, 179, 123, 246, 241, 255,
            227, 219, 171, 75, 150, 49, 98, 196, 149, 55, 110, 220, 165, 87, 174, 65,
            130, 25, 50, 100, 200, 141, 7, 14, 28, 56, 112, 224, 221, 167, 83, 166,
            81, 162, 89, 178, 121, 242, 249, 239, 195, 155, 43, 86, 172, 69, 138, 9,
            18, 36, 72, 144, 61, 122, 244, 245, 247, 243, 251, 235, 203, 139, 11, 22,
            44, 88, 176, 125, 250, 233, 207, 131, 27, 54, 108, 216, 173, 71, 142, 1
        };

            private static readonly byte?[] reverseGaloisField =
            {
            null, 0, 1, 25, 2, 50, 26, 198, 3, 223, 51, 238, 27, 104, 199, 75,
            4, 100, 224, 14, 52, 141, 239, 129, 28, 193, 105, 248, 200, 8, 76, 113,
            5, 138, 101, 47, 225, 36, 15, 33, 53, 147, 142, 218, 240, 18, 130, 69,
            29, 181, 194, 125, 106, 39, 249, 185, 201, 154, 9, 120, 77, 228, 114, 166,
            6, 191, 139, 98, 102, 221, 48, 253, 226, 152, 37, 179, 16, 145, 34, 136,
            54, 208, 148, 206, 143, 150, 219, 189, 241, 210, 19, 92, 131, 56, 70, 64,
            30, 66, 182, 163, 195, 72, 126, 110, 107, 58, 40, 84, 250, 133, 186, 61,
            202, 94, 155, 159, 10, 21, 121, 43, 78, 212, 229, 172, 115, 243, 167, 87,
            7, 112, 192, 247, 140, 128, 99, 13, 103, 74, 222, 237, 49, 197, 254, 24,
            227, 165, 153, 119, 38, 184, 180, 124, 17, 68, 146, 217, 35, 32, 137, 46,
            55, 63, 209, 91, 149, 188, 207, 205, 144, 135, 151, 178, 220, 252, 190, 97,
            242, 86, 211, 171, 20, 42, 93, 158, 132, 60, 57, 83, 71, 109, 65, 162,
            31, 45, 67, 216, 183, 123, 164, 118, 196, 23, 73, 236, 127, 12, 111, 246,
            108, 161, 59, 82, 41, 157, 85, 170, 251, 96, 134, 177, 187, 204, 62, 90,
            203, 89, 95, 176, 156, 169, 160, 81, 11, 245, 22, 235, 122, 117, 44, 215,
            79, 174, 213, 233, 230, 231, 173, 232, 116, 214, 244, 234, 168, 80, 88, 175
        };

            public static byte[][] CorrectionBlocks { get; private set; }

            private static short correctionBytesIndex = 0;

            private static void GetGeneratingPolynomial()
            {
                correctionBytesAmountPerBlock = m_CorrectionBytesAmountPerBlock[ServiceInformation.Version - 1];
                generatingPolynomial = new byte[correctionBytesAmountPerBlock];
                byte[] polynomial = generatingPolynomials[correctionBytesAmountPerBlock];
                Array.Copy(polynomial, generatingPolynomial, correctionBytesAmountPerBlock);
            }

            public static void CreateCorrectionBytes()
            {
                GetGeneratingPolynomial();
                CorrectionBlocks = new byte[SeparationIntoBlocks.Blocks.Length][];

                foreach (string block in SeparationIntoBlocks.Blocks)
                {
                    short blockSizeInBytes = (short)(block.Length / 8);
                    short length = short.Max(blockSizeInBytes, correctionBytesAmountPerBlock);

                    List<byte> correctionBytes = new List<byte>(length);
                    FillCorrectionBlock(correctionBytes, block);

                    for (int i = 0; i < blockSizeInBytes; ++i)
                    {
                        byte a = correctionBytes.ElementAt(0);
                        correctionBytes.RemoveAt(0);
                        correctionBytes.Add(0);

                        if (a == 0) continue;

                        byte[] polynomial = new byte[correctionBytesAmountPerBlock]; // Copying is expensive, remake
                        Array.Copy(generatingPolynomial, polynomial, generatingPolynomial.Length);

                        byte b = (byte)reverseGaloisField[a];

                        for (int j = 0; j < correctionBytesAmountPerBlock; ++j)
                        {
                            polynomial[j] = (byte)((polynomial[j] + b) % 255);
                            polynomial[j] = (byte)(galoisField[polynomial[j]]);
                            correctionBytes[j] = (byte)(correctionBytes[j] ^ polynomial[j]);
                        }
                    }

                    AddCorrectionBlock(correctionBytes);
                }
            }

            private static void FillCorrectionBlock(List<byte> correctionBlock, string block)
            {
                int index = 0;
                for (int i = 0; i < block.Length; i += 8)
                {
                    correctionBlock.Add(Convert.ToByte(block[i..(i + 8)], 2));
                    index++;
                }

                while (index++ < correctionBlock.Capacity)
                {
                    correctionBlock.Add(0);
                }
            }

            private static void AddCorrectionBlock(List<byte> block)
            {
                byte[] correctionBytes = new byte[correctionBytesAmountPerBlock];

                for (int i = 0; i < correctionBytesAmountPerBlock; ++i)
                {
                    correctionBytes[i] = block[i];
                }

                CorrectionBlocks[correctionBytesIndex++] = correctionBytes;
            }
        }
    }
}
