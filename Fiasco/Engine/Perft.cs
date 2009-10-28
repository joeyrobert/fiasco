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

namespace Fiasco.Engine
{
    public class Perft
    {
        static public ulong Minimax(Board board, int depth)
        {
            ulong nodes = 0;
            if (depth == 0) return 1;
            List<Move> moves = board.GenerateMoves(board.Turn);

            foreach (Move move in moves)
            {
                if (board.AddMove(move))
                {
                    nodes += Minimax(board, depth - 1);
                    board.SubtractMove();
                }
            }

            return nodes;
        }

        static public void Divide(Board board, int depth)
        {
            Console.WriteLine("Divide() for depth " + depth);
            Console.WriteLine("--------------------");
            List<Move> moves = board.GenerateMoves(board.Turn);
            ulong total = 0;
            foreach (Move move in moves)
            {
                if (board.AddMove(move))
                {
                    ulong current = Minimax(board, depth - 1);
                    total += current;
                    Console.WriteLine(Constants.MoveToString(move) + "\t" + current);
                    board.SubtractMove();
                }
            }
            Console.WriteLine("\nTotal: " + total);
        }
    }
}
