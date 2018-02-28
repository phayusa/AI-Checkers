using System;
using System.Linq;
using System.Drawing;

namespace AICheckers
{
    class AI_Tree : IAI
    {
        int AI_MAXPLYLEVEL = 2;

        //Offensive
        int WEIGHT_CAPTUREPIECE = 2;
        int WEIGHT_CAPTUREKING = 1;
        int WEIGHT_CAPTUREDOUBLE = 5;
        int WEIGHT_CAPTUREMULTI = 10;

        //Defensive
        int WEIGHT_ATRISK = 3;
        int WEIGHT_KINGATRISK = 4;

        private Tree<Move> _gameTree;

        public CheckerColour Colour { get; set; }

        public Move Process(Square[,] board)
        {
            Console.WriteLine();
            Console.WriteLine(@"AI: Building Game Tree...");

            _gameTree = new Tree<Move>(new Move());

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Colour == Colour)
                    {
                        foreach (Move myPossibleMove in Utils.GetOpenSquares(board, new Point(j, i)))
                        {
                            
                            CalculateChildMoves(0, _gameTree.AddChild(myPossibleMove), myPossibleMove, DeepCopy(board));

                            //gameTree.AddChildren(Utils.GetOpenSquares(Board, new Point(j, i)));
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine(@"AI: Scoring Game Tree...");

            ScoreTreeMoves(board);

            return SumTreeMoves();
        }

        private Square[,] DeepCopy(Square[,] sourceBoard)
        {
            Square[,] result = new Square[ BoardPanel.sizeCheckers,  BoardPanel.sizeCheckers];

            for (int i = 0; i < BoardPanel.sizeCheckers; i++)
            {
                for (int j = 0; j <  BoardPanel.sizeCheckers; j++)
                {
                    result[i, j] = new Square
                    {
                        Colour = sourceBoard[i, j].Colour,
                        King = sourceBoard[i, j].King
                    };
                }
            }

            return result;
        }

        private void CalculateChildMoves(int recursionLevel, Tree<Move> branch, Move move, Square[,] vBoard)
        {
            if (recursionLevel >= AI_MAXPLYLEVEL)
            {
                return;
            }

            CheckerColour moveColour = vBoard[move.Source.Y, move.Source.X].Colour;

            //Move the checker
            vBoard = ExecuteVirtualMove(move, vBoard);

            //Calculate the other player's moves
            for (int i = 0; i < BoardPanel.sizeCheckers; i++)
            {
                for (int j = 0; j < BoardPanel.sizeCheckers; j++)
                {
                    if (vBoard[i, j].Colour != moveColour)
                    {
                        foreach (Move otherPlayerMove in Utils.GetOpenSquares(vBoard, new Point(j, i)))
                        {
                            if (vBoard[i, j].Colour != CheckerColour.Empty)
                            {
                                CalculateChildMoves(
                                    ++recursionLevel,
                                    branch.AddChild(otherPlayerMove),
                                    otherPlayerMove,
                                    DeepCopy(vBoard));
                            }
                        }
                    }
                }
            }
        }

        private Square[,] ExecuteVirtualMove(Move move, Square[,] board)
        {
            board[move.Destination.Y, move.Destination.X].Colour = board[move.Source.Y, move.Source.X].Colour;
            board[move.Destination.Y, move.Destination.X].King = board[move.Source.Y, move.Source.X].King;
            board[move.Source.Y, move.Source.X].Colour = CheckerColour.Empty;
            board[move.Source.Y, move.Source.X].King = false;

            //Kinging
            if ((move.Destination.Y == BoardPanel.sizeCheckers-1 && board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                board[move.Destination.Y, move.Destination.X].King = true;
            }

            return board;
        }

        private void ScoreTreeMoves(Square[,] board)
        {
            //Iterate over top-level (currently possible) moves
            Action<Move> scoreMove = move => move.Score = ScoreMove(move, board);

            foreach (Tree<Move> possibleMove in _gameTree.Children)
            {
                possibleMove.Traverse(scoreMove);
            }

        }

        private Move SumTreeMoves()
        {
            //Iterate over top-level (currently possible) moves

            int[] branchSum = {0};
            Action<Move> sumScores = move => branchSum[0] += move.Score;

            foreach (Tree<Move> possibleMove in _gameTree.Children)
            {
                possibleMove.Traverse(sumScores);
                possibleMove.Value.Score += branchSum[0];
                branchSum[0] = 0;
            }

            //Return highest score
            return _gameTree.Children.OrderByDescending(o => o.Value.Score).ToList()[0].Value;
        }

        private int ScoreMove(Move move, Square[,] board)
        {
            int score = 0;

            //Offensive traits
            score += move.Captures.Count * WEIGHT_CAPTUREPIECE;

            if (move.Captures.Count == 2) score += WEIGHT_CAPTUREDOUBLE;
            if (move.Captures.Count > 2) score += WEIGHT_CAPTUREMULTI;

            //Check King Captures
            foreach (Point point in move.Captures)
            {
                if (board[point.Y, point.X].King) score += WEIGHT_CAPTUREKING;
            }

            //Check if piece is at risk
            for (int i = 0; i < BoardPanel.sizeCheckers; i++)
            {
                for (int j = 0; j < BoardPanel.sizeCheckers; j++)
                {
                    if (board[i, j].Colour == Colour)
                    {
                        foreach (Move opponentMove in Utils.GetOpenSquares(board, new Point(j, i)))
                        {
                            if (opponentMove.Captures.Contains(move.Source))
                            {
                                if (board[move.Source.Y, move.Source.X].King)
                                {
                                    score += WEIGHT_KINGATRISK;
                                }
                                else
                                {
                                    score += WEIGHT_ATRISK;
                                }
                            }
                        }
                    }
                }
            }


            //Subtract score if we are evaluating an opponent's piece
            if (board[move.Source.Y, move.Source.X].Colour != Colour) score *= -1;

            Console.WriteLine(
                @"{0,-5} {1} Score: {2,2}",
                board[move.Source.Y, move.Source.X].Colour.ToString(),
                move,
                score
                ); 

            return score;
        }

    }
}
