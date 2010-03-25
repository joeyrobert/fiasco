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

using System;
using System.Collections.Generic;

namespace Fiasco
{
    public static class Definitions
    {
        #region Constants
        // Piece values
        public const int P = 1;
        public const int B = 2;
        public const int N = 3;
        public const int R = 4;
        public const int Q = 5;
        public const int K = 6;

        // Team values
        public const int WHITE = 1;
        public const int BLACK = -1;

        // Off/empty values
        public const int OFF = 7;
        public const int EMPTY = 0;
        public const int NOENPASSANT = -64;

        public const bool ALLOWRANDOM = false;
        #endregion

        #region Default Board Setups
        public static int[] BlankArray = new int[] { 
                Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, 
                Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, 
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,                
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.EMPTY, Definitions.OFF,
                Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, 
                Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF, Definitions.OFF,             
            };
        #endregion

        #region Move Deltas
        // Move deltas (non sliding pieces)
        public static int[] deltaN = { -21, -19, -12, -8, +8, +12, +19, +21 };
        public static int[] deltaK = { -11, -10, -9, -1, +1, +9, +10, +11 };

        // Move deltas (sliding pieces)
        public static int[] deltaB = { -11, -9, +9, +11 };
        public static int[] deltaR = { -10, -1, +1, +10 };
        #endregion

        #region Row/Column conversions
        /// <summary>
        /// From 1 - 8
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetColumn(int index)
        {
            return index % 10;
        }

        /// <summary>
        /// From 1 - 8
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetRow(int index)
        {
            return (index / 10) - 1;
        }

        public static int GetIndex(int row, int column)
        {
            return (row - 1) * 10 + (column - 1) + 21;            
        }

        public static Dictionary<char, int> columnToNumber = new Dictionary<char, int>()
        {
            {'a', 1}, {'b', 2}, {'c', 3}, {'d', 4}, {'e', 5}, {'f', 6}, {'g', 7}, {'h', 8}
        };

        public static int ChessPieceToIndex(string chessPiece)
        {
            char[] chessParts = chessPiece.ToCharArray();

            int column = columnToNumber[chessParts[0]];
            int row = (int)(chessParts[1] - '0'); // weirdly cast the char to an int

            return GetIndex(row, column);
        }
        #endregion

        #region ToString methods
        // ToString methods
        //
        // These are included here and not in the classes themselves for performance
        // reasons. i.e. not every move generated will need to be converted ToString
        // and having the method there uses unnecessary memory.

        public static string[] columnNames = { "a", "b", "c", "d", "e", "f", "g", "h" };

        public static string MoveToString(Move move)
        {
            string moveString = string.Empty;

            int fromColumn = GetColumn(move.From);
            int fromRow = GetRow(move.From);
            moveString += columnNames[fromColumn - 1] + fromRow;

            int toColumn = GetColumn(move.To);
            int toRow = GetRow(move.To);
            moveString += columnNames[toColumn - 1] + toRow;

            return moveString;
        }

        /// <summary>
        /// Given input such as "e2e4" it runs the appropriate move without the bits.
        /// Use the AddMoveNoBits() function with these moves.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public static Move StringToMove(string movestring)
        {
            Move move = new Move();

            move.From = ChessPieceToIndex(movestring.Substring(0, 2));
            move.To = ChessPieceToIndex(movestring.Substring(2, 2));
            if (movestring.EndsWith("q"))
            {
                move.Bits += 32;
                move.Promote = Definitions.Q;
            }
            else if (movestring.EndsWith("r"))
            {
                move.Bits += 32;
                move.Promote = Definitions.R;
            }
            else if (movestring.EndsWith("b"))
            {
                move.Bits += 32;
                move.Promote = Definitions.B;
            }
            else if (movestring.EndsWith("n"))
            {
                move.Bits += 32;
                move.Promote = Definitions.N;
            }

            return move;
        }
        #endregion

        #region FEN board dictionaries
        /// <summary>
        /// FEN board lookup dictionary
        /// </summary>
        public static Dictionary<string, int> PieceStringToValue = new Dictionary<string, int>
        {
            {"p", Definitions.P},
            {"n", Definitions.N},
            {"b", Definitions.B},
            {"r", Definitions.R},
            {"q", Definitions.Q},
            {"k", Definitions.K},
            
            {"P", Definitions.P},
            {"N", Definitions.N},
            {"B", Definitions.B},
            {"R", Definitions.R},
            {"Q", Definitions.Q},
            {"K", Definitions.K},

            {" ", Definitions.EMPTY}
        };

        public static Dictionary<int, string> PieceValueToString = new Dictionary<int, string>
        {
            {Definitions.P, "p"},
            {Definitions.N, "n"},
            {Definitions.B, "b"},
            {Definitions.R, "r"},
            {Definitions.Q, "q"},
            {Definitions.K, "k"},

            {Definitions.EMPTY, " "}
        };
        #endregion
    }
}
