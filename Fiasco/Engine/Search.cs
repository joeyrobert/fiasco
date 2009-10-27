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

using System.Collections.Generic;

namespace Fiasco.Engine
{
    public class Search
    {
        public static int AlphaBeta(Board board, int depth, int alpha, int beta, ref Move bestMove)
        {
            if (depth == 0)
                return Eval.PieceValues(board);

            List<Move> moveList = board.GenerateMoves();
            Move tmpMove = new Move();

            foreach (Move move in moveList)
            {
                board.AddMove(move);
                int val = -AlphaBeta(board, depth - 1, -beta, -alpha, ref tmpMove);
                board.SubtractMove();

                if (val > alpha)
                {
                    if (val >= beta)
                        return beta;
                    alpha = val;
                    bestMove = move;
                }
            }
            return alpha;
        }
    }
}
