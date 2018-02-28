using System;
using System.Collections.Generic;
using System.Drawing;

namespace AICheckers
{
    class AI_Random : IAI
    {
        public CheckerColour Colour { get; set; }

        public Move Process(Square[,] board)
        {
            List<Move> moves = new List<Move>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Colour == Colour)
                    {
                        moves.AddRange(Utils.GetOpenSquares(board, new Point(j, i)));
                    }
                }
            }

            return moves[(new Random()).Next(moves.Count)];
        }
    }
}
