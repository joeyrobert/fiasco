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

namespace Fiasco
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Fiasco Chess Engine";
            Board board = new Board();
            board.SetFen("8/3K4/2p5/p2b2r1/5k2/8/8/1q6 b - - 1 67");

            Console.WriteLine("---------------------------");

            //Console.WriteLine(Engine.Search.AlphaBeta(board, 5, -Engine.Eval.KVALUE, Engine.Eval.KVALUE));
            //Display.Board(board);
            //Console.WriteLine(Engine.Search.Minimax(board, 5).Score);
            DateTime start = DateTime.Now;
            Engine.Perft.Divide(board, 5);
            DateTime end = DateTime.Now;
            Console.WriteLine((end - start).TotalMilliseconds);
            //Console.WriteLine(Engine.Search.minimax_count);
            //Console.WriteLine(Constants.MoveToString(Engine.Search._principleVariation[5]));
            Console.Read();
        }
    }
}
