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
using System.Collections.Generic;

namespace Fiasco
{
    /// <summary>
    /// Book employs a tree structure composed of BookNodes. Each
    /// BookNode contains a list of BookNodes below it. This allows
    /// for a quick traversal of the opening book.
    /// </summary>
    public class Book
    {
        private string[] _lines;
        private bool _outOfOpeningBook = false;
        private int _openingBookDepth = 0;
        private string _openingLine = string.Empty;

        #region Properties
        public bool OutOfOpeningBook
        {
            get
            {
                return _outOfOpeningBook;
            }
            set
            {
                _outOfOpeningBook = value;
            }
        }

        public int OpeningBookDepth
        {
            get
            {
                return _openingBookDepth;
            }
            set
            {
                _openingBookDepth = value;
            }
        }

        public string OpeningLine
        {
            get
            {
                return _openingLine;
            }
            set
            {
                _openingLine = value;
            }
        }
        #endregion

        /// <summary>
        /// Puts file into a tree structure in memory
        /// </summary>
        /// <param name="file"></param>
        public void Load(string file)
        {
            _lines = System.IO.File.ReadAllLines(file);
        }

        public List<Move> GenerateOpeningBookMoves()
        {
            List<Move> moves = new List<Move>();
            List<string> alreadyAdded = new List<string>();

            foreach (string line in _lines)
            {
                // skip if the line doesn't contain the opening or isn't long enough
                if (!line.StartsWith(OpeningLine) || line.Length < OpeningLine.Length + 4)
                    continue;
                string moveString = line.Substring(4 * OpeningBookDepth, 4);
                if (!alreadyAdded.Contains(moveString))
                {
                    alreadyAdded.Add(moveString);
                    moves.Add(new Move(Constants.StringToMove(moveString)));
                }
            }

            return moves;
        }
    }
}
