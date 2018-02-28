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

            Move[] captured = Utils.GetAllMoveCapturedByColor(Board, Colour);
            
            if (captured.Any())
                return captured[(new Random()).Next(captured.Length)];
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
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
