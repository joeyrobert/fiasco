/* Copyright (c) 2009 Joseph Robert. All rights reserved.
 *
 * This file is part of Fiasco.
 * 
 * Fiasco is free software; you can redistribute it and/or modify it 
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 3.0  of 
 * the License, or (at your option) any later version.
 * 
 * Fiasco is distributed in the hope that it will be useful, but 
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU 
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License 
 * along with Fiasco.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;

namespace Fiasco.Engine
{
    public class Eval
    {
        public const int KVALUE = 100000;
        public const int QVALUE = 900;
        public const int RVALUE = 500;
        public const int BVALUE = 300;
        public const int NVALUE = 300;
        public const int PVALUE = 100;

        private static Dictionary<int, int> _pieceValues = new Dictionary<int, int>()
        {
            {Definitions.K, KVALUE},
            {Definitions.Q, QVALUE},
            {Definitions.R, RVALUE},
            {Definitions.B, BVALUE},
            {Definitions.N, NVALUE},
            {Definitions.P, PVALUE},
            {Definitions.OFF, 0},
            {Definitions.EMPTY, 0}
        };

        #region Piece positioning
        public static int PieceValue(int piece)
        {
            return _pieceValues[piece];
        }

        private static int SumValues(Board board)
        {
            int score = 0;
            int colour;

            for (int i = 21; i < 99; i++)
            {
                colour = board.ColourArray[i];

                // this switch is much faster (3x) than  
                // something implemented with PieceValue()
                switch (board.PieceArray[i])
                {
                    case Definitions.K:
                        score += colour * KVALUE;
                        break;
                    case Definitions.Q:
                        score += colour * QVALUE;
                        break;
                    case Definitions.R:
                        score += colour * RVALUE;
                        break;
                    case Definitions.N:
                        score += colour * NVALUE;
                        break;
                    case Definitions.B:
                        score += colour * BVALUE;
                        break;
                    case Definitions.P:
                        score += colour * PVALUE;
                        break;
                }
            }

            return score;
        }

        private static int CenterValues(Board board)
        {
            int score = 0;
            int colour;

            for (int i = 4; i <= 7; i++)
            {
                for (int j = 3; j <= 6; j++)
                {
                    int position = i * 10 + j;
                    colour = board.ColourArray[position];
                    int value = 0;

                    switch (board.PieceArray[position])
                    {
                        case Definitions.K:
                            value = -1 * colour * PVALUE; // bad for the king to be in the center
                            break;
                        case Definitions.N:
                            value = colour * NVALUE;
                            break;
                        case Definitions.P:
                            value = colour * PVALUE;
                            break;
                    }

                    // bonus if you're in the very center
                    if (position == 64 || position == 65 || position == 54 || position == 55)
                        score += (int)(0.20 * value);
                    else
                        score += (int)(0.10 * value);
                }
            }

            return score;
        }
        #endregion

        #region Castling
        /// <summary>
        /// Adds a small factor if one can castle. Not being used.
        /// </summary>
        private static int Castling(Board board)
        {
            int resultant = 0;

            if ((board.Castling & 1) != 0) resultant += 25;
            if ((board.Castling & 2) != 0) resultant += 25;
            if ((board.Castling & 4) != 0) resultant -= 25;
            if ((board.Castling & 8) != 0) resultant -= 25;

            return resultant;
        }
        #endregion

        public static int Board(Board board)
        {
            int pieceValues = SumValues(board);
            int centerValues = CenterValues(board);

            return board.Turn * (pieceValues + centerValues);
        }
    }
}
