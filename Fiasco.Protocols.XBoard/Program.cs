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

namespace Fiasco.Protocols
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Fiasco Chess Engine - XBoard";
            Board board = new Board();
            board.SetFen("rnbqkbnr/pppppppp/8/8/8/1q6/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            XBoard xboard = new Fiasco.Protocols.XBoard();
            xboard.CommandLine();

            Console.Read();
        }
    }
}
