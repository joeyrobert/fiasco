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
using System.Text.RegularExpressions;
using Fiasco.Engine;

namespace Fiasco.Protocols
{
    class InvalidCommandException : Exception
    {
        public InvalidCommandException() { }
    }

    public class XBoard
    {
        private Board _board = new Board();
        private System.Random _randomNumber = new System.Random();

        public XBoard()
        {
            _board.Book.Load(@"../../../BOOK");
            iNew();
        }

        #region Interface Control
        public void CommandLine()
        {
            string command;
            while (true)
            {
                command = Console.ReadLine();
                try
                {
                    ExecuteCommand(command);
                }
                catch
                {
                    Console.WriteLine("Invalid command");
                }
            }
        }

        private void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "new":
                    iNew();
                    return;
                case "go":
                    iGo();
                    return;
                case "white":
                    iWhite();
                    return;
                case "black":
                    iBlack();
                    return;
                case "quit":
                    Environment.Exit(-1);
                    return;
                case "display":
                    Display.Board(_board);
                    return;
                case "nobook":
                    NoBook();
                    return;
                case "turn":
                    Turn();
                    return;
                case "clear":
                    Clear();
                    return;
                case "exit":
                    Environment.Exit(0);
                    return;
            }

            if (Regex.Match(command, "[a-z][1-8][a-z][1-8][qkbr]?", RegexOptions.IgnoreCase).Success)
            {
                iMove(command);
                return;
            }

            int depth;

            if (command.Split(' ')[0] == "perft" && int.TryParse(command.Split(' ')[1], out depth))
            {
                Perft(depth);
                return;
            }

            if (command.Split(' ')[0] == "divide" && int.TryParse(command.Split(' ')[1], out depth))
            {
                Divide(depth);
                return;
            }

            throw new InvalidCommandException(); 
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

            // check the opening book
            if (!_board.Book.OutOfOpeningBook)
            {
                List<Move> moves = _board.Book.GenerateOpeningBookMoves();
                int moveCount = moves.Count;

                if (moveCount <= 0)
                    _board.Book.OutOfOpeningBook = true;
                else
                    move = moves[_randomNumber.Next(0, moveCount)];

                if (moveCount != 0)
                    _board.AddMoveNoBits(move);
            }

            if(_board.Book.OutOfOpeningBook) // if we're out of the opening book, use the search
            {
                Search.Minimax(_board, 3);
                _board.AddMove(move);
            }
            Console.WriteLine("move " + Constants.MoveToString(move));
        }
        #endregion

        #region Fiasco Commands
        private void NoBook()
        {
            _board.Book.OutOfOpeningBook = true;
        }

        private void Clear()
        {
            Console.Clear();
        }

        private void Turn()
        {
            Console.WriteLine(Display.TurnText(_board.Turn));
        }

        private void Perft(int depth)
        {
            Console.WriteLine(Fiasco.Engine.Perft.Minimax(_board, depth));
        }

        private void Divide(int depth)
        {
            Fiasco.Engine.Perft.Divide(_board, depth);
        }
        #endregion
    }
}
