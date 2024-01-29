#pragma warning disable IDE0230

using System;
using System.Collections.Generic;
using System.Linq;

namespace QR_Code_Generator.Model
{
    /// <summary>
    /// This class is used to prepare information to be placed on a QR code
    /// </summary>
    internal static class InformationPlacement
    {
        /* This array contains the grid nodes vertically and horizontally
           where the center modules of the alignment patterns are located */
        private static readonly byte[]?[] s_alignmentPatternsPlacement =
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

        // This list contains the coordinates of each center of the alignment pattern 
        private static List<byte[]>? s_alignmentPatternsCoords = null;

        // This array contains the version codes for each version starting with version 7
        private static readonly string[] s_versionCodes =
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

        /* The next 4 arrays contain mask codes depending on the correction level
           This array corresponds to the L correction level */
        private static readonly string[] s_lMaskCodes =
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

        // This array corresponds to the M correction level
        private static readonly string[] s_mMaskCodes =
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

        // This array corresponds to the Q correction level
        private static readonly string[] s_qMaskCodes =
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

        // This array corresponds to the H correction level
        private static readonly string[] s_hMaskCodes =
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

        // This variable corresponds to one of 8 masks
        private static int s_maskIndex;

        // This property contains the size of the QR code in modules
        public static short QRSize { get; private set; }

        // This property represents the QR code that will be drawn on the bitmap image
        public static byte[][] QRCode { get; private set; }

        /// <summary>
        /// This method is used to place all the necessary information on the
        /// QR code for its subsequent rendering
        /// </summary>
        public static void PlaceInformation() 
        {
            /* The smallest QR code (version 1) has a size of 21 modules. Each next one is larger
               than the previous one by 4 modules */
            QRSize = (short)((Configuration.Version - 1) * 4 + 21);
            QRCode = new byte[QRSize][];

            for (int i = 0; i < QRSize; ++i)
            {
                QRCode[i] = new byte[QRSize];
            }


            PlaceSearchPatterns(); // Placing the search patterns
            PlaceSynchronizationStrips(); // Placing the sync strips
            PlaceAlignmentPatterns(); // Placing the alignment patterns (if version > 1)
            PlaceVersionCode(); // Placing the version code (if version > 6)

            if (Configuration.IsOptimized)
            {
                ChooseBestMask(); /* In case if user chose the optimization,
                                   * we have to select the best mask for a QR-code*/
            }
            else
            {
                Random randomMaskIndex = new();
                s_maskIndex = randomMaskIndex.Next(0, 8);
                PlaceRemainingData(); /* Placing the remaining data (a bit sequence that was obtained 
                                       * by combining data and correction blocks) eeusing random mask */
                PlaceMaskAndCorrectionLevelCodes(); // Placing the mask and correction level codes
            }

            AddIndent(); // Placing the indent (4 white modules around the QR-code for scaenners)

            // All the data is collected and ready to be displayed
        }

        /// <summary>
        /// This method is used to place the 3 search patterns on a QR code
        /// </summary>
        private static void PlaceSearchPatterns()
        {
            // Upper-left search pattern
            for (int i = 0; i < 7; ++i)
            {
                for (int j = 0; j < 7; ++j)
                {
                    if ((i == 1 || i == 5 || j == 1 || j == 5) &&
                        (i != 0 && i != 6 && j != 0 && j != 6)) continue;

                    QRCode[i][j] = 1;
                }
            }

            int leftBound = QRSize - 7; // Left bound for upper-right pattern

            // Upper-right search pattern
            for (int i = 0; i < 7; ++i)
            {
                for (int j = leftBound; j < QRSize; ++j)
                {
                    if ((i == 1 || i == 5 || j == leftBound + 1 || j == QRSize - 2) &&
                        (i != 0 && i != 6 && j != leftBound && j != QRSize - 1)) continue;

                    QRCode[i][j] = 1;
                }
            }

            int upperBound = QRSize - 7; // Upper bound for down-left pattern

            // Down-left search pattern
            for (int i = upperBound; i < QRSize; ++i)
            {
                for (int j = 0; j < 7; ++j)
                {
                    if ((i == upperBound + 1 || i == QRSize - 2 || j == 1 || j == 5) &&
                        (i != upperBound && i != QRSize - 1 && j != 0 && j != 6)) continue;

                    QRCode[i][j] = 1;
                }
            }
        }

        /// <summary>
        /// This method is used to place the sync strips on a QR code
        /// </summary>
        private static void PlaceSynchronizationStrips()
        {
            // The horizontal strip starts to the right of the bottom right module of the top-left search pattern
            int horizontalStripStart = 6;
            int horizontalStripEnd = QRSize - 8;

            // The strip is a line of alternating modules
            byte currentModule = 1;

            for (int i = horizontalStripStart; i < horizontalStripEnd; ++i)
            {
                QRCode[6][i] = currentModule;
                currentModule = (byte)(1 - currentModule); // Switching the module 
            }

            // The vertical strip starts to the bottom of the bottom right module of the top-left search pattern
            int verticalStripStart = 8;
            int verticalStripEnd = QRSize - 8;
            currentModule = 1;

            for (int i = verticalStripStart; i < verticalStripEnd; ++i)
            {
                QRCode[i][6] = currentModule;
                currentModule = (byte)(1 - currentModule); // Switching the module 
            }
        }

        /// <summary>
        /// This method is used to place the alignment patterns on a QR code
        /// </summary>
        private static void PlaceAlignmentPatterns()
        {
            // The smallest QR code has no alignment patterns
            if (Configuration.Version == 1) return;

            if (s_alignmentPatternsCoords?.Count != 0) s_alignmentPatternsCoords?.Clear();

            s_alignmentPatternsCoords = new();
            byte[] alignmentPatterns = s_alignmentPatternsPlacement[Configuration.Version - 1];

            for (int i = 0; i < alignmentPatterns.Length; ++i)
            {
                for (int j = 0; j < alignmentPatterns.Length; ++j)
                {
                    /* Starting from the version 7, the alignment patterns can overlap with search patterns.
                     * In this case we don't render them at the points (first, first), (first, last), (last, first) */
                    if (Configuration.Version > 6)
                    {
                        if (i == 0 && j == 0) continue;
                        else if (i == 0 && j == alignmentPatterns.Length - 1) continue;
                        else if (i == alignmentPatterns.Length - 1 && j == 0) continue;
                    }

                    int centerPointI = alignmentPatterns[i];
                    int centerPointJ = alignmentPatterns[j];

                    // Once we get the center point of the current pattern, we have to render its borders
                    FillAlignmentPattern(centerPointI, centerPointJ);
                    // Saving the center point of the pattern in the list
                    s_alignmentPatternsCoords.Add(new byte[2] { alignmentPatterns[i], alignmentPatterns[j] } );
                    #nullable enable
                }
            }
        }


        /// <summary>
        /// This method is used to place the borders of the alignment pattern on a QR code
        /// </summary>
        private static void FillAlignmentPattern(int centerPointI, int centerPointJ)
        {
            // Getting the borders for the alignment pattern
            int leftBorder = centerPointI - 2;
            int rightBorder = centerPointI + 3;
            int topBorder = centerPointJ - 2;
            int bottomBorder = centerPointJ + 3;

            for (int i = topBorder; i < bottomBorder; ++i)
            {
                for (int j = leftBorder; j < rightBorder; ++j)
                {
                    // Placing modules on the borders of the pattern
                    if (i == topBorder || i == bottomBorder - 1 || j == leftBorder || j == rightBorder - 1)
                    {
                        QRCode[i][j] = 1;
                    }
                }
            }

            QRCode[centerPointI][centerPointJ] = 1; // Placing the center module of a pattern
        }

        /// <summary>
        /// This method is used to place the version code on a QR code
        /// </summary>
        private static void PlaceVersionCode()
        {
            // If the version is lower than 7, we don't put the version code
            if (Configuration.Version < 7) return;

            string code = s_versionCodes[Configuration.Version - 7];
            string[] parts = code.Split(' ');

            for (int i = 0; i < 3; ++i)
            {
                int jPos = QRSize - 11 + i;

                for (int j = 0; j < 6; ++j)
                {
                    if (parts[i][j] == '1')
                    {
                        /* Because the version code is duplicated in 2 places mirrored,
                         * we simply swap out the coordinates */
                        QRCode[j][jPos] = 1;
                        QRCode[jPos][j] = 1;
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to place mask and correction level codes on a QR code
        /// </summary>
        private static void PlaceMaskAndCorrectionLevelCodes()
        {
            string[] maskCode = Array.Empty<string>();

            // Mask code depends on the correction level
            switch (Configuration.CorrectionLevel)
            {
                case CorrectionLevel.L: { maskCode = s_lMaskCodes; break; }
                case CorrectionLevel.M: { maskCode = s_mMaskCodes; break; }
                case CorrectionLevel.Q: { maskCode = s_qMaskCodes; break; }
                case CorrectionLevel.H: { maskCode = s_hMaskCodes; break; }
            }

            string mask = maskCode[s_maskIndex];

            for (int i = 0; i < 6; ++i)
            {
                byte bit = (byte)(mask[i] == '1' ? 1 : 0);
                QRCode[8][i] = bit;
            }

            QRCode[8][7] = (byte)(mask[6] == '1' ? 1 : 0);
            QRCode[8][8] = (byte)(mask[7] == '1' ? 1 : 0);
            QRCode[7][8] = (byte)(mask[8] == '1' ? 1 : 0);

            for (int i = 0; i < 6; ++i)
            {
                byte bit = (byte)(mask[i + 9] == '1' ? 1 : 0);
                QRCode[5 - i][8] = bit;
            }

            for (int i = 0; i < 7; ++i)
            {
                byte bit = (byte)(mask[i] == '1' ? 1 : 0);
                QRCode[QRSize - 1 - i][8] = bit;
            }

            QRCode[QRSize - 8][8] = 1; // This module is always black

            for (int i = 0; i < 8; ++i)
            {
                byte bit = (byte)(mask[i + 7] == '1' ? 1 : 0);
                QRCode[8][QRSize - 8 + i] = bit;
            }

        }

        /// <summary>
        /// This method is used to place a bit sequence on a QR code
        /// </summary>
        private static void PlaceRemainingData()
        {
            int dataLength = Configuration.BitSequence.Length;

            int iPos = QRSize - 1; // Starting from the bottom-right 
            int jPos = QRSize - 1;
            bool direction = true; // The direction of module placement. true - up, false - down

            for (int i = 0; i < dataLength; ++i)
            {
                byte bitToPlace = Convert.ToByte(Configuration.BitSequence[i] - 48);
                byte mask = ApplyMask(iPos, jPos);

                if (mask == 0)
                {
                    bitToPlace = (byte)(1 - bitToPlace); // Inverting the bit
                }

                /* A data module may overlap with an alignment pattern, a sync strip and a version code.
                   If the current module belongs to one of these, this module is skipped */
                if (!(IsAlignmentPattern(iPos, jPos) ||
                    IsSynchronizationStrip(iPos, jPos) ||
                    IsVersionCode(iPos, jPos)))
                {
                    QRCode[iPos][jPos] = bitToPlace; // If this module is free, we place there a bit of data
                }
                else
                {
                    i--; // if this module is not free, we have to prevent data loss by decreasing the index
                }

                // If we reached the end of the current column, we switch the module placement direction
                if (IsEndOfTheColumn(iPos, jPos, direction))
                {
                    // If we reached the end of the column that adjacent to the lower left search pattern
                    if (iPos == QRSize - 1 && jPos == 9)
                    {
                        iPos = QRSize - 9;
                        jPos = 8;
                    }
                    else if (jPos == 7) jPos -= 2; // If we reached the vertical sync strip, it should be skipped
                    else jPos--;

                    direction = !direction; // Switching the module placement direction
                }
                else
                {
                    if (direction)
                    {
                        // If we haven't reached the vertical sync strip
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
                /* In case if we've reached the last module on which it is possible to place a data bit,
                   we break out of the cycle */
                if (iPos == QRSize - 9 && jPos == 0) break;

            }
        }

        /// <summary>
        /// This method is used to determine if a module belongs to an alignment pattern
        /// </summary>
        private static bool IsAlignmentPattern(int row, int column)
        {
            if (Configuration.Version == 1) return false;

            foreach (byte[] center in s_alignmentPatternsCoords)
            {
                byte iCenter = center[0];
                byte jCenter = center[1];

                // If module is at a distance of 2 from the center of the pattern
                if (Math.Abs(iCenter - row) <= 2 && Math.Abs(jCenter - column) <= 2) return true;
            }

            return false;
        }

        /// <summary>
        /// This method is used to determine if a module belongs to a synchronization strip
        /// </summary>
        private static bool IsSynchronizationStrip(int row, int column) => (row == 6) || (column == 6);

        /// <summary>
        /// This method is used to determine if a module belongs to a version code
        /// </summary>
        private static bool IsVersionCode(int row, int column)
        {
            // If the version is lower than 7, there is no version code on a QR code
            if (Configuration.Version < 7) return false;

            // If the module is on the code to the left of the top-right search pattern
            if (Math.Abs(column - (QRSize - 10)) <= 1 && 0 <= row && row <= 5) return true;

            // If the module is on the code at the top of the bottom-left search pattern
            else if (0 <= column && column <= 5 && Math.Abs(row - (QRSize - 10)) <= 1) return true;
            else return false;
        }

        /// <summary>
        /// This method is used to determine if we reached the end of the current column
        /// </summary>
        private static bool IsEndOfTheColumn(int row, int column, bool direction)
        {
            if (direction) // If we go up
            {
                if (row == 9 && (column <= 8 || column >= QRSize - 8) || row == 0)
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

                if (row == QRSize - 9 && column <= 8 || row == QRSize - 1)
                {
                    if (column > 6 && column % 2 != 0) return true;
                    else if (column > 6 && column % 2 == 0) return false;
                    else if (column < 6 && column % 2 == 0) return true;
                    else return false;
                }
                else return false;
            }
        }
        
        /// <summary>
        /// This method is used to apply particular mask to the data modules
        /// </summary>
        private static byte ApplyMask(int row, int column)
        {
            return s_maskIndex switch
            {
                0 => (byte)((column + row) % 2),
                1 => (byte)(row % 2),
                2 => (byte)(column % 3),
                3 => (byte)((column + row) % 3),
                4 => (byte)((column / 3 + row / 2) % 2),
                5 => (byte)((column * row) % 2 + (column * row) % 3),
                6 => (byte)(((column * row) % 2 + (column * row) % 3) % 2),
                7 => (byte)(((column * row) % 3 + (column + row) % 2) % 2),
                _ => throw new()
            };
        }

        /// <summary>
        /// This method is used to find the most suitable mask for a QR-code
        /// </summary>
        private static void ChooseBestMask()
        {
            byte[][] QRCopy = new byte[QRSize][];
            for (int i = 0; i < QRSize; i++)
            {
                QRCopy[i] = new byte[QRSize];
            }

            int currentMaxPenaltyPoints = int.MaxValue;

            for (int maskIndex = 0; maskIndex < 8; ++maskIndex)
            {
                s_maskIndex = maskIndex;
                PlaceRemainingData();
                PlaceMaskAndCorrectionLevelCodes();

                int currentPenaltyPoints = CountPenaltyPoints();

                if (currentPenaltyPoints < currentMaxPenaltyPoints)
                {
                    currentMaxPenaltyPoints = currentPenaltyPoints;
                    Array.Copy(QRCode, QRCopy, QRCode.Length);
                }
            }
            
            Array.Copy(QRCopy, QRCode, QRCopy.Length);
        }

        /// <summary>
        /// This method is used to get the penalty points for a QR-code with a currently applied mask
        /// </summary>
        private static int CountPenaltyPoints()
        {
            int penaltyPoints = 0;

            #region Rule 1
            /* The rule 1 states:
             * Horizontally and vertically, for every 5 or more consecutive modules of the same color,
             * the number of points equal to the length of that section minus 2 will be awarded */

            // Checking horizontal sections
            for (int row = 0; row < QRSize; ++row)
            {
                int leftPtr = 0, rightPtr = 0;
                bool canPointsBeAdded = true;

                while ((rightPtr != QRSize - 1) && (leftPtr != rightPtr && rightPtr != 0)) // Going through the current row until both pointers reach its end
                {
                    if ((rightPtr != QRSize - 1) && (QRCode[row][leftPtr] == QRCode[row][rightPtr]))
                    {
                        if (!canPointsBeAdded) 
                            canPointsBeAdded = true;

                        rightPtr++;
                    }
                    else
                    {
                        if (canPointsBeAdded && (rightPtr - leftPtr) >= 5)
                        {
                            if (rightPtr == QRSize - 1) 
                                penaltyPoints += rightPtr - leftPtr - 1; /* The difference appears because
                                                                          * we can't move the right pointer 
                                                                          * beyond the end of the row */
                            else
                                penaltyPoints += rightPtr - leftPtr - 2;
                            
                            canPointsBeAdded = false;
                        }

                        leftPtr++;
                    }
                }
            }

            // Checking vertical sections
            for (int column = 0; column < QRSize; ++column)
            {
                int upPtr = 0, downPtr = 0;
                bool canPointsBeAdd = true;

                while ((downPtr != QRSize - 1) && (upPtr != downPtr && downPtr != 0)) // Going through the current column until both pointers reach its end
                {
                    if ((downPtr != QRSize - 1) && (QRCode[upPtr][column] == QRCode[downPtr][column]))
                    {
                        if (!canPointsBeAdd) 
                            canPointsBeAdd = true;
                        
                        downPtr++;
                    }
                    else
                    {
                        if (canPointsBeAdd && (downPtr - upPtr) >= 5)
                        {
                            if (downPtr == QRSize - 1)
                                penaltyPoints += downPtr - upPtr - 1; // The same difference appears

                            else
                                penaltyPoints -= downPtr - upPtr - 2;

                            canPointsBeAdd = false;
                        }

                        upPtr++;
                    }
                }
            }

            #endregion

            #region Rule 2
            /* The rule 2 states:
             * For each 2x2 square of modules of the same color 3 points will be awarded */

            for (int i = 0; i < QRSize - 1; ++i)
            {
                for (int j = 0; j < QRSize - 1; ++j)
                {
                    if (QRCode[i][j] == QRCode[i][j + 1] && 
                        QRCode[i][j + 1] == QRCode[i + 1][j + 1] &&
                        QRCode[i + 1][j + 1] == QRCode[i + 1][j])
                    {
                        penaltyPoints += 3;
                    }
                }
            }

            #endregion

            #region Rule 3
            /* The rule 3 states:
             * For each sequence of BWBBBWB modules (B - black, W - white) with 4 white modules
             * on one side (or 2 at once), 40 points will be awarded */

            for (int i = 0; i < QRSize; ++i)
            {
                for (int j = 0; j < QRSize - 7; ++j)
                {
                    if (IsPenaltySequence(i, j))
                    {
                        penaltyPoints += 40;
                    }
                }
            }

            #endregion

            #region Rule 4
            /* The rule 4 states:
             * Divite the number of black modules by the total number of modules, multiply the 
             * result by 100 and subtract 50, then discard the decimal part and take the modulus
             * of the number, and finally multiply the resulting number by 2 and add it to the
             * penalty points. */

            int blackModulesCount = 0;

            for (int i = 0; i < QRSize; ++i)
            {
                for (int j = 0; j < QRSize; ++j)
                {
                    if (QRCode[i][j] == 1) blackModulesCount++;
                }
            }

            penaltyPoints += Math.Abs((int)(Math.Floor(((double)(blackModulesCount) / (QRSize * QRSize) * 100 - 50)))) * 2; // yeah

            #endregion

            return penaltyPoints;
        }

        /// <summary>
        /// This method is used to detect the sequence by rule 3
        /// </summary>
        private static bool IsPenaltySequence(int row, int column)
        {
            byte[] middleSequence = { 1, 0, 1, 1, 1, 0, 1 };
            byte[] sideSequence = { 0, 0, 0, 0 };

            if (!QRCode[row][column..(column + 7)].SequenceEqual(middleSequence))
            {
                return false;
            }

            if (column > 3 && QRCode[row][(column - 4)..column].SequenceEqual(sideSequence))
            {
                return true;
            }

            else if (column < QRSize - 11 && QRCode[row][(column + 7)..(column + 11)].SequenceEqual(sideSequence))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method is used to add an 4-module indent to a QR code
        /// </summary>
        private static void AddIndent()
        {
            byte[][] qrCodeWithIndent = new byte[QRSize + 8][];

            for (int i = 0; i < QRSize + 8; ++i)
            {
                qrCodeWithIndent[i] = new byte[QRSize + 8];
            }

            for (int i = 4; i < QRSize + 4; ++i)
            {
                for (int j = 4; j < QRSize + 4; ++j)
                {
                    qrCodeWithIndent[i][j] = QRCode[i - 4][j - 4];
                }
            }

            QRCode = qrCodeWithIndent;
        }
    }
}
