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
using Fiasco;
using Fiasco.Engine;
using NUnit.Framework;

namespace Fiasco.Tests
{
    [TestFixture()]
    public class Perft
    {
        Board board;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        [Test()]
        public void Initial_Perft4()
        {
            ulong perft = Engine.Perft.Minimax(board, 4);
            Assert.AreEqual(197281, perft);
        }

        [Test()]
        public void Initial_Perft5()
        {
            ulong perft = Engine.Perft.Minimax(board, 5);
            Assert.AreEqual(4865609, perft);
        }

        [Test()]
        public void Position2_Perft3()
        {
            board.SetFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            ulong perft = Engine.Perft.Minimax(board, 3);
            Assert.AreEqual(97862, perft);
        }

        [Test()]
        public void Position2_Perft4()
        {
            board.SetFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
            ulong perft = Engine.Perft.Minimax(board, 4);
            Assert.AreEqual(4085603, perft);
        }

        [Test()]
        public void Position3_Perft4()
        {
            board.SetFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
            ulong perft = Engine.Perft.Minimax(board, 4);
            Assert.AreEqual(43238, perft);
        }

        [Test()]
        public void Position3_Perft5()
        {
            board.SetFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
            ulong perft = Engine.Perft.Minimax(board, 5);
            Assert.AreEqual(674624, perft);
        }
    }
}
