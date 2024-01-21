using System.Text;

namespace QR_Code_Generator.Model
{
    /// <summary>
    /// This class is responsible for combining data blocks with correction blocks
    /// </summary>
    internal static class CombiningBlocks
    {
        /// <summary>
        /// This method is used to combine data blocks and correction blocks into a single bit sequence
        /// </summary>
        public static void Combine()
        {
            StringBuilder finalSequence = new();

            int maxDataBlockLength = SeparationIntoBlocks.Blocks[^1].Length;
            int blocksAmount = SeparationIntoBlocks.Blocks.Length;

            /* Combining all data blocks by taking the first byte from each block, then the second 
             * byte, and so on until no bytes remain. If there are no bytes in the current block, it is skipped. */
            for (int i = 0; i < maxDataBlockLength; i += 8)
            {
                for (int j = 0; j < blocksAmount; ++j)
                {
                    string currentBlock = SeparationIntoBlocks.Blocks[j];

                    if (currentBlock.Length > i) // If there are bytes in the current block
                    {
                        finalSequence.Append(currentBlock[i..(i + 8)]);
                    }
                }
            }

            int maxCorrectionBlockLength = CorrectionBytesCreation.CorrectionBlocks[^1].Length;
            int correctionBlocksAmount = CorrectionBytesCreation.CorrectionBlocks.Length;

            // Correction blocks are combined in the same way as data blocks
            for (int i = 0; i < maxCorrectionBlockLength; i += 8)
            {
                for (int j = 0; j < correctionBlocksAmount; ++j)
                {
                    string currentBlock = CorrectionBytesCreation.CorrectionBlocks[j];

                    if (currentBlock.Length > i) // If there are bytes in the current block
                    {
                        finalSequence.Append(currentBlock[i..(i + 8)]);
                    }
                }
            }

            Configuration.BitSequence = finalSequence.ToString();
        }
    }
}
