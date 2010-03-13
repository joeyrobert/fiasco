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

using System.Collections.Generic;

namespace Fiasco
{
    public class Board
    {
        #region Board Information
        private int[] _pieceArray = new int[120];
        private int[] _colourArray = new int[120];

        private int _turn;

        private int _castling;
        private int _enPassantTarget;
        private int _halfMoveClock;
        private int _fullMoveNumber;

        private Stack<Square> _history = new Stack<Square>();
        private Book _book = new Book();
        #endregion

        #region Constructors and Destructor
        public Board()
        {
            Turn = Constants.WHITE;
            FullMoveNumber = 1;
            HalfMoveClock = 0;
            Castling = 15;
            System.Array.Copy(Constants.BlankArray, this._pieceArray, 120);
            System.Array.Copy(Constants.BlankArray, this._colourArray, 120);
        }

        public Board(string fenBoard)
        {
            SetFen(fenBoard);
        }

		/// <summary>
		/// Full copy constructor
		/// </summary>
		/// <param name="board">Board to be copied</param>
        public Board(Board board)
        {
            System.Array.Copy(board.PieceArray, this.PieceArray, 120);
            System.Array.Copy(board.ColourArray, this.ColourArray, 120);

            this.Turn = board.Turn;
            this.Castling = board.Castling;
            this.EnPassantTarget = board.EnPassantTarget;
            this.HalfMoveClock = board.HalfMoveClock;
            this.FullMoveNumber = board.FullMoveNumber;
            this.Book = board.Book;
            this.History = board.History;
        }
        #endregion

        #region Properties
        public int[] PieceArray
        {
            get
            {
                return _pieceArray;
            }
        }

        public int[] ColourArray
        {
            get
            {
                return _colourArray;
            }
        }

        public int Turn
        {
            get
            {
                return _turn;
            }
            set
            {
                _turn = value;
            }
        }

        /// <summary>
        /// Castling info:
        ///   0001b (1) for K
        ///   0010b (2) for Q
        ///   0100b (4) for k
        ///   1000b (8) for q
        /// </summary>
        public int Castling
        {
            get
            {
                return _castling;
            }
            set
            {
                _castling = value;
            }
        }

        public int EnPassantTarget
        {
            get
            {
                return _enPassantTarget;
            }
            set
            {
                _enPassantTarget = value;
            }
        }

        public int FullMoveNumber
        {
            get
            {
                return _fullMoveNumber;
            }
            set
            {
                _fullMoveNumber = value;
            }
        }

        public int HalfMoveClock
        {
            get
            {
                return _halfMoveClock;
            }
            set
            {
                _halfMoveClock = value;
            }
        }

        public Fiasco.Book Book
        {
            get
            {
                return _book;
            }
            set
            {
                _book = value;
            }
        }

        public Stack<Square> History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
            }
        }
        #endregion

        #region Fen Methods
        public void SetFen(string fenBoard)
        {
            string[] fenBoardSplit = fenBoard.Split(' ');

            // Piece placement info
            string[] rows = fenBoardSplit[0].Split('/');
            char[] pieces = new char[8];
            int rowmarker, columnmarker, index, offset;
            rowmarker = 8;

            foreach (string row in rows)
            {
                columnmarker = 1;
                pieces = row.ToCharArray();
                foreach (char piece in pieces)
                {
                    if (System.Char.IsNumber(piece))
                    {
                        offset = int.Parse(piece.ToString());

                        // made simpler if you make the board initial empty
                        for (int loop = 0; loop < offset; loop++)
                        {
                            index = Constants.GetIndex(rowmarker, columnmarker);
                            _pieceArray[index] = Constants.EMPTY;
                            _colourArray[index] = Constants.EMPTY;
                            columnmarker++;
                        }
                        continue;
                    }
                    else
                    {
                        index = Constants.GetIndex(rowmarker, columnmarker);
                        _pieceArray[index] = Constants.PieceStringToValue[piece.ToString().ToLower()];

                        if (piece.ToString() == piece.ToString().ToUpper())
                            _colourArray[index] = Constants.WHITE;
                        else
                            _colourArray[index] = Constants.BLACK;
                    }
                    columnmarker++;
                }
                rowmarker--;
            }

            // Active Colour
            if (fenBoardSplit[1] == "w")
                _turn = Constants.WHITE;
            else if (fenBoardSplit[1] == "b")
                _turn = Constants.BLACK;

            // Castling Availability
            _castling = 0;
            if (fenBoardSplit[2].Contains("K"))
                _castling += 1;
            if (fenBoardSplit[2].Contains("Q"))
                _castling += 2;
            if (fenBoardSplit[2].Contains("k"))
                _castling += 4;
            if (fenBoardSplit[2].Contains("q"))
                _castling += 8;

            // En Passant Target Square
            if (fenBoardSplit[3] == "-")
                _enPassantTarget = Constants.NOENPASSANT;
            else
                _enPassantTarget = Constants.ChessPieceToIndex(fenBoardSplit[3]);

            // Halfmove Clock
            _halfMoveClock = System.Int32.Parse(fenBoardSplit[4]);

            // Fullmove Number
            _fullMoveNumber = System.Int32.Parse(fenBoardSplit[5]);
        }
        #endregion

        #region IsSquareAttacked
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="Turn">Turn of the current player</param>
        /// <returns></returns>
        public bool IsAttacked(int i, int turn)
        {
            int newMove;

            #region Diagonal (bishop and queen)
            foreach (int delta in Constants.deltaB)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, keep going
                    if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == Constants.EMPTY)
                    {
                    }
                    else if ((_pieceArray[newMove] == Constants.B || _pieceArray[newMove] == Constants.Q) && _colourArray[newMove] == -1 * turn)
                    {
                        return true;
                    }
                    else
                        break; // you must be or off limits or my own/another piece. stop loop.    

                    newMove += delta;
                }
            }
            #endregion

            #region Horizontal and Vertical (rook and queen)
            foreach (int delta in Constants.deltaR)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, keep going
                    if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == Constants.EMPTY)
                    {
                    }
                    else if ((_pieceArray[newMove] == Constants.R || _pieceArray[newMove] == Constants.Q) && _colourArray[newMove] == -1 * turn)
                    {
                        return true;
                    }
                    else
                        break; // you must be or off limits or my own/another piece. stop loop.    

                    newMove += delta;
                }
            }
            #endregion

            #region Knight
            foreach (int delta in Constants.deltaN)
            {
                newMove = i + delta;

                if (_pieceArray[newMove] == Constants.OFF)
                    continue;
                if (_colourArray[newMove] == -1 * turn
                    && _pieceArray[newMove] == Constants.N)
                    return true;
            }
            #endregion

            #region King
            foreach (int delta in Constants.deltaK)
            {
                newMove = i + delta;

                if (_pieceArray[newMove] == Constants.OFF)
                    continue;
                if (_colourArray[newMove] == -1 * turn
                    && _pieceArray[newMove] == Constants.K)
                    return true;
            }
            #endregion

            #region Pawn
            newMove = i + (9 * turn);
            if (_colourArray[newMove] == -1 * turn && _pieceArray[newMove] == Constants.P)
                return true;
            newMove = i + (11 * turn);
            if (_colourArray[newMove] == -1 * turn && _pieceArray[newMove] == Constants.P)
                return true;
            #endregion

            return false;
        }

        public bool IsInCheck(int turn)
        {
            // Find the king (TODO: add a king variable to board class to speed this up)
            int king = 0;
            for (int i = 21; i < 99; i++)
            {
                if (_colourArray[i] == turn && _pieceArray[i] == Constants.K)
                {
                    king = i;
                    break;
                }
            }
            if (king == 0) return false;
            return IsAttacked(king, turn);
        }
        #endregion

        #region Piece Move Generation
        private List<Move> GeneratePawn(int i, int turn)
        {
            List<Move> moves = new List<Move>();
            int newMove;

            // Pawn push
            newMove = i + turn * 10;
            if (_pieceArray[newMove] == Constants.EMPTY)
                moves.Add(new Move(i, newMove, 16)); // 16 = pawn move

            // Double pawn push
            newMove = i + turn * 20;
            int firstRank;

            if (turn == Constants.WHITE)
                firstRank = 2;
            else
                firstRank = 7;

            if (_pieceArray[newMove] == Constants.EMPTY && _pieceArray[(i + turn * 10)] == Constants.EMPTY && Constants.GetRow(i) == firstRank)
                moves.Add(new Move(i, newMove, 24)); // 24 = pawn move + double pawn push

            // Captures
            newMove = i + (9 * turn);
            if (_colourArray[newMove] == -1 * turn)
                moves.Add(new Move(i, newMove, 17)); // 17 = pawn move + capture

            newMove = i + (11 * turn);
            if (_colourArray[newMove] == -1 * turn)
                moves.Add(new Move(i, newMove, 17));

            // En passant captures
            newMove = i + (9 * turn);
            if (_enPassantTarget == newMove)
                moves.Add(new Move(i, newMove, 21)); // 21 = capture + pawn move + en passant capture

            newMove = i + (11 * turn);
            if (_enPassantTarget == newMove)
                moves.Add(new Move(i, newMove, 21));

            // Promotions
            //int lastRank;

            //if (turn == Const.WHITE)
            //    lastRank = 8;
            //else
            //    lastRank = 1;

            return moves;
        }

        private List<Move> GenerateKnight(int i, int turn)
        {
            List<Move> moves = new List<Move>();
            int newMove;

            foreach (int delta in Constants.deltaN)
            {
                newMove = i + delta;
                if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == Constants.EMPTY)
                    moves.Add(new Move(i, newMove, 0));
                else if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == -1 * turn)
                    moves.Add(new Move(i, newMove, 1)); // 1 = capture
            }
            return moves;
        }

        private List<Move> GenerateKing(int i, int turn)
        {
            List<Move> moves = new List<Move>();
            int newMove;

            foreach (int delta in Constants.deltaK)
            {
                newMove = i + delta;
                if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == Constants.EMPTY)
                    moves.Add(new Move(i, newMove, 0));
                else if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == -1 * turn)
                    moves.Add(new Move(i, newMove, 1)); // 1 = capture
            }
            return moves;
        }

        private List<Move> GenerateBishop(int i, int turn)
        {
            List<Move> moves = new List<Move>();
            int newMove;

            foreach (int delta in Constants.deltaB)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, add move
                    if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == Constants.EMPTY)
                        moves.Add(new Move(i, newMove, 0));
                    else if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == -1 * turn)
                    {
                        moves.Add(new Move(i, newMove, 1)); // 1 = capture
                        break; // if the place being moved to is a piece from the other team, stop looping
                    }
                    else
                        break; // you must be or off limits or my own piece. stop loop.            
       
                    newMove += delta;
                }
            }
            return moves;
        }

        private List<Move> GenerateRook(int i, int turn)
        {
            List<Move> moves = new List<Move>();
            int newMove;

            foreach (int delta in Constants.deltaR)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, add move
                    if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == Constants.EMPTY)
                        moves.Add(new Move(i, newMove, 0));
                    else if (_pieceArray[newMove] != Constants.OFF && _colourArray[newMove] == -1 * turn)
                    {
                        moves.Add(new Move(i, newMove, 1)); // 1 = capture
                        break; // if the place being moved to is a piece from the other team, stop looping
                    }
                    else
                        break;                            

                    newMove += delta;
                }
            }
            return moves;
        }

        private List<Move> GenerateCastle(int turn)
        {
            List<Move> moves = new List<Move>();

            // can't castle out of check
            if (IsInCheck(turn)) return moves;

            if (turn == Constants.WHITE)
            {
                if ((Castling & 1) != 0 
                    && _pieceArray[26] == Constants.EMPTY
                    && _pieceArray[27] == Constants.EMPTY
                    && !IsAttacked(26, turn)
                    && !IsAttacked(27, turn))
                    moves.Add(new Move(25, 27, 2));

                if ((Castling & 2) != 0
                    && _pieceArray[22] == Constants.EMPTY
                    && _pieceArray[23] == Constants.EMPTY
                    && _pieceArray[24] == Constants.EMPTY
                    && !IsAttacked(23, turn)
                    && !IsAttacked(24, turn)
                    && !IsAttacked(25, turn))
                    moves.Add(new Move(25, 23, 2));                
            }
            else
            {
                if ((Castling & 4) != 0
                    && _pieceArray[96] == Constants.EMPTY
                    && _pieceArray[97] == Constants.EMPTY
                    && !IsAttacked(96, turn)
                    && !IsAttacked(97, turn))
                    moves.Add(new Move(95, 97, 2));

                if ((Castling & 8) != 0
                    && _pieceArray[92] == Constants.EMPTY
                    && _pieceArray[93] == Constants.EMPTY
                    && _pieceArray[94] == Constants.EMPTY
                    && !IsAttacked(93, turn)
                    && !IsAttacked(94, turn)
                    && !IsAttacked(95, turn))
                    moves.Add(new Move(95, 93, 2)); 
            }
            return moves;
        }
        #endregion

        #region Generate Moves
        public List<Move> GenerateMoves(int turn)
        {
            List<Move> moves = new List<Move>();

            for (int i = 21; i < 99; i++)
            {
                if (_pieceArray[i] == Constants.OFF) continue;

                // if piece is of the same colour and not empty
                if (_colourArray[i] == turn)
                {
                    switch (_pieceArray[i])
                    {
                        case Constants.P:
                            moves.AddRange(GeneratePawn(i, turn));
                            break;
                        case Constants.N:
                            moves.AddRange(GenerateKnight(i, turn));
                            break;
                        case Constants.K:
                            moves.AddRange(GenerateKing(i, turn));
                            break;
                        case Constants.B:
                            moves.AddRange(GenerateBishop(i, turn));
                            break;
                        case Constants.R:
                            moves.AddRange(GenerateRook(i, turn));
                            break;
                        case Constants.Q: // QUEEN == BISHOP + ROOK
                            moves.AddRange(GenerateBishop(i, turn));
                            moves.AddRange(GenerateRook(i, turn));
                            break;
                    }
                }
            }
            moves.AddRange(GenerateCastle(turn));
            return moves;
        }

        public List<Move> GenerateMoves()
        {
            return GenerateMoves(this.Turn);
        }
        #endregion

        #region Move Piece
        /// <summary>
        /// Moves a piece, overwrites any
        /// </summary>
        /// <param name="from">from index</param>
        /// <param name="to">to index</param>
        private void MovePiece(int from, int to)
        {                
            // Move the piece
            _pieceArray[to] = _pieceArray[from];
            _colourArray[to] = _colourArray[from];

            // Delete the original piece
            _pieceArray[from] = Constants.EMPTY;
            _colourArray[from] = Constants.EMPTY;
        }
        #endregion

        #region AddMove and SubtractMove
        public bool AddMoveNoBits(Move move)
        {
            move.Bits = 0;

            if (_colourArray[move.From] == -1 * _turn)
                move.Bits += 1;
            
            if (move.From == 25)
            {
                if (move.To == 27 || move.To == 23)
                    move.Bits += 2;
            }
            else if (move.From == 95)
            {
                if (move.To == 97 || move.To == 93)
                    move.Bits += 2;
            }

            if (_pieceArray[move.From] == Constants.P)
                move.Bits += 16;

            if (_pieceArray[move.From] == Constants.P && move.To == move.From + 20 * _colourArray[move.From])
                move.Bits += 8;

            _book.OpeningBookDepth++;
            _book.OpeningLine += Constants.MoveToString(move);
            return AddMove(move);
        }

        public bool AddMove(Move move)
		{
            // Add the piece that was there to the move stack
            _history.Push(new Square(move, _pieceArray[move.To], _colourArray[move.To], _enPassantTarget, _castling));

            // ORDINARY MOVE
			if (move.Bits == 0)
			{
                MovePiece(move.From, move.To);
                _enPassantTarget = Constants.NOENPASSANT;
            }
            // EN PASSANT
            else if ((move.Bits & 4) != 0)
            {
                MovePiece(move.From, move.To);

                // Delete the en passant target square
                _pieceArray[_enPassantTarget - Turn * 10] = Constants.EMPTY;
                _colourArray[_enPassantTarget - Turn * 10] = Constants.EMPTY;
                _enPassantTarget = Constants.NOENPASSANT;
            }
            // CAPTURE (todo: implement 50 move rule)
            else if ((move.Bits & 1) != 0)
            {
                MovePiece(move.From, move.To);
                _enPassantTarget = Constants.NOENPASSANT;
            }
            // DOUBLE PAWN PUSH (todo: implement 50 move rule)
            else if ((move.Bits & 8) != 0)
            {
                MovePiece(move.From, move.To);
                _enPassantTarget = move.From + (10 * Turn);
            }
            // PAWN PUSH (todo: implement 50 move rule)
            else if ((move.Bits & 16) != 0)
            {
                MovePiece(move.From, move.To);
                _enPassantTarget = Constants.NOENPASSANT;
            }
            // CASTLING MOVE
            else if ((move.Bits & 2) != 0)
            {
                // move the king
                MovePiece(move.From, move.To);

                // figure out which rook to move
                switch (move.To)
                {
                    case 27: // white king side
                        MovePiece(28, 26); // right rook
                        break;
                    case 23: // white queen side
                        MovePiece(21, 24); // left rook
                        break;
                    case 97: // black king side
                        MovePiece(98, 96); // right rook
                        break;
                    case 93:  // black queen size
                        MovePiece(91, 94); // left rook
                        break;
                }

                _enPassantTarget = Constants.NOENPASSANT;
            }
            // PROMOTION (todo: implement 50 move rule)
            else if ((move.Bits & 32) != 0)
            {
                // Move the piece
                _pieceArray[move.To] = move.Promote;
                _colourArray[move.To] = Turn;

                // Delete the original piece
                _pieceArray[move.From] = Constants.EMPTY;
                _colourArray[move.From] = Constants.EMPTY;
                _enPassantTarget = Constants.NOENPASSANT;
            }

            // POST MOVE

            #region Remove castling rights
            if (_pieceArray[move.To] == Constants.K)
            {
                if (Turn == Constants.WHITE)
                {
                    // remove white's ability to castle
                    if ((_castling & 1) != 0) _castling = _castling - 1;
                    if ((_castling & 2) != 0) _castling = _castling - 2;
                }
                else
                {
                    // remove black's ability to castle
                    if ((_castling & 4) != 0) _castling = _castling - 4;
                    if ((_castling & 8) != 0) _castling = _castling - 8;
                }
            }

            if ((move.From == 28 || move.To == 28) && (_castling & 1) != 0)
                _castling -= 1;
            else if ((move.From == 21 || move.To == 21) && (_castling & 2) != 0)
                _castling -= 2;
            else if ((move.From == 98 || move.To == 98) && (_castling & 4) != 0)
                _castling -= 4;
            else if ((move.From == 91 || move.To == 91) && (_castling & 8) != 0)
                _castling -= 8;
            #endregion

            // Increment the full move number after black
            if (Turn == Constants.BLACK)
                _fullMoveNumber++;

            // Switch the active turn
            Turn = -1 * Turn;

            // If the board is in check, subtract the move
            if (IsInCheck(-1 * Turn))
            {
                SubtractMove();
                return false;
            }
			return true;
		}

        public bool SubtractMove()
        {
            if (_history.Count == 0)
                return false;

            Square square = _history.Pop();

            // Switch the active turn
            Turn = -1 * Turn;

            // Decrement the full move number after black
            if (Turn == Constants.BLACK)
                _fullMoveNumber--;

            // Restore castling rights
            _castling = square.Castling;

            // ORDINARY MOVE
            if ((square.Move.Bits & 63) == 0)
            {
                MovePiece(square.Move.To, square.Move.From); // move back
            }
            // EN PASSANT
            else if ((square.Move.Bits & 4) != 0)
            {
                MovePiece(square.Move.To, square.Move.From);

                // Add the piece back to the en passant target square
                _pieceArray[square.EnPassantTarget - Turn * 10] = Constants.P;
                _colourArray[square.EnPassantTarget - Turn * 10] = -1 * Turn;
            }
            // CAPTURE
            else if ((square.Move.Bits & 1) != 0)
            {
                MovePiece(square.Move.To, square.Move.From);
                _pieceArray[square.Move.To] = square.Piece; // restore the old piece
                _colourArray[square.Move.To] = square.Colour;
            }
            // DOUBLE PAWN PUSH
            else if ((square.Move.Bits & 8) != 0)
            {
                MovePiece(square.Move.To, square.Move.From);
            }
            // PAWN PUSH
            else if ((square.Move.Bits & 16) != 0)
            {
                MovePiece(square.Move.To, square.Move.From);
            }
            // CASTLING MOVE
            else if ((square.Move.Bits & 2) != 0)
            {
                // move the king back
                MovePiece(square.Move.To, square.Move.From);

                // figure out which rook to move back
                switch (square.Move.To)
                {
                    case 27: // white king side
                        MovePiece(26, 28); // right rook
                        break;
                    case 23: // white queen side
                        MovePiece(24, 21); // left rook
                        break;
                    case 97: // black king side
                        MovePiece(96, 98); // right rook
                        break;
                    case 93:  // black queen size
                        MovePiece(94, 91); // left rook
                        break;
                }
            }
            // PROMOTION (todo: implement 50 move rule)
            else if ((square.Move.Bits & 4) != 0)
            {
                // Move the piece
                _pieceArray[square.Move.From] = Constants.P;
                _colourArray[square.Move.From] = Turn;

                // Delete the original piece
                _pieceArray[square.Move.To] = Constants.EMPTY;
                _colourArray[square.Move.To] = Constants.EMPTY;
            }

            // Put the old en passant square back
            _enPassantTarget = square.EnPassantTarget;

            return true;
        }
        #endregion
    }
}
