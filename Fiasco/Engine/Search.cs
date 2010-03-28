﻿/* Copyright (c) 2009 Joseph Robert. All rights reserved.
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
        private static Dictionary<int, Move> _principleVariation = new Dictionary<int, Move>();
        private static int _ply = 0;        
        private static long _nodesSearched = 0;

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
                else if (value == best.Score && Definitions.ALLOWRANDOM)
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

        public static int AlphaBeta(Board board, int depth, int alpha, int beta, Move? bestMove)
        {
            // Check for end of search or terminal node
            if (depth == 0 || board.WhiteKing == Definitions.EMPTY || board.BlackKing == Definitions.EMPTY)
            {
                _nodesSearched++;
                return Quiescence(board, alpha, beta);
            }

            List<Move> moveList = board.GenerateMoves();

            // Put captures at the top of the list
            int count = moveList.Count, beginningOfList = 0;
            Move tempMove;
            for (int i = 0; i < count; i++)
            {
                if ((moveList[i].Bits & 1) != 0)
                {
                    tempMove = moveList[i];
                    moveList[i] = moveList[beginningOfList];
                    moveList[beginningOfList] = tempMove;
                    beginningOfList++;
                }
            }

            // Put the previous best move at the top of the list
            if (bestMove.HasValue && moveList.Contains(bestMove.Value))
            {
                moveList.Remove(bestMove.Value);
                moveList.Insert(0, bestMove.Value);
            }

            foreach (Move move in moveList)
            {
                if (!board.AddMove(move)) continue;
                _ply++;
                int result = -AlphaBeta(board, depth - 1, -beta, -alpha, null);
                _ply--;
                board.SubtractMove();

                if (result > alpha)
                {
                    alpha = result;
                    if (_principleVariation.ContainsKey(_ply))
                        _principleVariation[_ply] = move;
                    else
                        _principleVariation.Add(_ply, move);
                }

                if (beta <= alpha)
                    break;
            }

            return alpha;
        }

        public static Move Think(Board board, int depth)
        {
            _principleVariation = new Dictionary<int, Move>();
            AlphaBeta(board, depth, -Eval.KVALUE, Eval.KVALUE, null);
            return _principleVariation[0];
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
            Move best = new Move();
            TimeSpan elapsedTime;
            int i, score;
            _nodesSearched = 0;

            for (i = 1; ; i++)
            {
                // Think
                _principleVariation = new Dictionary<int, Move>();
                if (i == 1)
                    score = AlphaBeta(board, i, -Eval.KVALUE, Eval.KVALUE, null);
                else
                    score = AlphaBeta(board, i, -Eval.KVALUE, Eval.KVALUE, best);
                
                elapsedTime = (DateTime.Now - start);

                if (xboard)
                {
                    string moveString = "";
                    for (int j = 0; j < i - 1; j++)
                    {
                        if (!_principleVariation.ContainsKey(j))
                            break;

                        moveString += Definitions.MoveToString(_principleVariation[j]) + " ";
                    }
                    
                    Console.WriteLine(i + " " + score + " " + (int)(elapsedTime.TotalMilliseconds / 10) + " " + _nodesSearched + " " + moveString);
                }

                if (score >= Engine.Eval.KVALUE || score <= -Engine.Eval.KVALUE)
                    break;

                best = _principleVariation[0];

                // 0.04 is approx. number_of_moves(depth n) / number_of_moves(depth n + 1)
                if (elapsedTime.TotalMilliseconds > 0.2 * totalMilliseconds)
                    break;
            }

            return best;
        }

        private static int Quiescence(Board board, int alpha, int beta)
        {
            int stand_pat = Engine.Eval.Board(board);

            if (stand_pat >= beta)
                return beta;
            if (alpha < stand_pat)
                alpha = stand_pat;

            List<Move> captures = board.GenerateCaptures();
            foreach (Move move in captures)
            {
                if (!board.AddMove(move)) continue;
                int score = -Quiescence(board, -beta, -alpha);
                board.SubtractMove();

                if (score >= beta)
                    return beta;
                if (score > alpha)
                    alpha = score;
            }

            return alpha;

        }
    }
}