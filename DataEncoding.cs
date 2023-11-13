using System;
using System.Text;

namespace QR_Code_Generator
{
    /// <summary>
    /// A class that encodes data into a sequence of bytes.
    /// </summary>
    internal static class DataEncoding
    {
        public static string EncodeDataIntoAByteSequence(string data)
        {
            if (data.Length == 0)
            {
                throw new ArgumentException("Unable to encode an empty string, please try again.");
            }

            byte[] utf8Codes = Encoding.UTF8.GetBytes(data); // This array contains UTF-8 codes of each inputted character
            StringBuilder bitSequence = new StringBuilder(); // A bit sequence of the original data

            for (int i = 0; i < utf8Codes.Length; i++)
            {
                string binaryCode = Convert.ToString(utf8Codes[i], 2); // A binary representation of current byte

                while (binaryCode.Length < 8)
                {
                    binaryCode = "0" + binaryCode; // If the binary representation is less than 8 bits
                }

                bitSequence.Append(binaryCode);
            }

            return bitSequence.ToString();
        }
    }
}
