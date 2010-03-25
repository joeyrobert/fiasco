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

        // saves lookup time
        private int _whiteKing = 0, _blackKing = 0;

        private Stack<Square> _history = new Stack<Square>();
        private Book _book = new Book();
        #endregion

        #region Constructors and Destructor
        public Board()
        {
            Turn = Definitions.WHITE;
            FullMoveNumber = 1;
            HalfMoveClock = 0;
            Castling = 15;
            System.Array.Copy(Definitions.BlankArray, this._pieceArray, 120);
            System.Array.Copy(Definitions.BlankArray, this._colourArray, 120);
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
            this.WhiteKing = board.WhiteKing;
            this.BlackKing = board.BlackKing;
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

        public int WhiteKing
        {
            get
            {
                return _whiteKing;
            }
            set
            {
                _whiteKing = value;
            }
        }

        public int BlackKing
        {
            get
            {
                return _blackKing;
            }
            set
            {
                _blackKing = value;
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
                            index = Definitions.GetIndex(rowmarker, columnmarker);
                            _pieceArray[index] = Definitions.EMPTY;
                            _colourArray[index] = Definitions.EMPTY;
                            columnmarker++;
                        }
                        continue;
                    }
                    else
                    {
                        index = Definitions.GetIndex(rowmarker, columnmarker);
                        _pieceArray[index] = Definitions.PieceStringToValue[piece.ToString().ToLower()];

                        if (piece.ToString() == piece.ToString().ToUpper())
                            _colourArray[index] = Definitions.WHITE;
                        else
                            _colourArray[index] = Definitions.BLACK;
                    }
                    columnmarker++;
                }
                rowmarker--;
            }

            // Active Colour
            if (fenBoardSplit[1] == "w")
                _turn = Definitions.WHITE;
            else if (fenBoardSplit[1] == "b")
                _turn = Definitions.BLACK;

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
                _enPassantTarget = Definitions.NOENPASSANT;
            else
                _enPassantTarget = Definitions.ChessPieceToIndex(fenBoardSplit[3]);

            // Halfmove Clock
            _halfMoveClock = System.Int32.Parse(fenBoardSplit[4]);

            // Fullmove Number
            _fullMoveNumber = System.Int32.Parse(fenBoardSplit[5]);

            // Reload the king positions
            ReloadKings();
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
            foreach (int delta in Definitions.deltaB)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, keep going
                    if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == Definitions.EMPTY)
                    {
                    }
                    else if ((_pieceArray[newMove] == Definitions.B || _pieceArray[newMove] == Definitions.Q) && _colourArray[newMove] == -1 * turn)
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
            foreach (int delta in Definitions.deltaR)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, keep going
                    if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == Definitions.EMPTY)
                    {
                    }
                    else if ((_pieceArray[newMove] == Definitions.R || _pieceArray[newMove] == Definitions.Q) && _colourArray[newMove] == -1 * turn)
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
            foreach (int delta in Definitions.deltaN)
            {
                newMove = i + delta;

                if (_pieceArray[newMove] == Definitions.OFF)
                    continue;
                if (_colourArray[newMove] == -1 * turn
                    && _pieceArray[newMove] == Definitions.N)
                    return true;
            }
            #endregion

            #region King
            foreach (int delta in Definitions.deltaK)
            {
                newMove = i + delta;

                if (_pieceArray[newMove] == Definitions.OFF)
                    continue;
                if (_colourArray[newMove] == -1 * turn
                    && _pieceArray[newMove] == Definitions.K)
                    return true;
            }
            #endregion

            #region Pawn
            newMove = i + (9 * turn);
            if (_colourArray[newMove] == -1 * turn && _pieceArray[newMove] == Definitions.P)
                return true;
            newMove = i + (11 * turn);
            if (_colourArray[newMove] == -1 * turn && _pieceArray[newMove] == Definitions.P)
                return true;
            #endregion

            return false;
        }

        /// <summary>
        /// Check if a side is in check
        /// </summary>
        /// <param name="turn">side</param>
        /// <returns>true if that side is in check</returns>
        public bool IsInCheck(int turn)
        {
            if (turn == Definitions.WHITE)
                return IsAttacked(this.WhiteKing, turn);
            else
                return IsAttacked(this.BlackKing, turn);
        }

        /// <summary>
        /// Reloads the king's position variables
        /// </summary>
        public void ReloadKings()
        {
            int whiteKing = 0;
            int blackKing = 0;

            for (int i = 21; i < 99; i++)
            {
                if (_pieceArray[i] == Definitions.K)
                {
                    if (_colourArray[i] == Definitions.WHITE)
                        whiteKing = i;
                    else
                        blackKing = i;
                }

                if (blackKing != 0 && whiteKing != 0)
                    break;
            }

            this.WhiteKing = whiteKing;
            this.BlackKing = blackKing;
        }
        #endregion

        #region Piece Move Generation
        private void GeneratePawn(int i, int turn, ref List<Move> moves)
        {
            int newMove, column;

            // Promotions
            int lastRank;

            if (turn == Definitions.WHITE)
                lastRank = 8;
            else
                lastRank = 1;

            // Pawn push (possible promotion)
            newMove = i + turn * 10;
            column = Definitions.GetRow(newMove);

            if (_pieceArray[newMove] == Definitions.EMPTY)
            {
                if (column == lastRank)
                    GeneratePromotions(i, newMove, 16, moves);
                else
                    moves.Add(new Move(i, newMove, 16)); // 16 = pawn move
            }

            // Double pawn push
            newMove = i + turn * 20;
            int firstRank;

            if (turn == Definitions.WHITE)
                firstRank = 2;
            else
                firstRank = 7;

            if (_pieceArray[newMove] == Definitions.EMPTY && _pieceArray[(i + turn * 10)] == Definitions.EMPTY && Definitions.GetRow(i) == firstRank)
                moves.Add(new Move(i, newMove, 24)); // 24 = pawn move + double pawn push

            // Captures (possible promotion)
            newMove = i + (9 * turn);
            column = Definitions.GetRow(newMove);

            if (_colourArray[newMove] == -1 * turn)
            {
                if (column == lastRank)
                    GeneratePromotions(i, newMove, 17, moves);
                else
                    moves.Add(new Move(i, newMove, 17)); // 17 = pawn move + capture
            }

            newMove = i + (11 * turn);
            column = Definitions.GetRow(newMove);

            if (_colourArray[newMove] == -1 * turn)
            {
                if (column == lastRank)
                    GeneratePromotions(i, newMove, 17, moves);
                else
                    moves.Add(new Move(i, newMove, 17)); // 17 = pawn move + capture
            }

            // En passant captures
            newMove = i + (9 * turn);
            if (_enPassantTarget == newMove)
                moves.Add(new Move(i, newMove, 21)); // 21 = capture + pawn move + en passant capture

            newMove = i + (11 * turn);
            if (_enPassantTarget == newMove)
                moves.Add(new Move(i, newMove, 21));
        }

        private void GeneratePromotions(int from, int to, int bits, List<Move> moves)
        {
            moves.Add(new Move(from, to, bits + 32, Definitions.N));
            moves.Add(new Move(from, to, bits + 32, Definitions.B));
            moves.Add(new Move(from, to, bits + 32, Definitions.R));
            moves.Add(new Move(from, to, bits + 32, Definitions.Q));
        }

        private void GenerateKnight(int i, int turn, ref List<Move> moves)
        {
            int newMove;

            foreach (int delta in Definitions.deltaN)
            {
                newMove = i + delta;
                if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == Definitions.EMPTY)
                    moves.Add(new Move(i, newMove, 0));
                else if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == -1 * turn)
                    moves.Add(new Move(i, newMove, 1)); // 1 = capture
            }
        }

        private void GenerateKing(int i, int turn, ref List<Move> moves)
        {
            int newMove;

            foreach (int delta in Definitions.deltaK)
            {
                newMove = i + delta;
                if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == Definitions.EMPTY)
                    moves.Add(new Move(i, newMove, 0));
                else if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == -1 * turn)
                    moves.Add(new Move(i, newMove, 1)); // 1 = capture
            }
        }

        private void GenerateBishop(int i, int turn, ref List<Move> moves)
        {
            int newMove;

            foreach (int delta in Definitions.deltaB)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, add move
                    if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == Definitions.EMPTY)
                        moves.Add(new Move(i, newMove, 0));
                    else if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == -1 * turn)
                    {
                        moves.Add(new Move(i, newMove, 1)); // 1 = capture
                        break; // if the place being moved to is a piece from the other team, stop looping
                    }
                    else
                        break; // you must be or off limits or my own piece. stop loop.            
       
                    newMove += delta;
                }
            }
        }

        private void GenerateRook(int i, int turn, ref List<Move> moves)
        {
            int newMove;

            foreach (int delta in Definitions.deltaR)
            {
                newMove = i + delta;
                while (true)
                {
                    // if you are not an off limits piece or my own piece, add move
                    if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == Definitions.EMPTY)
                        moves.Add(new Move(i, newMove, 0));
                    else if (_pieceArray[newMove] != Definitions.OFF && _colourArray[newMove] == -1 * turn)
                    {
                        moves.Add(new Move(i, newMove, 1)); // 1 = capture
                        break; // if the place being moved to is a piece from the other team, stop looping
                    }
                    else
                        break;                            

                    newMove += delta;
                }
            }
        }

        private void GenerateCastle(int turn, ref List<Move> moves)
        {
            // can't castle out of check
            if (IsInCheck(turn)) return;

            if (turn == Definitions.WHITE)
            {
                if ((Castling & 1) != 0 
                    && _pieceArray[26] == Definitions.EMPTY
                    && _pieceArray[27] == Definitions.EMPTY
                    && !IsAttacked(26, turn)
                    && !IsAttacked(27, turn))
                    moves.Add(new Move(25, 27, 2));

                if ((Castling & 2) != 0
                    && _pieceArray[22] == Definitions.EMPTY
                    && _pieceArray[23] == Definitions.EMPTY
                    && _pieceArray[24] == Definitions.EMPTY
                    && !IsAttacked(23, turn)
                    && !IsAttacked(24, turn)
                    && !IsAttacked(25, turn))
                    moves.Add(new Move(25, 23, 2));                
            }
            else
            {
                if ((Castling & 4) != 0
                    && _pieceArray[96] == Definitions.EMPTY
                    && _pieceArray[97] == Definitions.EMPTY
                    && !IsAttacked(96, turn)
                    && !IsAttacked(97, turn))
                    moves.Add(new Move(95, 97, 2));

                if ((Castling & 8) != 0
                    && _pieceArray[92] == Definitions.EMPTY
                    && _pieceArray[93] == Definitions.EMPTY
                    && _pieceArray[94] == Definitions.EMPTY
                    && !IsAttacked(93, turn)
                    && !IsAttacked(94, turn)
                    && !IsAttacked(95, turn))
                    moves.Add(new Move(95, 93, 2)); 
            }
        }
        #endregion

        #region Generate Moves
        public List<Move> GenerateMoves(int turn)
        {
            List<Move> moves = new List<Move>();

            for (int i = 21; i < 99; i++)
            {
                if (_pieceArray[i] == Definitions.EMPTY || _pieceArray[i] == Definitions.OFF) continue;

                // if piece is of the same colour and not empty
                if (_colourArray[i] == turn)
                {
                    switch (_pieceArray[i])
                    {
                        case Definitions.P:
                            GeneratePawn(i, turn, ref moves);
                            break;
                        case Definitions.N:
                            GenerateKnight(i, turn, ref moves);
                            break;
                        case Definitions.K:
                            GenerateKing(i, turn, ref moves);
                            break;
                        case Definitions.B:
                            GenerateBishop(i, turn, ref moves);
                            break;
                        case Definitions.R:
                            GenerateRook(i, turn, ref moves);
                            break;
                        case Definitions.Q: // QUEEN == BISHOP + ROOK
                            GenerateBishop(i, turn, ref moves);
                            GenerateRook(i, turn, ref moves);
                            break;
                    }
                }
            }
            GenerateCastle(turn, ref moves);
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
            _pieceArray[from] = Definitions.EMPTY;
            _colourArray[from] = Definitions.EMPTY;
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

            if (_pieceArray[move.From] == Definitions.P)
                move.Bits += 16;

            if (_pieceArray[move.From] == Definitions.P && move.To == move.From + 20 * _colourArray[move.From])
                move.Bits += 8;

            _book.OpeningBookDepth++;
            _book.OpeningLine += Definitions.MoveToString(move);
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
                _enPassantTarget = Definitions.NOENPASSANT;
            }
            // EN PASSANT
            else if ((move.Bits & 4) != 0)
            {
                MovePiece(move.From, move.To);

                // Delete the en passant target square
                _pieceArray[_enPassantTarget - Turn * 10] = Definitions.EMPTY;
                _colourArray[_enPassantTarget - Turn * 10] = Definitions.EMPTY;
                _enPassantTarget = Definitions.NOENPASSANT;
            }
            // CAPTURE (todo: implement 50 move rule)
            else if ((move.Bits & 1) != 0)
            {
                if (_pieceArray[move.To] == Definitions.K)
                {
                    switch (_colourArray[move.To])
                    {
                        case Definitions.WHITE:
                            _whiteKing = Definitions.EMPTY;
                            break;
                        case Definitions.BLACK:
                            _blackKing = Definitions.EMPTY;
                            break;
                    }
                }

                MovePiece(move.From, move.To);
                _enPassantTarget = Definitions.NOENPASSANT;
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
                _enPassantTarget = Definitions.NOENPASSANT;
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

                _enPassantTarget = Definitions.NOENPASSANT;
            }

            // PROMOTION (todo: implement 50 move rule)
            if ((move.Bits & 32) != 0)
            {
                // Everything else should be taken care of by this point.
                // Just overwrite the underlying piece.
                _pieceArray[move.To] = move.Promote;
            }

            // POST MOVE

            #region Remove castling rights and reset king
            if (_pieceArray[move.To] == Definitions.K)
            {
                if (Turn == Definitions.WHITE)
                {
                    // remove white's ability to castle
                    if ((_castling & 1) != 0) _castling = _castling - 1;
                    if ((_castling & 2) != 0) _castling = _castling - 2;
                    this.WhiteKing = move.To;
                }
                else
                {
                    // remove black's ability to castle
                    if ((_castling & 4) != 0) _castling = _castling - 4;
                    if ((_castling & 8) != 0) _castling = _castling - 8;
                    this.BlackKing = move.To;
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
            if (Turn == Definitions.BLACK)
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
            if (Turn == Definitions.BLACK)
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
                _pieceArray[square.EnPassantTarget - Turn * 10] = Definitions.P;
                _colourArray[square.EnPassantTarget - Turn * 10] = -1 * Turn;
            }
            // CAPTURE
            else if ((square.Move.Bits & 1) != 0)
            {
                MovePiece(square.Move.To, square.Move.From);
                _pieceArray[square.Move.To] = square.Piece; // restore the old piece
                _colourArray[square.Move.To] = square.Colour;

                if (_pieceArray[square.Move.To] == Definitions.K)
                {
                    switch (_colourArray[square.Move.To])
                    {
                        case Definitions.WHITE:
                            _whiteKing = square.Move.To;
                            break;
                        case Definitions.BLACK:
                            _blackKing = square.Move.To;
                            break;
                    }
                }
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
            if ((square.Move.Bits & 32) != 0)
            {
                // Overwrite with a pawn
                _pieceArray[square.Move.From] = Definitions.P;
            }

            // Put the old en passant square back
            _enPassantTarget = square.EnPassantTarget;

            // Since the king has been moved back, check the 'from' square
            if (_pieceArray[square.Move.From] == Definitions.K)
            {
                if (_colourArray[square.Move.From] == Definitions.WHITE)
                    this.WhiteKing = square.Move.From;
                else
                    this.BlackKing = square.Move.From;
            }

            return true;
        }
        #endregion
    }
}
