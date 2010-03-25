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

namespace Fiasco
{
    /// <summary>
    /// Move structure (taken from TSCP)
    /// 1	capture                     (000001) - 50 move rule
    /// 2	castle                      (000010) - castling, tells Board to move rook too
    /// 4	en passant capture          (000100) - tells Board which piece to remove
    /// 8	pushing a pawn 2 squares    (001000) - en passant target square
    /// 16	pawn move                   (010000) - 50 move rule
    /// 32	promote                     (100000) - promotion
    /// </summary>
    public struct Move
    {
        public int To;
        public int From;
        public int Promote; // what piece to promote to, Const.Q, Const.N, Const.B, Const.R
        public int Bits;

        #region Constructors
        public Move(Move move)
        {
            this.To = move.To;
            this.From = move.From;
            this.Promote = move.Promote;
            this.Bits = move.Bits;
        }

        public Move(int iFrom, int iTo)
        {
            To = iTo;
            From = iFrom;
            Promote = 0;
            Bits = 0;
        }

        public Move(int iFrom, int iTo, int iBits)
        {
            To = iTo;
            From = iFrom;
            Promote = 0;
            Bits = iBits;
        }

        public Move(int iFrom, int iTo, int iBits, int iPromote)
        {
            To = iTo;
            From = iFrom;
            Bits = iBits;
            Promote = iPromote;
        }
        #endregion
    }
}
