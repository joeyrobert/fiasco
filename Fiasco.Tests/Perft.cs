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

        #region Initial Board
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
        public void Initial_Perft6()
        {
            ulong perft = Engine.Perft.Minimax(board, 6);
            Assert.AreEqual(119060324, perft);
        }
        #endregion

        #region Position 2
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
        #endregion

        #region Position 3
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
        #endregion

        #region Position 4
        [Test()]
        public void Position4_Perft5()
        {
            board.SetFen("rnbqkb1r/ppppp1pp/7n/4Pp2/8/8/PPPP1PPP/RNBQKBNR w KQkq f6 0 3");
            ulong perft = Engine.Perft.Minimax(board, 5);
            Assert.AreEqual(11139762, perft);
        }
        #endregion

        #region Check cases
        [Test()]
        public void RemoveIfInCheck()
        {
            board.SetFen("rnb1kbnr/3p1Qpp/1q6/1B2N3/3PPB2/2P5/PP3PPP/R4RK1 b k - 0 16");
            ulong perft = Engine.Perft.Minimax(board, 1);
            Assert.AreEqual(1, perft);
        }
        #endregion
    }
}
