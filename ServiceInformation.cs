using System;
using System.Text;

namespace QR_Code_Generator
{
    /// <summary>
    /// This class is responsible for adding a service information into a original bit sequence.
    /// </summary>
    internal static class ServiceInformation
    {
        /* This array represents maximum amount of bytes that can be encoded in QR-code of each version.
           This data corresponds to the M correction level. */
        private static readonly short[] maxAmountOfInformation =
        {
            128, 224, 352, 512, 688, 864, 992, 1232, 1456, 1728,
            2032, 2320, 2672, 2920, 3320, 3624, 4056, 4504, 5016, 5352,
            5712, 6256, 6880, 7312, 8000, 8496, 9024, 9544, 10136, 10984,
            11640, 12328, 13048, 13800, 14496, 15312, 15936, 16816, 17728, 18672
        };
        private static readonly string encodingMethod = "0100"; // This 4 bytes correspond to byte encoding
        private static readonly string[] fillingBlocks = { "11101100", "00010001" }; /* This blocks are used to fill the sequence so its length is equal to the 
                                                                                        maximum amount of informarion that can be stored in the QR code of 
                                                                                        the corresponding version */
        public static int Version { get; private set; } // This property represents the version of the QR code
        public static string BitSequence { get; private set; } // This property represents the bit sequence with added service information
        public static void InitSequence(string bitSequence) // This method is used to add service information to the original bit sequence
        {
            string dataQuantity = GetDataQuantity(bitSequence);
            BitSequence = encodingMethod + dataQuantity + bitSequence;
            Version = GetVersion(BitSequence);
            FillSequence();
        }
        private static int GetVersion(string bitSequence) // This method is used to obtain the version of QR code based on the given bit sequence
        {
            int version = -1;

            for (int i = 0; i < maxAmountOfInformation.Length; ++i)
            {
                if (bitSequence.Length <= maxAmountOfInformation[i]) // If it is possible to store data in this number of bytes
                {
                    version = i + 1;
                    break;
                }
            }

            if (version == -1) // In case if the sequence is too long
            {
                throw new ArgumentException("Entered text is too long, cannot be encoded.");
            }

            return version;
        }
        private static string GetDataQuantity(string bitSequence)
        {
            int version = GetVersion(bitSequence);
            int dataQuantityLength = GetDataQuantityLength(version);
            int bitSequenceLength = bitSequence.Length + dataQuantityLength + encodingMethod.Length;

            if (bitSequence.Length > maxAmountOfInformation[version - 1]) /* If the sequence's length after adding service info exceeds maximum 
                                                                             amount of the selected version, version must be increased */
            {
                if (version == 40) // In case if the sequence is too long
                {
                    throw new ArgumentException("Entered text is too long, cannot be encoded.");
                }
                else
                {
                    version++;
                    dataQuantityLength = GetDataQuantityLength(version);
                }
            }

            string dataQuantity = Convert.ToString(bitSequence.Length, 2);

            while (dataQuantity.Length < dataQuantityLength)
            {
                dataQuantity = "0" + dataQuantity;
            }

            return dataQuantity;
        }

        private static int GetDataQuantityLength(int version) => (version < 10) ? 8 : 16; // This method is used to get the amount of bytes for data quantity

        private static void FillSequence() /* This method is used to fill the sequence so its length is equal to the
                                              maximum amount of informarion that can be stored in the QR code of
                                              the corresponding version */
        {
            StringBuilder sequenceBuidler = new StringBuilder(BitSequence, maxAmountOfInformation[Version - 1]);

            while (sequenceBuidler.Length % 8 != 0)
            {
                sequenceBuidler.Append("0");
            }

            int fillingBlockIndex = 0;

            while (sequenceBuidler.Length != sequenceBuidler.Capacity) // Adding filling blocks
            {
                sequenceBuidler.Append(fillingBlocks[fillingBlockIndex]);
                fillingBlockIndex = 1 - fillingBlockIndex; // Switching the filling block
            }

            BitSequence = sequenceBuidler.ToString();
        }
    }
}
