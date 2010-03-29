using System;
using System.Collections.Generic;

namespace Fiasco.Transposition
{
    public class HashValues
    {
        #region Random number definitions

        /// <summary>
        /// Intentionally not seeded. Hash table can contain 2^N piece,
        /// where N is the number of bits in the random number.
        /// </summary>
        Random _random = new Random();

        /// <summary>
        /// WHITE: 0 - 5, BLACK: 6 - 11
        /// Lookup first by position, then by type.
        /// Easier to generate 120 upfront.
        /// </summary>
        int[,] _pieceTable = new int[120, 12];

        /// <summary>
        /// KQkq (white is in capitals)
        /// </summary>
        int[] _castlingRights = new int[4];

        /// <summary>
        /// Side to move. XORed in if black is playing
        /// </summary>
        int _ifBlackIsPlaying;

        /// <summary>
        /// One for each en passant column
        /// </summary>
        int[] _enPassantFile = new int[8];

        #endregion

        #region Constructor and seeding

        public HashValues()
        {
            SeedTable();
        }

        private void SeedTable()
        {
            _random = new Random();

            // Side to move
            _ifBlackIsPlaying = _random.Next();

            // Pieces
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 12; j++)
                    _pieceTable[i, j] = _random.Next();
            }

            // Castling rights
            for (int i = 0; i < 4; i++)
                _castlingRights[i] = _random.Next();

            // En passant file
            for (int i = 0; i < 8; i++)
                _enPassantFile[i] = _random.Next();
        }

        #endregion

        #region Properties

        public int[,] PieceTable
        {
            get
            {
                return _pieceTable;
            }
        }

        public int[] CastlingRights
        {
            get
            {
                return _castlingRights;
            }
        }

        public int IfBlackIsPlaying
        {
            get
            {
                return _ifBlackIsPlaying;
            }
        }

        public int[] EnPassantFile
        {
            get
            {
                return _enPassantFile;
            }
        }

        #endregion
    }
}
