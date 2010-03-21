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
        public static Dictionary<int, Move> _principleVariation = new Dictionary<int, Move>();
        public static long _nodesSearched = 0;

        public static Pair Minimax(Board board, int depth)
        {
            Move tmpMove = new Move();

            if (depth == 0)
                return new Pair(Eval.Board(board), tmpMove);

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
                else if (value == best.Score && Constants.ALLOWRANDOM)
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

        public static int AlphaBeta(Board board, int depth, int alpha, int beta)
        {
            if (depth == 0)
            {
                _nodesSearched++;
                return Eval.Board(board);
            }

            List<Move> moveList = board.GenerateMoves();

            foreach (Move move in moveList)
            {
                if (!board.AddMove(move)) continue;
                int result = -AlphaBeta(board, depth - 1, -beta, -alpha);
                board.SubtractMove();

                if (result > alpha)
                {
                    alpha = result;
                    if (_principleVariation.ContainsKey(depth))
                        _principleVariation[depth] = move;
                    else
                        _principleVariation.Add(depth, move);
                }

                if (beta <= alpha)
                    break;
            }

            return alpha;
        }

        public static Move Think(Board board, int depth)
        {
            _principleVariation = new Dictionary<int, Move>();
            AlphaBeta(board, depth, -Eval.KVALUE, Eval.KVALUE);
            return _principleVariation[depth];
        }

        /// <summary>
        /// Search method with iterative deeping.
        /// TODO: Move ordering with iterative deepening.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="totalMilliseconds">number of milliseconds to move in (not guaranteed)</param>
        /// <returns>the best move</returns>
        public static Move ThinkIteratively(Board board, int totalMilliseconds, bool xboard)
        {
            DateTime start = DateTime.Now;
            TimeSpan elapsedTime;
            int i, score;
            _nodesSearched = 0;

            Console.WriteLine("Total milliseconds " + totalMilliseconds);

            for (i = 1; ; i++)
            {
                // Think
                _principleVariation = new Dictionary<int, Move>();
                score = AlphaBeta(board, i, -Eval.KVALUE, Eval.KVALUE);
                
                elapsedTime = (DateTime.Now - start);

                if (xboard)
                {
                    string moveString = "";
                    for(int j = 1; j <= i; j++)
                        moveString += Constants.MoveToString(_principleVariation[j]) + " ";

                    Console.WriteLine(i + " " + score + " " + (int)(elapsedTime.TotalMilliseconds / 10) + " " + _nodesSearched + " " + moveString);
                }

                // 0.04 is approx. number_of_moves(depth n) / number_of_moves(depth n + 1)
                if (elapsedTime.TotalMilliseconds > 0.04 * totalMilliseconds)
                    break;
            }

            return _principleVariation[i];
        }
    }
}