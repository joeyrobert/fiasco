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
        public const bool NULLMOVEALLOWED = true;
        #endregion

        #region Default Board Setups

        public static int[] BlankArray = new int[] { 
                OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, 
                OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, 
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, OFF,
                OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, 
                OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF, OFF,             
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

        #region Attack boards

        public static ulong[] BishopAttackArray = new ulong[] {
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
            EMPTY, 9241421688590303744, 36099303471056128, 141012904249856, 550848566272, 6480472064, 1108177604608, 283691315142656, 72624976668147712, EMPTY,
            EMPTY, 4620710844295151618, 9241421688590368773, 36099303487963146, 141017232965652, 1659000848424, 283693466779728, 72624976676520096, 145249953336262720, EMPTY,
            EMPTY, 2310355422147510788, 4620710844311799048, 9241421692918565393, 36100411639206946, 424704217196612, 72625527495610504, 145249955479592976, 290499906664153120, EMPTY,
            EMPTY, 1155177711057110024, 2310355426409252880, 4620711952330133792, 9241705379636978241, 108724279602332802, 145390965166737412, 290500455356698632, 580999811184992272, EMPTY,
            EMPTY, 577588851267340304, 1155178802063085600, 2310639079102947392, 4693335752243822976, 9386671504487645697, 326598935265674242, 581140276476643332, 1161999073681608712, EMPTY,
            EMPTY, 288793334762704928, 577868148797087808, 1227793891648880768, 2455587783297826816, 4911175566595588352, 9822351133174399489, 1197958188344280066, 2323857683139004420, EMPTY,
            EMPTY, 144117404414255168, 360293502378066048, 720587009051099136, 1441174018118909952, 2882348036221108224, 5764696068147249408, 11529391036782871041, 4611756524879479810, EMPTY,
            EMPTY, 567382630219904, 1416240237150208, 2833579985862656, 5667164249915392, 11334324221640704, 22667548931719168, 45053622886727936, 18049651735527937, EMPTY,
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
        };

        public static ulong[] RookAttackArray = new ulong[] { 
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
            EMPTY, 72340172838076926, 144680345676153597, 289360691352306939, 578721382704613623, 1157442765409226991, 2314885530818453727, 4629771061636907199, 9259542123273814143, EMPTY,
            EMPTY, 72340172838141441, 144680345676217602, 289360691352369924, 578721382704674568, 1157442765409283856, 2314885530818502432, 4629771061636939584, 9259542123273813888, EMPTY,
            EMPTY, 72340172854657281, 144680345692602882, 289360691368494084, 578721382720276488, 1157442765423841296, 2314885530830970912, 4629771061645230144, 9259542123273748608, EMPTY,
            EMPTY, 72340177082712321, 144680349887234562, 289360695496279044, 578721386714368008, 1157442769150545936, 2314885534022901792, 4629771063767613504, 9259542123257036928, EMPTY,
            EMPTY, 72341259464802561, 144681423712944642, 289361752209228804, 578722409201797128, 1157443723186933776, 2314886351157207072, 4629771607097753664, 9259542118978846848, EMPTY,
            EMPTY, 72618349279904001, 144956323094725122, 289632270724367364, 578984165983651848, 1157687956502220816, 2315095537539358752, 4629910699613634624, 9259541023762186368, EMPTY,
            EMPTY, 143553341945872641, 215330564830528002, 358885010599838724, 645993902138460168, 1220211685215703056, 2368647251370188832, 4665518383679160384, 9259260648297103488, EMPTY,
            EMPTY, 18302911464433844481, 18231136449196065282, 18087586418720506884, 17800486357769390088, 17226286235867156496, 16077885992062689312, 13781085504453754944, 9187484529235886208, EMPTY,
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
            EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY, EMPTY,
        };

        private static ulong P2(int power)
        {
            ulong start = 1;
            start <<= power;
            return start;
        }

        private static int PiecePower(int index)
        {
            int column = Definitions.GetColumn(index);
            int row = Definitions.GetRow(index);

            return (row - 1) * 8 + column - 1;
        }

        private static void GenerateAttackBoard(int direction)
        {
            for (int row = 1; row <= 8; row++)
            {
                for (int column = 1; column <= 8; column++)
                {
                    Board board = new Board("8/8/8/8/8/8/8/8 w - - 0 1");
                    board.PieceArray[Definitions.GetIndex(row, column)] = direction;
                    board.ColourArray[Definitions.GetIndex(row, column)] = Definitions.WHITE;

                    List<Move> moves = board.GenerateMoves();

                    ulong resultant = 0;

                    foreach (Move move in moves)
                    {
                        int piece = PiecePower(move.To);
                        resultant += P2(piece);
                    }

                    Console.Write(resultant + ", ");
                    if (column % 4 == 0)
                        Console.WriteLine("");
                }

            }
        }

        #endregion
    }
}
