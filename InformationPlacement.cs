#nullable disable

using System;
using System.Collections.Generic;
using System.Drawing;

namespace QR_Code_Generator
{
    internal static class InformationPlacement
    {
        private static readonly byte[][] _alignmentPatternsPlacement = 
        {
            null,
            new byte[] { 18 },
            new byte[] { 22 },
            new byte[] { 26 },
            new byte[] { 30 },
            new byte[] { 34 },
            new byte[] { 6, 22, 38 },
            new byte[] { 6, 24, 42 },
            new byte[] { 6, 26, 46 },
            new byte[] { 6, 28, 50 },
            new byte[] { 6, 30, 54 },
            new byte[] { 6, 32, 58 },
            new byte[] { 6, 34, 62 },
            new byte[] { 6, 26, 46, 66 },
            new byte[] { 6, 26, 48, 70 },
            new byte[] { 6, 26, 50, 74 },
            new byte[] { 6, 30, 54, 78 },
            new byte[] { 6, 30, 56, 82 },
            new byte[] { 6, 30, 58, 86 },
            new byte[] { 6, 34, 62, 90 },
            new byte[] { 6, 28, 50, 72, 94 },
            new byte[] { 6, 26, 50, 74, 98 },
            new byte[] { 6, 30, 54, 78, 102 },
            new byte[] { 6, 28, 54, 80, 106 },
            new byte[] { 6, 32, 58, 84, 110 },
            new byte[] { 6, 30, 58, 86, 114 },
            new byte[] { 6, 34, 62, 90, 118 },
            new byte[] { 6, 26, 50, 74, 98, 122 }, 
            new byte[] { 6, 30, 54, 78, 102, 126 },
            new byte[] { 6, 26, 52, 78, 104, 130 },
            new byte[] { 6, 30, 56, 82, 108, 134 },
            new byte[] { 6, 34, 60, 86, 112, 138 },
            new byte[] { 6, 30, 58, 86, 114, 142 },
            new byte[] { 6, 34, 62, 90, 118, 146 },
            new byte[] { 6, 30, 54, 78, 102, 126, 150 },
            new byte[] { 6, 24, 50, 76, 102, 128, 154 },
            new byte[] { 6, 28, 54, 80, 106, 132, 158 },
            new byte[] { 6, 32, 58, 84, 110, 136, 162 },
            new byte[] { 6, 26, 54, 82, 110, 138, 166 },
            new byte[] { 6, 30, 58, 86, 114, 142, 170 }
        };

        private static readonly string[] versionCodes =
        {
            "000010 011110 100110",
            "010001 011100 111000",
            "110111 011000 000100",
            "101001 111110 000000",
            "001111 111010 111100",
            "001101 100100 011010",
            "101011 100000 100110",
            "110101 000110 100010",
            "010011 000010 011110",
            "011100 010001 011100",
            "111010 010101 100000",
            "100100 110011 100100",
            "000010 110111 011000",
            "000000 101001 111110",
            "100110 101101 000010",
            "111000 001011 000110",
            "011110 001111 111010",
            "001101 001101 100100",
            "101011 001001 011000",
            "110101 101111 011100",
            "010011 101011 100000",
            "010001 110101 000110",
            "110111 110001 111010",
            "101001 010111 111110",
            "001111 010011 000010",
            "101000 011000 101101",
            "001110 011100 010001",
            "010000 111010 010101",
            "110110 111110 101001",
            "110100 100000 001111",
            "010010 100100 110011",
            "001100 000010 110111",
            "101010 000110 001011",
            "111001 000100 010101"
        };

        private static readonly string[] _lMaskCodes =
        {
            "111011111000100",
            "111001011110011",
            "111110110101010",
            "111100010011101",
            "110011000101111",
            "110001100011000",
            "110110001000001",
            "110100101110110"
        };

        private static readonly string[] _mMaskCodes =
        {
            "101010000010010",
            "101000100100101",
            "101111001111100",
            "101101101001011",
            "100010111111001",
            "100000011001110",
            "100111110010111",
            "100101010100000"
        };

        private static readonly string[] _qMaskCodes =
        {
            "011010101011111",
            "011000001101000",
            "011111100110001",
            "011101000000110",
            "010010010110100",
            "010000110000011",
            "010111011011010",
            "010101111101101"
        };

        private static readonly string[] _hMaskCodes =
        {
            "001011010001001",
            "001001110111110",
            "001110011100111",
            "001100111010000",
            "000011101100010",
            "000001001010101",
            "000110100001100",
            "000100000111011"
        };

        private static short _QRSize;

        private static byte[][] _QRCode;

        private static List<byte[]> _alignmentPatternsCoords;

        /// <summary>
        /// 
        /// </summary>
        public static void PlaceInformation()
        {
            _QRSize = (short)((Configuration.Version - 1) * 4 + 21);
            _QRCode = new byte[_QRSize][];

            for (int i = 0; i < _QRSize; ++i)
            {
                _QRCode[i] = new byte[_QRSize];
            }

            PlaceSearchPatterns();
            PlaceSynchronizationStrips();
            PlaceAlignmentPatterns();
            PlaceVersionCode();
            PlaceRemainingData();
            PlaceMaskAndCorrectionLevelCodes();
        }

        private static void PlaceSearchPatterns()
        {
            // Upper-left search pattern
            for (int i = 0; i < 7; ++i)
            {
                for (int j = 0; j < 7; ++j) 
                {
                    if ((i == 1 || i == 5 || j == 1 || j == 5) &&
                        (i != 0 && i != 6 && j != 0 && j != 6)) continue;

                    _QRCode[i][j] = 1;
                }
            }

            int leftBound = _QRSize - 7; // Left bound for upper-right pattern

            // Upper-right search pattern
            for (int i = 0; i < 7; ++i)
            {
                for (int j = leftBound; j < _QRSize; ++j)
                {
                    if ((i == 1 || i == 5 || j == leftBound + 1 || j == _QRSize - 2) && 
                        (i != 0 && i != 6 && j != leftBound && j != _QRSize - 1)) continue;

                    _QRCode[i][j] = 1;
                }
            }

            int upperBound = _QRSize - 7; // Upper bound for down-left pattern

            // Down-left search pattern
            for (int i = upperBound; i < _QRSize; ++i)
            {
                for (int j = 0; j < 7; ++j)
                {
                    if ((i == upperBound + 1 || i == _QRSize - 2 || j == 1 || j == 5) &&
                        (i != upperBound && i != _QRSize - 1 && j != 0 && j != 6)) continue;

                    _QRCode[i][j] = 1;
                }
            }
        }

        private static void PlaceSynchronizationStrips()
        {
            int horizontalStripStart = 6;
            int horizontalStripEnd = _QRSize - 8;
            byte currentModule = 1;

            for (int i = horizontalStripStart; i < horizontalStripEnd; ++i)
            {
                _QRCode[6][i] = currentModule;
                currentModule = (byte)(1 - currentModule);
            }

            int verticalStripStart = 8;
            int verticalStripEnd = _QRSize - 8;
            currentModule = 1;

            for (int i =  verticalStripStart; i < verticalStripEnd; ++i)
            {
                _QRCode[i][6] = currentModule;
                currentModule = (byte)(1 - currentModule);
            }
        }

        private static void PlaceAlignmentPatterns()
        {
            if (Configuration.Version == 1) return;

            _alignmentPatternsCoords = new List<byte[]>();
            byte[] alignmentPatterns = _alignmentPatternsPlacement[Configuration.Version - 1];

            for (int i = 0; i < alignmentPatterns.Length; ++i)
            {
                for (int j = 0; j < alignmentPatterns.Length; ++j)
                {
                    if (Configuration.Version > 6)
                    {
                        if (i == 0 && j == 0) continue;
                        else if (i == 0 && j == alignmentPatterns.Length - 1) continue;
                        else if (i == alignmentPatterns.Length - 1 && j == 0) continue;
                    }

                    int centerPointX = alignmentPatterns[i];
                    int centerPointY = alignmentPatterns[j];

                    FillAlignmentPattern(centerPointX, centerPointY);
                    _alignmentPatternsCoords.Add(new byte[2] { alignmentPatterns[i], alignmentPatterns[j] });
                }
            }
        }

        private static void FillAlignmentPattern(int centerPointX, int centerPointY)
        {
            // Getting the borders for the pattern
            int leftBorder = centerPointX - 2;
            int rightBorder = centerPointX + 3;
            int topBorder = centerPointY - 2;
            int bottomBorder = centerPointY + 3;

            for (int i = topBorder; i < bottomBorder; ++i)
            {
                for (int j = leftBorder; j < rightBorder; ++j)
                {
                    // Placing modules on tht borders of the pattern
                    if (i == topBorder || i == bottomBorder - 1 || j == leftBorder || j == rightBorder - 1)
                    {
                        _QRCode[i][j] = 1;
                    }
                    else
                    {
                        _QRCode[i][j] = 0;
                    }
                }
            }

            _QRCode[centerPointY][centerPointX] = 1; // Placing the center module of a pattern
        }

        // refactor
        private static void PlaceVersionCode()
        {
            if (Configuration.Version < 7) return;

            string code = versionCodes[Configuration.Version - 7];
            string[] parts = code.Split(' ');

            for (int i = 0;  i < 3; ++i)
            {
                int jPos = _QRSize - 11 + i;

                for (int j = 0; j < 6; ++j)
                {
                    if (parts[i][j] == '1') 
                    {
                        _QRCode[j][jPos] = 1;
                        _QRCode[jPos][j] = 1;
                    }
                }
            }
        }

        private static void PlaceMaskAndCorrectionLevelCodes()
        {
            string[] maskCode;

            switch (Configuration.CorrectionLevel)
            {
                case CorrectionLevel.L: { maskCode = _lMaskCodes; break; }
                case CorrectionLevel.M: { maskCode = _mMaskCodes; break; }
                case CorrectionLevel.Q: { maskCode = _qMaskCodes; break; }
                case CorrectionLevel.H: { maskCode = _hMaskCodes; break; }
                default: { throw new NotSupportedException(); }
            }

            string mask = maskCode[0]; // DEFAULT

            for (int i = 0; i < 6; ++i)
            {
                byte bit = (byte)(mask[i] == '1' ? 1 : 0);
                _QRCode[8][i] = bit;
            }

            _QRCode[8][7] = (byte)(mask[6] == '1' ? 1 : 0);
            _QRCode[8][8] = (byte)(mask[7] == '1' ? 1 : 0);
            _QRCode[7][8] = (byte)(mask[8] == '1' ? 1 : 0);

            for (int i = 0; i < 6; ++i) 
            {
                byte bit = (byte)(mask[i + 9] == '1' ? 1 : 0);
                _QRCode[5 - i][8] = bit;
            }

            for (int i = 0; i < 7; ++i)
            {
                byte bit = (byte)(mask[i] == '1' ? 1 : 0);
                _QRCode[_QRSize - 1 - i][8] = bit;
            }

            _QRCode[_QRSize - 8][8] = 1; // This module is always black

            for (int i = 0; i < 8; ++i)
            {
                byte bit = (byte)(mask[i + 7] == '1' ? 1 : 0);
                _QRCode[8][_QRSize - 8 + i] = bit;
            }

        }

        private static void PlaceRemainingData()
        {
            int dataLength = Configuration.BitSequence.Length;

            int iPos = _QRSize - 1; // Starting from the bottom-right 
            int jPos = _QRSize - 1;
            bool direction = true; // The direction of module placement. true - up, false - down

            while (iPos != _QRSize - 9 && jPos != 0) // This is the last bit to place
            {
                for (int i = 0; i < dataLength; ++i)
                {
                    byte bitToPlace = Convert.ToByte(Configuration.BitSequence[i] - 48);
                    byte mask = ApplyMask(iPos, jPos);

                    if (mask == 0)
                    {
                        bitToPlace = (byte)(1 - bitToPlace); // Inverting the bit
                    }

                    if (!(IsAlignmentPattern(iPos, jPos) ||
                        IsSynchronizationStrip(iPos, jPos) ||
                        IsVersionCode(iPos, jPos))) 
                    {
                        _QRCode[iPos][jPos] = bitToPlace; // If this module is free, we place there a bit of data
                    }
                    else
                    {
                        i--; // if this module is not free, we have to prevent data loss by decreasing the index
                    }

                    if (IsEndOfTheColumn(iPos, jPos, direction))
                    {
                        if (iPos == _QRSize - 1 && jPos == 9)
                        {
                            iPos = _QRSize - 9;
                            jPos = 8;
                        }
                        else if (jPos == 7) jPos -= 2;
                        else jPos--;

                        direction = !direction; // Switching the module placement direction
                    }
                    else
                    {
                        if (direction)
                        {
                            if (jPos > 6)
                            {
                                if (jPos % 2 == 0) jPos--;
                                else
                                {
                                    iPos--;
                                    jPos++;
                                }
                            }
                            else
                            {
                                if (jPos % 2 != 0) jPos--;
                                else
                                {
                                    iPos--;
                                    jPos++;
                                }
                            }
                        }
                        else
                        {
                            if (jPos > 6)
                            {
                                if (jPos % 2 == 0) jPos--;
                                else
                                {
                                    iPos++;
                                    jPos++;
                                }
                            }
                            else
                            {
                                if (jPos % 2 != 0) jPos--;
                                else
                                {
                                    iPos++;
                                    jPos++;
                                }
                            }
                        }
                    }
                    if (iPos == _QRSize - 9 && jPos == 0) break;
                }
            }
        }

        private static bool IsAlignmentPattern(int row, int column)
        {
            if (Configuration.Version == 1) return false;

            foreach (byte[] center in _alignmentPatternsCoords)
            {
                byte iCenter = center[0];
                byte jCenter = center[1];

                if (Math.Abs(iCenter - row) <= 2 && Math.Abs(jCenter - column) <= 2) return true;
            }

            return false;
        }

        private static bool IsSynchronizationStrip(int row, int column) => (row == 6) || (column == 6);

        private static bool IsVersionCode(int row, int column)
        {
            if (Configuration.Version < 7) return false;

            if (Math.Abs(column - (_QRSize - 10)) <= 1 && 0 <= row && row <= 5) return true;
            else if (0 <= column && column <= 5 && Math.Abs(row - (_QRSize - 10) ) <= 1) return true;
            else return false;
        }

        private static bool IsEndOfTheColumn(int row, int column, bool direction)
        {
            if (direction) // If we go up
            {
                if (row == 9 && (column <= 8 || column >= _QRSize - 8) || row == 0)
                {
                    if (column > 6 && column % 2 != 0) return true;
                    else if (column > 6 && column % 2 == 0) return false;
                    else if (column < 6 && column % 2 == 0) return true;
                    else return false;
                }
               
                else return false;
            }
            else // If we go down
            {
                
                if (row == _QRSize - 9 && column <= 8 || row == _QRSize - 1)
                {
                    if (column > 6 && column % 2 != 0) return true;
                    else if (column > 6 && column % 2 == 0) return false;
                    else if (column < 6 && column % 2 == 0) return true;
                    else return false;
                }
                else return false;
            }
        }

        private static byte ApplyMask(int row, int column)
        {
            return (byte)((column + row) % 2);
        }

        public static void DisplayQR()
        {
            for (int i = 0; i < _QRSize; ++i)
            {
                for (int j = 0; j < _QRSize; ++j)
                {
                    Console.Write(_QRCode[i][j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        
        public static void Test()
        {
            Bitmap image = new Bitmap(_QRSize * 10 + 80, _QRSize * 10 + 80);

            byte[][] withoutIndent = new byte[_QRSize * 10][];
            for (int i = 0; i < _QRSize * 10; ++i) withoutIndent[i] = new byte[_QRSize * 10];

            for (int i = 0; i < _QRSize; ++i)
            {
                for (int j = 0; j < _QRSize; ++j)
                {
                    withoutIndent[i * 10][j * 10] = _QRCode[i][j];
                    if (_QRCode[i][j] == 1)
                    {
                        FillSquare(ref withoutIndent, i * 10, j * 10);
                    }
                }
            }

            byte[][] withIndent = new byte[_QRSize * 10 + 80][];
            for (int i = 0; i < _QRSize * 10 + 80; ++i) withIndent[i] = new byte[_QRSize * 10 + 80];
            
            for (int i = 40; i < _QRSize * 10 + 40; ++i)
            {
                for (int j = 40; j < _QRSize * 10 + 40; ++j)
                {
                    withIndent[i][j] = withoutIndent[i - 40][j - 40];
                }
            }

            for (int i = 0; i < _QRSize * 10 + 80; ++i)
            {
                for (int j = 0; j < _QRSize * 10 + 80; ++j)
                {
                    if (withIndent[i][j] == 1) image.SetPixel(j, i, Color.Black);
                    else image.SetPixel(j, i, Color.White);
                }
            }

            image.Save("~qr.png");
        }

        private static void FillSquare(ref byte[][] array, int iPos, int jPos)
        {
            for (int i = iPos; i < iPos + 10; ++i)
            {
                for (int j = jPos; j < jPos + 10; ++j)
                {
                    array[i][j] = 1;
                }
            }
        }
    }
}
