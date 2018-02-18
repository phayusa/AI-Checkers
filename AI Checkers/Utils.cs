using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    static class Utils
    {
        public static Move[] GetAllMoveCapturedByColor(Square[,] Board, CheckerColour color)
        {
            List<Move> result = new List<Move>();
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    if (Board[x,y].Colour == color)
                    {
                        foreach (Move move in GetOpenSquares(Board, new Point(y, x)))
                            if (move.Captures.Any())
                                result.Add(move);
                    }
            return result.ToArray();
        }

        //Convenience method
        public static Move[] GetOpenSquares(Square[,] Board, Point checker)
        {
            return GetOpenSquares(Board, checker, new Move(-1, -1, -1, -1), null);
        }

        private static Move[] GetOpenSquares(Square[,] Board, Point checker, Move lastMove, List<Point> priorPositions)
        {
            if (priorPositions == null)
            {
                priorPositions = new List<Point>();
                priorPositions.Add(checker);
            }

            List<Move> OpenSquares = new List<Move>();

            //Top Left
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Red || Board[priorPositions[0].Y, priorPositions[0].X].King)        //Stop regular red pieces from moving backwards
            {
                if (IsValidPoint(checker.X - 1, checker.Y - 1))
                {
                    if (Board[checker.Y - 1, checker.X - 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)  //Allow immediate empty spaces if it's the first jump
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X - 1, checker.Y - 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X - 2, checker.Y - 2)
                        && ((checker.X - 2) != lastMove.Destination.X || (checker.Y - 2) != lastMove.Destination.Y)
                        && ((checker.X - 2) != priorPositions[0].X || (checker.Y - 2) != priorPositions[0].Y)
                        && Board[checker.Y - 1, checker.X - 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y - 2, checker.X - 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X - 2, checker.Y - 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X - 1, checker.Y - 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures                        
                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X - 2, checker.Y - 2), move, priorPositions));
                        }
                    }
                }
            }

            //Top Right
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Red || Board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X + 1, checker.Y - 1))
                {
                    if (Board[checker.Y - 1, checker.X + 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X + 1, checker.Y - 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X + 2, checker.Y - 2)
                        && ((checker.X + 2) != lastMove.Destination.X || (checker.Y - 2) != lastMove.Destination.Y)
                        && ((checker.X + 2) != priorPositions[0].X || (checker.Y - 2) != priorPositions[0].Y)
                        && Board[checker.Y - 1, checker.X + 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y - 2, checker.X + 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X + 2, checker.Y - 2);
                        if (!priorPositions.Contains(new Point(checker.X + 2, checker.Y - 2)))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X + 1, checker.Y - 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures
                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X + 2, checker.Y - 2), move, priorPositions));
                        }
                    }
                }
            }

            //Bottom Left
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Black || Board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X - 1, checker.Y + 1))
                {
                    if (Board[checker.Y + 1, checker.X - 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X - 1, checker.Y + 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X - 2, checker.Y + 2)
                        && ((checker.X - 2) != lastMove.Destination.X || (checker.Y + 2) != lastMove.Destination.Y)
                        && ((checker.X - 2) != priorPositions[0].X || (checker.Y + 2) != priorPositions[0].Y)
                        && Board[checker.Y + 1, checker.X - 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y + 2, checker.X - 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X - 2, checker.Y + 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X - 1, checker.Y + 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures
                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X - 2, checker.Y + 2), move, priorPositions));
                        }
                    }
                }
            }

            //Bottom Right
            if (Board[priorPositions[0].Y, priorPositions[0].X].Colour != CheckerColour.Black || Board[priorPositions[0].Y, priorPositions[0].X].King)
            {
                if (IsValidPoint(checker.X + 1, checker.Y + 1))
                {
                    if (Board[checker.Y + 1, checker.X + 1].Colour == CheckerColour.Empty && lastMove.Destination.X == -1)
                    {
                        OpenSquares.Add(new Move(priorPositions[0], checker.X + 1, checker.Y + 1));
                    }
                    //Check for a capturable piece
                    else if (IsValidPoint(checker.X + 2, checker.Y + 2)
                        && ((checker.X + 2) != lastMove.Destination.X || (checker.Y + 2) != lastMove.Destination.Y)
                        && ((checker.X + 2) != priorPositions[0].X || (checker.Y + 2) != priorPositions[0].Y)
                        && Board[checker.Y + 1, checker.X + 1].Colour != Board[checker.Y, checker.X].Colour
                        && Board[checker.Y + 2, checker.X + 2].Colour == CheckerColour.Empty)
                    {
                        Point newDest = new Point(checker.X + 2, checker.Y + 2);
                        if (!priorPositions.Contains(newDest))
                        {
                            Move move = new Move(priorPositions[0], newDest);
                            move.Captures.Add(new Point(checker.X + 1, checker.Y + 1));
                            move.Captures.AddRange(lastMove.Captures);
                            OpenSquares.Add(move);

                            priorPositions.Add(newDest);

                            //Use recursion to find multiple captures
                            OpenSquares.AddRange(GetOpenSquares(Board, new Point(checker.X + 2, checker.Y + 2), move, priorPositions));
                        }
                    }
                }
            }

            // The color of the player
            CheckerColour playerColor = Board[checker.Y, checker.X].Colour;
            // The possibility for the king to move
            int[,] possibility = new int[4,2]{ {1,1},{1,-1}, {-1,1}, {-1,-1}};
            ////King move
            if ((Board[checker.X, checker.Y].King))
            {
                for (int i = 0; i < 4; i++)
                {
                    int test_x = checker.X + possibility[i, 0];
                    int test_y = checker.Y + possibility[i, 1];

                    List<Move> empty_move = new List<Move>();
                    List<Move> captured_move = new List<Move>();

                    while (IsValidPoint(test_x, test_y))
                    {
                        if (Board[test_y, test_x].Colour == CheckerColour.Empty)
                        {
                            Move move = new Move(new Point(checker.X, checker.Y), new Point(test_x, test_y));
                            empty_move.Add(move);
                            //// Add the position
                            //OpenSquares.Add(move);

                            ////Use recursion to find multiple captures
                            //OpenSquares.AddRange(GetOpenSquares(Board, new Point(test_x, test_y), move, priorPositions));
                        }
                        else
                        {
                            int further_x = test_x + possibility[i, 0];
                            int further_y = test_y + possibility[i, 1];
                            if (IsValidPoint(further_x, further_y) && Board[test_x, test_y].Colour != playerColor && Board[further_x, further_y].Colour == CheckerColour.Empty)
                            {
                                empty_move.Clear();
                                Move move = new Move(new Point(checker.X, checker.Y), new Point(further_x, further_y));
                                move.Captures.Add(new Point(test_x, test_y));
                                move.Captures.AddRange(lastMove.Captures);
                                // Add the position
                                //OpenSquares.Add(move);

                                //Use recursion to find multiple captures
                                captured_move.AddRange(GetOpenSquares(Board, new Point(further_x, further_y), move, priorPositions));
                            }
                        }

                        // Iterate through all the possibility
                        test_x += possibility[i, 0];
                        test_y += possibility[i, 1];

                    }


                    if (empty_move.Any())
                    {
                        OpenSquares.AddRange(empty_move);
                    }
                    else
                    {
                        OpenSquares.AddRange(captured_move);
                    }
                }
            }

            List<Move> captures_moves = new List<Move>();
            // Check Captures
            foreach(Move move in OpenSquares)
            {
                if (move.Captures.Any())
                    captures_moves.Add(move);
            }

            if (captures_moves.Any())
                return captures_moves.ToArray();
            return OpenSquares.ToArray();
        }

        private static bool IsValidPoint(int x, int y)
        {
            if (0 <= x && x < 8 && 0 <= y && y < 8) return true;
            return false;
        }

        private static bool IsValidPoint(Point point)
        {
            return (IsValidPoint(point.X, point.Y));
        }
    }
}
