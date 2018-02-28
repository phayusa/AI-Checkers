using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    class AI_Learn : IAI
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

        //Strategic
        int WEIGHT_MAKEKING = 1;

        static Dictionary<Square[,], List<Move>> memory = new Dictionary<Square[,], List<Move>>();

        CheckerColour colour;

        Tree<Move> gameTree;

        public CheckerColour Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        private List<Move> get_knowns_childs(Square[,] board)
        {
            Move[] possibilities = Utils.GetAllMovedByColor(board, colour);
            
            if (memory.ContainsKey(board))
            {
                List<Move> played_moved = memory[board];
                if (played_moved.Count < possibilities.Length)
                {
                    int indexToGet = played_moved.Any() ? played_moved.Count : 0;
                    played_moved.Add(possibilities[indexToGet]);
                    memory.Add(board, played_moved);
                }
                return played_moved;
            }

            List<Move> to_play = new List<Move>();
            to_play.Add(possibilities[0]);
            memory.Add(board, to_play);

            return to_play;
        }


        public Move Process(Square[,] board)
        {
            Console.WriteLine();
            Console.WriteLine("AI: Building Game Tree...");

            gameTree = new Tree<Move>(new Move());

            for (int i = 0; i < BoardPanel.sizeCheckers; i++)
            {
                for (int j = 0; j < BoardPanel.sizeCheckers; j++)
                {
                    if (board[i, j].Colour == Colour)
                    {
                        // TODO : Check if all exists and Calculate if not full
                        foreach (Move myPossibleMove in Utils.GetOpenSquares(board, new Point(j, i)))
                        {

                            CalculateChildMoves(0, gameTree.AddChild(myPossibleMove), myPossibleMove, DeepCopy(board));

                            //gameTree.AddChildren(Utils.GetOpenSquares(Board, new Point(j, i)));
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("AI: Scoring Game Tree...");

            ScoreTreeMoves(board);

            return SumTreeMoves();
        }

        private Square[,] DeepCopy(Square[,] sourceBoard)
        {
            Square[,] result = new Square[BoardPanel.sizeCheckers, BoardPanel.sizeCheckers];

            for (int i = 0; i < BoardPanel.sizeCheckers; i++)
            {
                for (int j = 0; j < BoardPanel.sizeCheckers; j++)
                {
                    result[i, j] = new Square();
                    result[i, j].Colour = sourceBoard[i, j].Colour;
                    result[i, j].King = sourceBoard[i, j].King;
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

        private Square[,] ExecuteVirtualMove(Move move, Square[,] Board)
        {
            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Board[move.Source.Y, move.Source.X].Colour = CheckerColour.Empty;
            Board[move.Source.Y, move.Source.X].King = false;

            //Kinging
            if ((move.Destination.Y == (BoardPanel.sizeCheckers - 1) && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }

            return Board;
        }

        private void ScoreTreeMoves(Square[,] Board)
        {
            //Iterate over top-level (currently possible) moves
            Action<Move> scoreMove = (Move move) => move.Score = ScoreMove(move, Board);

            foreach (Tree<Move> possibleMove in gameTree.Children)
            {
                possibleMove.Traverse(scoreMove);
            }

        }

        private Move SumTreeMoves()
        {
            //Iterate over top-level (currently possible) moves

            int branchSum = 0;
            Action<Move> sumScores = (Move move) => branchSum += move.Score;

            foreach (Tree<Move> possibleMove in gameTree.Children)
            {
                possibleMove.Traverse(sumScores);
                possibleMove.Value.Score += branchSum;
                branchSum = 0;
            }

            //Return highest score
            return gameTree.Children.OrderByDescending(o => o.Value.Score).ToList()[0].Value;
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

            //Check strategy
            //TODO: Kinging code here


            //Subtract score if we are evaluating an opponent's piece
            if (board[move.Source.Y, move.Source.X].Colour != colour) score *= -1;

            Console.WriteLine(
                "{0,-5} {1} Score: {2,2}",
                board[move.Source.Y, move.Source.X].Colour.ToString(),
                move.ToString(),
                score
                );

            return score;
        }

    }
}
