#nullable disable

using System;
using System.Text;

namespace QR_Code_Generator
{
    /// <summary>
    /// This class is responsible for adding a service information into a original bit sequence.
    /// </summary>
    internal static class ServiceInformation
    {
        /* The next 4 arrays represent maximum amount of bits that can be encoded in QR-code of each version
           for different correction levels. This data corresponds to the L correction level. */
        private static readonly short[] _lMaxAmountOfInformation =
        {
            152, 272, 440, 640, 864, 1088, 1248, 1552, 1856, 2192,
            2592, 2960, 3424, 3688, 4184, 4712, 5176, 5768, 6360, 6888,
            7456, 8048, 8752, 9392, 10208, 10960, 11744, 12248, 13048, 13880,
            14744, 15640, 16568, 17528, 18448, 19472, 20528, 21616, 22496, 23648
        };

        // This data corresponds to the M correction level.
        private static readonly short[] _mMaxAmountOfInformation =
        {
            128, 224, 352, 512, 688, 864, 992, 1232, 1456, 1728,
            2032, 2320, 2672, 2920, 3320, 3624, 4056, 4504, 5016, 5352,
            5712, 6256, 6880, 7312, 8000, 8496, 9024, 9544, 10136, 10984,
            11640, 12328, 13048, 13800, 14496, 15312, 15936, 16816, 17728, 18672
        };

        // This data corresponds to the Q correction level.
        private static readonly short[] _qMaxAmountOfInformation = 
        {
            104, 176, 272, 384, 496, 608, 704, 880, 1056, 1232,
            1440, 1648, 1952, 2088, 2360, 2600, 2936, 3176, 3560, 3880,
            4096, 4544, 4912, 5312, 5744, 6032, 6464, 6968, 7288, 7880,
            8264, 8920, 9368, 9848, 10288, 10832, 11408, 12016, 12656, 13328
        };

        // This data corresponds to the H correction level.
        private static readonly short[] _hMaxAmountOfInformation =
        {
            72, 128, 208, 288, 368, 480, 528, 688, 800, 976,
            1120, 1264, 1440, 1576, 1784, 2024, 2264, 2504, 2728, 3080,
            3248, 3536, 3712, 4112, 4304, 4768, 5024, 5288, 5608, 5960,
            6344, 6760, 7208, 7688, 7888, 8432, 8768, 9136, 9776, 10208
        };

        private static short[] _maxAmountOfInformation; // This array will reference one of 4 above depending on the correction level

        private static string _encodingMethod; // This string corresponds to the encoding method

        private static readonly string[] _fillingBlocks = { "11101100", "00010001" }; /* This blocks are used to fill the sequence so its length is equal to the 
                                                                                        maximum amount of informarion that can be stored in the QR code of 
                                                                                        the corresponding version and correction level */

        /// <summary>
        /// This method is used to add service information to the original bit sequence
        /// </summary>
        /// <param name="bitSequence">The original bit sequence</param>
        public static void AddServiceInformation()
        {
            GetServiceData();
            string dataQuantity = GetDataQuantity(Configuration.BitSequence);
            Configuration.BitSequence = _encodingMethod + dataQuantity + Configuration.BitSequence;
            Configuration.Version = GetVersion(Configuration.BitSequence.Length);
            FillSequence();
        }

        private static void GetServiceData()
        {
            switch (Configuration.EncodingMethod)
            {
                case EncodingMethod.Numeric:
                {
                    _encodingMethod = "0001";
                    break;
                }
                case EncodingMethod.Alphanumeric:
                {
                    _encodingMethod = "0010";
                    break;
                }
                case EncodingMethod.Binary:
                {
                    _encodingMethod = "0100";
                    break;
                }
                default:
                {
                    throw new NotSupportedException();
                }
            }

            switch (Configuration.CorrectionLevel)
            {
                case CorrectionLevel.L:
                {
                    _maxAmountOfInformation = _lMaxAmountOfInformation;
                    break;
                }
                case CorrectionLevel.M:
                {
                    _maxAmountOfInformation = _mMaxAmountOfInformation;
                    break;
                }
                case CorrectionLevel.Q:
                {
                    _maxAmountOfInformation = _qMaxAmountOfInformation;
                    break;
                }
                case CorrectionLevel.H:
                {
                    _maxAmountOfInformation = _hMaxAmountOfInformation;
                    break;
                }
                default:
                { 
                    throw new NotSupportedException(); 
                }
            }
        }

        /// This method is used to obtain the version of QR code based on the given bit sequence
        private static int GetVersion(int bitSequenceLength) 
        {
            int version = -1;

            for (int i = 0; i < _maxAmountOfInformation.Length; ++i)
            {
                if (bitSequenceLength <= _maxAmountOfInformation[i]) // If it is possible to store data in this number of bits
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

        // This method is used to get the data quantity which will be inserted into the bit sequence
        private static string GetDataQuantity(string bitSequence)
        {
            int version = GetVersion(bitSequence.Length);
            int dataQuantityLength = GetDataQuantityLength(version);
            int bitSequenceLength = _encodingMethod.Length + dataQuantityLength + bitSequence.Length;

            if (bitSequenceLength > _maxAmountOfInformation[version - 1]) /* If the sequence's length after adding service info exceeds maximum 
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

            string dataQuantity;

            switch (Configuration.EncodingMethod)
            {
                case EncodingMethod.Numeric:
                {
                    int lastDigitsAmount = (bitSequence.Length % 10 == 7) ? 2 : 1;
                    int totalDigitAmount = bitSequence.Length / 10 * 3 + lastDigitsAmount;
                    dataQuantity = Convert.ToString(totalDigitAmount, 2).PadLeft(dataQuantityLength, '0');
                    break;
                }
                case EncodingMethod.Alphanumeric:
                {
                    int lastCharacter = (bitSequence.Length % 11 == 0) ? 0 : 1;
                    int totalCharactersAmount = bitSequence.Length / 11 * 2 + lastCharacter;
                    dataQuantity = Convert.ToString(totalCharactersAmount, 2).PadLeft(dataQuantityLength, '0');
                    break;
                }
                case EncodingMethod.Binary:
                {
                    dataQuantity = Convert.ToString(bitSequence.Length / 8, 2).PadLeft(dataQuantityLength, '0');
                    break;
                }
                default:
                {
                    throw new NotSupportedException();
                }
            }

            return dataQuantity;
        }

        // This method is used to get the amount of bits for data quantity
        private static int GetDataQuantityLength(int version) 
        {
            int dataQuantityLength = 0;

            switch (Configuration.EncodingMethod)
            {
                case EncodingMethod.Numeric:
                {
                    if (version < 10) dataQuantityLength = 10;
                    else if (version < 27) dataQuantityLength = 12;
                    else dataQuantityLength = 14;
                    break;
                }
                case EncodingMethod.Alphanumeric:
                {
                    if (version < 10) dataQuantityLength = 9;
                    else if (version < 27) dataQuantityLength = 11;
                    else dataQuantityLength = 13; 
                    break;
                }
                case EncodingMethod.Binary:
                {
                    if (version < 10) dataQuantityLength = 8;
                    else dataQuantityLength = 16;
                    break;
                }
            }

            return dataQuantityLength;
        }

        /* This method is used to fill the sequence so its length is equal to the
           maximum amount of informarion that can be stored in the QR code of
           the corresponding version */
        private static void FillSequence() 
        {
            StringBuilder sequenceBuidler = new StringBuilder(Configuration.BitSequence,
                _maxAmountOfInformation[Configuration.Version - 1]);

            while (sequenceBuidler.Length % 8 != 0)
            {
                sequenceBuidler.Append('0');
            }

            int fillingBlockIndex = 0;

            while (sequenceBuidler.Length != sequenceBuidler.Capacity) // Adding filling blocks
            {
                sequenceBuidler.Append(_fillingBlocks[fillingBlockIndex]);
                fillingBlockIndex = 1 - fillingBlockIndex; // Switching the filling block
            }

            Configuration.BitSequence = sequenceBuidler.ToString();
        }
    }
}
