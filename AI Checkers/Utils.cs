using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace AICheckers
{
    static class Utils
    {
        public static Move[] GetAllMoveCapturedByColor(Square[,] board, CheckerColour color)
        {
            List<Move> result = new List<Move>();
            for (int x = 0; x < BoardPanel.sizeCheckers; x++)
                for (int y = 0; y < BoardPanel.sizeCheckers; y++)
                    if (board[y,x].Colour == color)
                    {
                        foreach (Move move in GetOpenSquares(board, new Point(x, y)))
                            if (move.Captures.Any())
                                result.Add(move);
                    }
            return result.ToArray();
        }
        
        public static Move[] GetAllMovedByColor(Square[,] Board, CheckerColour color)
        {
            List<Move> result = new List<Move>();
            for (int x = 0; x < BoardPanel.sizeCheckers; x++)
            for (int y = 0; y < BoardPanel.sizeCheckers; y++)
                if (Board[y,x].Colour == color)
                {
                    foreach (Move move in GetOpenSquares(Board, new Point(x, y)))
                        result.Add(move);
                }

            Move[] captured = GetAllMoveCapturedByColor(Board, color);
            if (captured.Any())
                return captured;
            
            return result.ToArray();
        }

        //Convenience method
        public static Move[] GetOpenSquares(Square[,] board, Point checker)
        {
            return GetOpenSquares(board, checker, new Move(-1, -1, -1, -1), null);
        }

        private static Move[] GetOpenSquares(Square[,] board, Point checker, Move lastMove, List<Point> priorPositions)
        {
            if (priorPositions == null)
            {
                priorPositions = new List<Point> {checker};
            }

            List<Move> openSquares = new List<Move>();

            //Top Left
            if (board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Red || board[priorPositions[0].Y, priorPositions[0].X].King)        //Stop regular red pieces from moving backwards
            {
                if (IsValidPoint(checker.X - 1, checker.Y - 1))
                {
                    if (board[checker.Y - 1, checker.X - 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)  //Allow immediate empty spaces if it's the first jump
                    {
                        openSquares.Add(new Move(priorPositions[0], checker.X - 1, checker.Y - 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X - 2, checker.Y - 2)
                        && ((checker.X - 2) != lastMove.Destination.X || (checker.Y - 2) != lastMove.Destination.Y)
                        && ((checker.X - 2) != priorPositions[0].X || (checker.Y - 2) != priorPositions[0].Y)
                        && board[checker.Y - 1, checker.X - 1].Colour != board[checker.Y, checker.X].Colour
                        && board[checker.Y - 2, checker.X - 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X - 2, checker.Y - 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X - 1, checker.Y - 1));
                            move.Captures.AddRange(lastMove.Captures);
                            openSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures                        
                            openSquares.AddRange(GetOpenSquares(board, new Point(checker.X - 2, checker.Y - 2), move, priorPositions));
                        }
                    }
                }
            }

            //Top Right
            if (board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Red || board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X + 1, checker.Y - 1))
                {
                    if (board[checker.Y - 1, checker.X + 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        openSquares.Add(new Move(priorPositions[0], checker.X + 1, checker.Y - 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X + 2, checker.Y - 2)
                        && ((checker.X + 2) != lastMove.Destination.X || (checker.Y - 2) != lastMove.Destination.Y)
                        && ((checker.X + 2) != priorPositions[0].X || (checker.Y - 2) != priorPositions[0].Y)
                        && board[checker.Y - 1, checker.X + 1].Colour != board[checker.Y, checker.X].Colour
                        && board[checker.Y - 2, checker.X + 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X + 2, checker.Y - 2);
                        if (!priorPositions.Contains(new Point(checker.X + 2, checker.Y - 2)))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X + 1, checker.Y - 1));
                            move.Captures.AddRange(lastMove.Captures);
                            openSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures
                            openSquares.AddRange(GetOpenSquares(board, new Point(checker.X + 2, checker.Y - 2), move, priorPositions));
                        }
                    }
                }
            }

            //Bottom Left
            if (board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Black || board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X - 1, checker.Y + 1))
                {
                    if (board[checker.Y + 1, checker.X - 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        openSquares.Add(new Move(priorPositions[0], checker.X - 1, checker.Y + 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X - 2, checker.Y + 2)
                        && ((checker.X - 2) != lastMove.Destination.X || (checker.Y + 2) != lastMove.Destination.Y)
                        && ((checker.X - 2) != priorPositions[0].X || (checker.Y + 2) != priorPositions[0].Y)
                        && board[checker.Y + 1, checker.X - 1].Colour != board[checker.Y, checker.X].Colour
                        && board[checker.Y + 2, checker.X - 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X - 2, checker.Y + 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X - 1, checker.Y + 1));
                            move.Captures.AddRange(lastMove.Captures);
                            openSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures
                            openSquares.AddRange(GetOpenSquares(board, new Point(checker.X - 2, checker.Y + 2), move, priorPositions));
                        }
                    }
                }
            }

            //Bottom Right
            if (board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Black || board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X + 1, checker.Y + 1))
                {
                    if (board[checker.Y + 1, checker.X + 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        openSquares.Add(new Move(priorPositions[0], checker.X + 1, checker.Y + 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X + 2, checker.Y + 2)
                        && ((checker.X + 2) != lastMove.Destination.X || (checker.Y + 2) != lastMove.Destination.Y)
                        && ((checker.X + 2) != priorPositions[0].X || (checker.Y + 2) != priorPositions[0].Y)
                        && board[checker.Y + 1, checker.X + 1].Colour != board[checker.Y, checker.X].Colour
                        && board[checker.Y + 2, checker.X + 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X + 2, checker.Y + 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X + 1, checker.Y + 1));
                            move.Captures.AddRange(lastMove.Captures);
                            openSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures
                            openSquares.AddRange(GetOpenSquares(board, new Point(checker.X + 2, checker.Y + 2), move, priorPositions));
                        }
                    }
                }
            }

            // The color of the player
            CheckerColour playerColor = board[checker.Y, checker.X].Colour;
            // The possibility for the king to move
            int[,] possibility = { {1,1},{1,-1}, {-1,1}, {-1,-1}};
            ////King move
            if ((board[checker.Y, checker.X].King))
            {
                for (int i = 0; i < 4; i++)
                {
                    int testX = checker.X + possibility[i, 0];
                    int testY = checker.Y + possibility[i, 1];

                    List<Move> emptyMove = new List<Move>();
                    List<Move> capturedMove = new List<Move>();

                    while (IsValidPoint(testY, testX))
                    {
                        if (board[testY, testX].Colour == CheckerColour.Empty)
                        {
                            Move move = new Move(new Point(checker.X, checker.Y), new Point(testX, testY));
                            emptyMove.Add(move);
                        }
                        else
                        {
                            // Position of the king
                            int furtherX = testX + possibility[i, 0];
                            int furtherY = testY + possibility[i, 1];

                            // Check if the point to catch is a ennemy
                            // And if it is 
                            if (IsValidPoint(furtherX, furtherY) && board[testY, testX].Colour != playerColor && board[furtherY, furtherX].Colour == CheckerColour.Empty)
                            {
                                emptyMove.Clear();
                                Move move = new Move(new Point(checker.X, checker.Y), new Point(furtherX, furtherY));
                                move.Captures.Add(new Point(testX, testY));
                                move.Captures.AddRange(lastMove.Captures);

                                // Add the position
                                openSquares.Add(move);

                                Square[,] copy = board.Clone() as Square[,];

                                Square tmp = new Square {Colour = CheckerColour.Empty};
                                if (copy != null)
                                {
                                    copy[checker.Y, checker.X] = tmp;
                                    copy[testY, testX] = tmp;

                                    copy[furtherY, furtherX] = board[checker.Y, checker.X];
                                }

                                break;

                                //Use recursion to find multiple captures
                                //captured_move.AddRange(GetOpenSquares(Board, new Point(further_x, further_y), move, priorPositions));
                            }
                        }

                        // Iterate through all the possibility
                        testX += possibility[i, 0];
                        testY += possibility[i, 1];

                    }


                    openSquares.AddRange(emptyMove.Any() ? emptyMove : capturedMove);
                }
            }

            List<Move> capturesMoves = new List<Move>();
            // Check Captures
            foreach(Move move in openSquares)
            {
                if (move.Captures.Any())
                    capturesMoves.Add(move);
            }

            if (capturesMoves.Any())
                return capturesMoves.ToArray();
            return openSquares.ToArray();
        }

        private static bool IsValidPoint(int x, int y)
        {
            if (0 <= x && x < BoardPanel.sizeCheckers && 0 <= y && y < BoardPanel.sizeCheckers) return true;
            return false;
        }
    }
}
