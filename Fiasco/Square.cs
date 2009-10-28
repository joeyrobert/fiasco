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
    /// Used to represent capture history. For SubtractMove()
    /// </summary>
    public struct Square
    {
        public int Piece;
        public int Colour;
        public int EnPassantTarget; // en passant needs to be restored
        public int Castling;
        public Move Move;

        public Square(Move move, int piece, int colour, int enpassanttarget, int castling)
        {
            EnPassantTarget = enpassanttarget;
            Piece = piece;
            Colour = colour;
            Move = move;
            Castling = castling;
        }
    }
}
