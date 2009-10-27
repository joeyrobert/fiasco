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
using System.Text.RegularExpressions;
using Fiasco.Engine;

namespace Fiasco.Protocols
{
    public class XBoard
    {
        private Board _board = new Board();
        private System.Random _randomNumber = new System.Random();

        public XBoard()
        {
            _board.Book.Load(@"..\..\..\BOOK");
            iNew();
        }

        #region Interface Control
        public void CommandLine()
        {
            string command = "";
            while (true)
            {
                command = Console.ReadLine();
                if (command == "quit") break;
                LookUpCommand(command);
            }
        }

        private void LookUpCommand(string command)
        {
            if (command == "new")
            {
                iNew();
            }
            else if (command == "go")
            {
                iGo();
            }
            else if (command == "white")
            {
                iWhite();
            }
            else if (command == "black")
            {
                iBlack();
            }
            else if (command == "display")
            {
                Display.Board(_board);
            }
            else if (Regex.Match(command, "[a-z][1-8][a-z][1-8][qkbr]?", RegexOptions.IgnoreCase).Success)
            {
                iMove(command);
            }
        }
        #endregion

        #region XBoard Input Commands
        private void iNew()
        {
            _board.SetFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            _board.Castling = 15;
            _board.EnPassantTarget = Constants.NOENPASSANT;
        }

        private void iGo()
        {
            oMove();
        }

        private void iMove(string command)
        {
            Move move = new Move();

            move = Constants.StringToMove(command);

            _board.AddMoveNoBits(move);
            oMove();
        }

        private void iBlack()
        {
            _board.Turn = Constants.BLACK;
        }

        private void iWhite()
        {
            _board.Turn = Constants.WHITE;
        }
        #endregion

        #region XBoard Output Commands
        private void oMove()
        {
            Move move = new Move();
            int moveCount;
            // check the opening book
            if (!_board.Book.OutOfOpeningBook)
            {
                List<Move> moves = _board.Book.GenerateOpeningBookMoves();
                moveCount = moves.Count;
                if (moveCount == 0)
                    _board.Book.OutOfOpeningBook = true;
                else if (moveCount <= 1)
                    move = moves[0];
                else
                    move = moves[_randomNumber.Next(0, moveCount - 1)];

                if (moveCount != 0)
                    _board.AddMoveNoBits(move);
            }

            if(_board.Book.OutOfOpeningBook) // if we're out of the opening book, use the search
            {
                Search.AlphaBeta(_board, 4, -1 * Eval.KVALUE, Eval.KVALUE, ref move);
                // the current move will always be at the top of the stack
                _board.AddMove(move);
            }

            Console.WriteLine("move " + Constants.MoveToString(move));
        }
        #endregion
    }
}
