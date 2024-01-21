#nullable disable

namespace QR_Code_Generator.Model
{
    /// <summary>
    /// This class is resposible for dividing a given bit sequence into blocks.
    /// </summary>
    internal static class SeparationIntoBlocks
    {
        /* The next 4 arrays represent the number of blocks into which the bit sequence will be split depending
           on the version and correction level of the QR code. This array corresponds to the L correction level */
        private static readonly byte[] s_lBlocksQuantities =
        {
            1, 1, 1, 1, 1, 2, 2, 2, 2, 4,
            4, 4, 4, 4, 6, 6, 6, 6, 7, 8,
            8, 9, 9, 10, 12, 12, 12, 13, 14, 15,
            16, 17, 18, 19, 19, 20, 21, 22, 24, 25
        };

        // This array corresponds to the M correction level        
        private static readonly byte[] s_mBlocksQuantities =
        {
            1, 1, 1, 2, 2, 4, 4, 4, 5, 5,
            5, 8, 9, 9, 10, 10, 11, 13, 14, 16,
            17, 17, 18, 20, 21, 23, 25, 26, 28, 29,
            31, 33, 35, 37, 38, 40, 43, 45, 47, 49
        };

        // This array corresponds to the Q correction level
        private static readonly byte[] s_qBlocksQuantities =
        {
            1, 1, 2, 2, 4, 4, 6, 6, 8, 8,
            8, 10, 12, 16, 12, 17, 16, 18, 21, 20,
            23, 23, 25, 27, 29, 34, 34, 35, 38, 40,
            43, 45, 48, 51, 53, 56, 59, 62, 65, 68
        };

        // This array corresponds to the H correction level
        private static readonly byte[] s_hBlocksQuantities =
        {
            1, 1, 2, 4, 4, 4, 5, 6, 8, 8,
            11, 11, 16, 16, 18, 16, 19, 21, 25, 25,
            25, 34, 30, 32, 35, 37, 40, 42, 45, 48,
            51, 54, 57, 60, 63, 66, 70, 74, 77, 81
        };

        private static short s_blockSizeInBytes; // The size of the one block in bytes

        private static short s_blocksQuantity; // The amount of block into which the bit sequence will be split

        private static short s_remainder; /* The remainder shows the number of blocks that will have increased size.
                                          * For example, if we have the sequence of size 193 bytes (1544 bits,
                                          * with the correction level M - this is 9 version), the number of blocks is
                                          * 5, then the block size is 38, the _remainder is 3. This means that blocks will
                                          * have the following sizes: 38, 38, 39, 39, 39. If remainder were 0, 
                                          * then all the blocks would have a size equal to 38 bytes */

        public static string[] Blocks { get; private set; } // A property that represents this blocks

        /// <summary>
        /// This method is used to get the data from a given sequence that will be used
        /// to separate it into the blocks
        /// </summary>
        private static void GetSequenceData()
        {
            short byteQuantity = (short)(Configuration.BitSequence.Length / 8);

            switch (Configuration.CorrectionLevel)
            {
                case CorrectionLevel.L: { s_blocksQuantity = s_lBlocksQuantities[Configuration.Version - 1]; break; }
                case CorrectionLevel.M: { s_blocksQuantity = s_mBlocksQuantities[Configuration.Version - 1]; break; }
                case CorrectionLevel.Q: { s_blocksQuantity = s_qBlocksQuantities[Configuration.Version - 1]; break; }
                case CorrectionLevel.H: { s_blocksQuantity = s_hBlocksQuantities[Configuration.Version - 1]; break; }
            }
            
            s_blockSizeInBytes = (short)(byteQuantity / s_blocksQuantity);
            s_remainder = (short)(byteQuantity % s_blocksQuantity);
            Blocks = new string[s_blocksQuantity];
        }

        /// <summary>
        /// This method is used to separate the bit sequence into blocks
        /// </summary>
        public static void Separate()
        {
            GetSequenceData();

            for (int i = 0; i < s_blocksQuantity - s_remainder; ++i)
            {
                int leftIndex = i * s_blockSizeInBytes * 8;
                int rightIndex = (i + 1) * s_blockSizeInBytes * 8;
                Blocks[i] = Configuration.BitSequence[leftIndex..rightIndex];
            }

            s_blockSizeInBytes++;
            for (int i = 1; i <= s_remainder; ++i)
            {
                int rightIndex = Configuration.BitSequence.Length - (i - 1) * s_blockSizeInBytes * 8;
                int leftIndex = Configuration.BitSequence.Length - i * s_blockSizeInBytes * 8;
                Blocks[^i] = Configuration.BitSequence[leftIndex..rightIndex];
            }
        }
    }
}

