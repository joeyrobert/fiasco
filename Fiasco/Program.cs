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
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Fiasco Chess Engine";
            Board board = new Board();
            board.SetFen("1r1qk2r/5ppp/Q2p4/6R1/4P1bq/2N5/P1P5/4K1N1 b k - 0 23");

            //for (int i = 0; i < 3; i++)
            //{
            //    DateTime start = DateTime.Now;
            //    Engine.Perft.Divide(board, 6);
            //    DateTime end = DateTime.Now;
            //    Console.WriteLine((end - start).TotalSeconds + " s");
            //}

            for (int i = 0; i < 4; i++)
            {
                Move move = Engine.Search.ThinkIteratively(board, (int)(500000 * 0.02), true);
                board.AddMove(move);
            }

            //Console.Read();
        }
    }
}
