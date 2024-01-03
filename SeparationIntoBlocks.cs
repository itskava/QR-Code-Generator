#nullable disable

namespace QR_Code_Generator
{
    /// <summary>
    /// This class is resposible for dividing a given bit sequence into blocks.
    /// </summary>
    internal class SeparationIntoBlocks
    {
        /* This array represents the number of blocks into which the bit sequence will be split depending
           on the version and correction level of the QR code */
        private static readonly byte[] blocksQuantities =
        {
            1, 1, 1, 2, 2, 4, 4, 4, 5, 5,
            5, 8, 9, 9, 10, 10, 11, 13, 14, 16,
            17, 17, 18, 20, 21, 23, 25, 26, 28, 29,
            31, 33, 35, 37, 38, 40, 43, 45, 47, 49
        };

        private static short blockSizeInBytes;

        private static short blocksQuantity; // The amount of block into which the bit sequence will be split

        private static short remainder; /* The remainder shows the number of blocks that will have increased size.
                                                  * For example, if we have the sequence of size 193 bytes (1544 bits,
                                                  * with the correction level M - this is 9 version), the number of blocks is
                                                  * 5, then the block size is 38, the remainder is 3. This means that blocks will
                                                  * have the following sizes: 38, 38, 39, 39, 39. If remainder were 0, 
                                                  * then all the blocks would have a size equal to 38 bytes */

        public static string[] Blocks { get; private set; } // A property that represents this blocks

        /* This method is used to get the data from a given sequence that will be used
         * to separate it into the blocks */
        private static void GetSequenceData()
        {
            short byteQuantity = (short)(ServiceInformation.BitSequence.Length / 8);

            blocksQuantity = blocksQuantities[ServiceInformation.Version - 1];
            blockSizeInBytes = (short)(byteQuantity / blocksQuantity);
            remainder = (short)(byteQuantity % blocksQuantity);
            Blocks = new string[blocksQuantity];
        }

        /* This method is used to separate the sequence into a blocks */
        public static void Separate()
        {
            GetSequenceData();
            for (int i = 0; i < blocksQuantity - remainder; ++i)
            {
                int leftIndex = i * blockSizeInBytes * 8;
                int rightIndex = (i + 1) * blockSizeInBytes * 8;
                Blocks[i] = ServiceInformation.BitSequence[leftIndex..rightIndex];
            }

            blockSizeInBytes++;
            for (int i = 1; i <= remainder; ++i)
            {
                int length = ServiceInformation.BitSequence.Length;
                int rightIndex = length - (i - 1) * blockSizeInBytes * 8;
                int leftIndex = length - i * blockSizeInBytes * 8;
                Blocks[^i] = ServiceInformation.BitSequence[leftIndex..rightIndex];
            }
        }
    }
}

