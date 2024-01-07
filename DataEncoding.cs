using System;
using System.Collections.Generic;
using System.Text;

namespace QR_Code_Generator
{
    /// <summary>
    /// A class that encodes given data into a sequence of bits.
    /// </summary>
    internal static class DataEncoding
    {
        /* A dictionary that contains all possible characters for alphanumeric encoding and 
           values that correspond to them.
           It's definetely woule be more effiecient to replace this values with their binary representation to
           avoid having to convert them every time, but i'm lazy. Perhaps in the future. */
        private static readonly SortedDictionary<char, byte> _alphaNumericTable = 
            new SortedDictionary<char, byte>()
        {
            { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 }, { '5', 5 }, { '6', 6 },
            { '7', 7 }, { '8', 8 }, { '9', 9 }, { 'A', 10 }, { 'B', 11 }, { 'C', 12 }, { 'D', 13 },
            { 'E', 14 }, { 'F', 15 }, { 'G', 16 }, { 'H', 17 }, { 'I', 18 }, { 'J', 19 },
            { 'K', 20 }, { 'L', 21 }, { 'M', 22 }, { 'N', 23 }, { 'O', 24 }, { 'P', 25 },
            { 'Q', 26 }, { 'R', 27 }, { 'S', 28 }, { 'T', 29 }, { 'U', 30 }, { 'V', 31 },
            { 'W', 32 }, { 'X', 33 }, { 'Y', 34 }, { 'Z', 35 }, { ' ', 36 }, { '$', 37 },
            { '%', 38 }, { '*', 39 }, { '+', 40 }, { '-', 41 }, { '.', 42 }, { '/', 43 },
            { ':', 44 }
        };  

        /* This method is used to encode data into a sequence of bits in case of using
           digital encoding */
        private static string EncodeUsingDigitalEncoding(string data)
        {
            StringBuilder sequenceBuilder = new StringBuilder();

            /* Iterating the data to the end excluding last 1 or 2 characters
               (if the data length isn't divisible by 3) */
            for (int i = 0; i < data.Length - data.Length % 3; i += 3) 
            {
                // Converting three current digits into a binary number
                short value = Convert.ToInt16(data[i..(i + 3)]);
                // A binary number must contain 10 bits so it's filled with zeros
                string binaryValue = Convert.ToString(value, 2).PadLeft(10, '0');
                sequenceBuilder.Append(binaryValue);
            }

            /* If the data length isn't divisible by 3, we encode last characters using 
               different amount of bits */
            if (data.Length % 3 != 0)
            {
                short value = Convert.ToInt16(data[(data.Length - data.Length % 3)..]);
                string binaryValue = Convert.ToString(value, 2);
                
                /* If we have one last digit, we encode it into a 4-bit number, 
                   if we have two digits - into a 7-bit number */
                int binaryValueLength = (data.Length % 3 == 2) ? 7 : 4;
                binaryValue = binaryValue.PadLeft(binaryValueLength, '0');

                sequenceBuilder.Append(binaryValue);
            }

            return sequenceBuilder.ToString();
        }

        /* This method is used to encode data into a sequence of bits in case of using
           alphanumeric encoding */
        private static string EncodeUsingAlphanumericEncoding(string data)
        {
            StringBuilder sequenceBuidler = new StringBuilder();

            /* Iterating the data to the end excluding the last character
               (if the data length isn't divisible by 2) */
            for (int i = 0; i < data.Length - data.Length % 2; i += 2)
            {
                // Getting corresponding values from the table
                byte firstDigit = _alphaNumericTable[data[i]];
                byte secondDigit = _alphaNumericTable[data[i + 1]];

                /* Here we calculate the value using the following rule:
                   value = firstDigit * 45 + secondDigit,
                   then it's filled with zeros up to 11 bits */
                short value = (short)(firstDigit * 45 + secondDigit);
                string binaryValue = Convert.ToString(value, 2).PadLeft(11, '0');
                sequenceBuidler.Append(binaryValue);
            }

            /* If the data length isn't divisible by 2, we encode last character using 
               6 bits */
            if (data.Length % 2 != 0)
            {
                int digit = _alphaNumericTable[data[^1]];
                string binaryValue = Convert.ToString(digit, 2).PadLeft(6, '0');
                sequenceBuidler.Append(binaryValue);
            }

            return sequenceBuidler.ToString();
        }

        /* This method is used to encode data into a sequence of bits in case of using
           byte encoding */
        private static string EncodeUsingByteEncoding(string data)
        {
            byte[] utf8Codes = Encoding.UTF8.GetBytes(data); // This array contains UTF-8 codes of each inputted character
            StringBuilder sequenceBuilder = new StringBuilder(); 

            for (int i = 0; i < utf8Codes.Length; ++i)
            {
                // Converting current value into a 8-bit binary number
                string binaryValue = Convert.ToString(utf8Codes[i], 2).PadLeft(8, '0');
                sequenceBuilder.Append(binaryValue);
            }

            return sequenceBuilder.ToString();
        }

        /// <summary>
        /// This method is used to encode a given data into a sequence of bits.
        /// </summary>
        /// <param name="data">The data to be encoded</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static void Encode(string data)
        {
            if (data.Length == 0) //// Will be changed
            {
                throw new ArgumentException("Unable to encode an empty string, please try again.");
            }

            string bitSequence;

            // Based on the chosen encoding type, we encode the data into a sequence of bits
            switch (Configuration.EncodingMethod)
            {
                case EncodingMethod.Numeric: 
                {
                    bitSequence = EncodeUsingDigitalEncoding(data);
                    break; 
                }
                case EncodingMethod.Alphanumeric: 
                {
                    bitSequence = EncodeUsingAlphanumericEncoding(data); 
                    break;
                }
                case EncodingMethod.Binary:
                {
                    bitSequence = EncodeUsingByteEncoding(data);
                    break; 
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }

            Configuration.BitSequence = bitSequence;
        }
    }
}
