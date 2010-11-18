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
using Fiasco;
using Fiasco.Engine;
using NUnit.Framework;

namespace Fiasco.Tests
{
    [TestFixture()]
    public class Perft
    {
        Board board;
        // For FEN board which have already failed, presumably at a
        // lower depth. This assumes that this FEN board is dedicated
        // to this process/thread.
        List<string> skipFen = new List<string>();

        [SetUp()]
        public void Init()
        {
            board = new Board();
        }

        [Test, TestCaseSource("PerftReader")]
        public void PerftRunner(string fenBoard, int depth, ulong expected)
        {
            if (skipFen.Contains(fenBoard))
                Assert.Fail("  FEN board failed at a lower level");
            board.SetFen(fenBoard);
            ulong perft = Engine.Perft.Minimax(board, depth);
            if (expected != perft) skipFen.Add(fenBoard);
            Assert.AreEqual(expected, perft);
        }

        public static List<object> PerftReader()
        {
            List<object> results = new List<object>();
            string[] lines = System.IO.File.ReadAllLines("TestSuites/perftsuite.epd");
            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                string fen = parts[0].Trim();
                for (int i = 1; i < parts.Length; i++)
                {
                    string[] parameters = parts[i].Split(' ');
                    int depth = int.Parse(parameters[0].Substring(1));
                    ulong expected = ulong.Parse(parameters[1]);
                    results.Add(new object[] { fen, depth, expected });
                }
            }
            return results;
        }
    }
}
