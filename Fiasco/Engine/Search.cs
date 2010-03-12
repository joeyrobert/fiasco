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
    public class Search
    {
        public static Pair Minimax(Board board, int depth)
        {
            Move tmpMove = new Move();

            if (depth == 0)
                return new Pair(Eval.PieceValues(board), tmpMove);

            List<Move> moveList = board.GenerateMoves();
            Pair best = new Pair();

            foreach (Move move in moveList)
            {
                if (!board.AddMove(move)) continue;

                Pair children = Minimax(board, depth - 1);
                int value = -1 * children.Score;

                board.SubtractMove();

                if(!best.Set || value > best.Score) {
                    best.Move = move;
                    best.Score = value;
                    best.Set = true;
                }
                // if two moves are equivalent, randomly choose one.
                else if (value == best.Score)
                {
                    Random random = new Random((int)(DateTime.Now.Ticks % 1000000));
                    int choice = random.Next(2);

                    if (choice == 1)
                    {
                        best.Move = move;
                        best.Score = value;
                    }
                }
            }

            return best;
        }
    }
}
