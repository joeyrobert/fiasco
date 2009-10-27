/* Copyright (c) 2009 Joseph Robert. All rights reserved.
 *
 * This file is part of Fiasco.
 * 
 * Fiasco is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Fiasco is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Fiasco.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Fiasco.Engine;

namespace Fiasco
{
    static public class Display
    {
        /// <summary>
        /// Some colour acrobatics
        /// </summary>
        /// <param name="board"></param>
        static public void Board(Board board)
        {
            int index;
            ConsoleColor tempColour;

            Console.BackgroundColor = ConsoleColor.Gray;
            for (int i = 8; i >= 1; i--)
            {
                tempColour = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(i + " ");
                Console.BackgroundColor = tempColour;

                for (int j = 1; j <= 8; j++)
                {
                    index = Constants.GetIndex(i, j);
                    if (board.ColourArray[index] == Constants.WHITE)
                        Console.ForegroundColor = ConsoleColor.White;
                    else if (board.ColourArray[index] == Constants.BLACK)
                        Console.ForegroundColor = ConsoleColor.Black;

                    Console.Write(" " + Constants.PieceValueToString[board.PieceArray[index]]);

                    if (Console.BackgroundColor == ConsoleColor.Gray)
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    else
                        Console.BackgroundColor = ConsoleColor.Gray;
                }

                if (Console.BackgroundColor == ConsoleColor.Gray)
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                else
                    Console.BackgroundColor = ConsoleColor.Gray;
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.WriteLine("   A B C D E F G H\n");
            Console.WriteLine("Castling value:       " + board.Castling);
            Console.WriteLine("En passant target:    " + board.EnPassantTarget);
            Console.WriteLine("Full move number:     " + board.FullMoveNumber);
            Console.WriteLine("Half move clock:      " + board.HalfMoveClock);
            Console.WriteLine("Turn to move:         " + board.Turn);
            Console.WriteLine("Current board value:  " + Eval.PieceValues(board));
        }
    }
}
