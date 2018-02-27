using System;
using System.Collections.Generic;
using System.Drawing;

namespace AICheckers
{
    class AI_Learn : IAI
    {
        CheckerColour colour;

        public CheckerColour Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        public Move Process(Square[,] Board)
        {
            List<Move> moves = new List<Move>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j].Colour == Colour)
                    {
                        moves.AddRange(Utils.GetOpenSquares(Board, new Point(j, i)));
                    }
                }
            }

            return moves[(new Random()).Next(moves.Count)];
        }
    }
}
