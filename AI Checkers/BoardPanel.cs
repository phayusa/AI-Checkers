using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using AICheckers.Properties;

namespace AICheckers
{
    public class BoardPanel : Panel
    {
        IAI AI = null;
        IAI AI2 = null;

        //Assets
        Image checkerRed = Resources.checkerred;
        Image checkerRedKing = Resources.checkerredking;
        Image checkerBlack = Resources.checkerblack;
        Image checkerBlackKing = Resources.checkerblackking;

        Color darkSquare = Color.DarkGray;
        Color lightSquare = Color.Gainsboro;

        bool animating = false;
        const int animDuration = 1000;
        Square animPiece;
        Point oldPoint = new Point(-1, -1);
        Point currentPoint = new Point(-1, -1);
        Point newPoint = new Point(-1, -1);
        int delta = 10;

        int squareWidth = 0;

        Point selectedChecker = new Point(-1, -1);
        List<Move> possibleMoves = new List<Move>();
        //List<Point> highlightedSquares = new List<Point>();

        List<Tuple<CheckerColour , Move>> moves_played;

        CheckerColour currentTurn = CheckerColour.Black;
        private System.ComponentModel.IContainer components;

        Square[,] Board = new Square[8,8];
        
        public BoardPanel()
            : base()
        {
            //this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);

            init_game();

            //AdvanceTurn();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Logic

            //Draw
            base.OnPaint(e);                        
            e.Graphics.Clear(lightSquare);

            //Draw the board
            squareWidth = (Width) / 8;
            for (int c = 0; c < Width; c += squareWidth)
            {
                int offset = 0;
                if ((c / squareWidth) % 2 != 0)
                {
                    offset += squareWidth;
                }
                for (int i = offset; i < Width; i += (squareWidth * 2))
                {
                    e.Graphics.FillRectangle(Brushes.DarkGray, c, i, squareWidth, squareWidth);
                }
            }

            //Draw possible moves
            foreach (Move move in possibleMoves)
            {
                e.Graphics.FillRectangle(Brushes.PaleTurquoise, move.Destination.X * squareWidth, move.Destination.Y * squareWidth, squareWidth, squareWidth);
            }

            //Draw selected checker
            if (selectedChecker.X >= 0 && selectedChecker.Y >= 0)
            {
                e.Graphics.FillRectangle(Brushes.PeachPuff, selectedChecker.X * squareWidth, selectedChecker.Y * squareWidth, squareWidth, squareWidth);
            }

            //Draw Border
            e.Graphics.DrawRectangle(Pens.DarkGray,
            e.ClipRectangle.Left,
            e.ClipRectangle.Top,
            e.ClipRectangle.Width - 1,
            e.ClipRectangle.Height - 1);

            //Set for higher quality resizing of images
            //e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            //Draw Checker Images
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i,j].Colour == CheckerColour.Red)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(checkerRedKing, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(checkerRed, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                    else if (Board[i, j].Colour == CheckerColour.Black)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(checkerBlackKing, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(checkerBlack, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                }
            }

            //if (animating)
            //{
            //    Console.WriteLine("Animating" + currentPoint.ToString());
            //    //currentPoint.X += -1 * (newPoint.X - oldPoint.X) / 10;
            //    //currentPoint.Y += -1 * (newPoint.Y - oldPoint.Y) / 10;

            //    e.Graphics.DrawImage(checkerBlack, new Rectangle(currentPoint.Y, currentPoint.Y, squareWidth, squareWidth));

            //    if (currentPoint.X == newPoint.X && currentPoint.Y == newPoint.Y)
            //        animating = false;
            //}

        }
        
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int clickedX = (int)(((double)e.X / (double)Width) * 8.0d);
            int clickedY = (int)(((double)e.Y / (double)Height) * 8.0d);

            Point clickedPoint = new Point(clickedX, clickedY);

            //Determine if this is the correct player
            if (Board[clickedY, clickedX].Colour != CheckerColour.Empty
                && Board[clickedY, clickedX].Colour != currentTurn)
                return;

            //Determine if this is a move or checker selection
            List<Move> matches = possibleMoves.Where(m => m.Destination == clickedPoint).ToList<Move>();
            if (matches.Count > 0)
            {
                //Move the checker to the clicked square
                MoveChecker(matches[0]);
            }
            else if (Board[clickedY, clickedX].Colour != CheckerColour.Empty)
            {
                //Select the clicked checker
                selectedChecker.X = clickedX;
                selectedChecker.Y = clickedY;
                possibleMoves.Clear();

                Console.WriteLine("Selected Checker: {0}",selectedChecker.ToString());
                Console.WriteLine("current turn: {0}",currentTurn.ToString());
                
                Move[] OpenSquares = Utils.GetOpenSquares(Board, selectedChecker);
                // Check if we have captued checkes
                Move[] MoveWithCaptures = Utils.GetAllMoveCapturedByColor(Board, currentTurn);

                if (MoveWithCaptures.Any())
                    possibleMoves.AddRange(MoveWithCaptures);
                else
                    possibleMoves.AddRange(OpenSquares);

                this.Invalidate();
            }            
        }

        private void MoveChecker(Move move)
        {
            // Check if we have captued checkes
            Move[] MoveWithCaptures = Utils.GetAllMoveCapturedByColor(Board, currentTurn);
            Console.WriteLine("Tour actuelle : "+move.Captures.Any());
            Console.WriteLine("Tour actuelle : "+MoveWithCaptures.Count());

            if (!move.Captures.Any() && MoveWithCaptures.Any())
            {
                MessageBox.Show("Une capture est possible !!!");
            }

            // Saved the played move
            moves_played.Add(new Tuple<CheckerColour, AICheckers.Move>(currentTurn, move));

            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            ResetSquare(move.Source);

            foreach (Point point in move.Captures)
                ResetSquare(point);

            selectedChecker.X = -1;
            selectedChecker.Y = -1;

            //Kinging
            if ((move.Destination.Y == 7 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }


            possibleMoves.Clear();

            oldPoint.X = move.Source.Y * squareWidth;
            oldPoint.Y = move.Source.X * squareWidth;
            newPoint.X = move.Destination.Y * squareWidth;
            newPoint.Y = move.Destination.X * squareWidth;
            currentPoint = oldPoint;
            animating = true;

            this.Invalidate();

            AdvanceTurn();
        }

        private void ResetSquare(Point square)
        {
            //Reset the square and the selected checker
            Board[square.Y, square.X].Colour = CheckerColour.Empty;
            Board[square.Y, square.X].King = false;
        }

        private void init_game()
        {
            //Initialize board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Board[i, j] = new Square();
                    Board[i, j].Colour = CheckerColour.Empty;
                }
            }

            //Setup Pieces
            for (int i = 0; i < 8; i += 1)
            {
                int offset = 0;
                if (i % 2 != 0)
                {
                    offset++;
                }
                for (int j = offset; j < 8; j += 2)
                {
                    if (i < 3) Board[i, j].Colour = CheckerColour.Red;
                    if (i > 4) Board[i, j].Colour = CheckerColour.Black;
                    Board[i, j].King = true;
                }
            }

            AI = new AI_Tree();
            AI.Colour = CheckerColour.Red;

            AI2 = new AI_Tree();
            AI2.Colour = CheckerColour.Black;

            moves_played = new List<Tuple<CheckerColour, AICheckers.Move>>();
        }

        private void AdvanceTurn()
        {
            if (currentTurn == CheckerColour.Red)
            {
                currentTurn = CheckerColour.Black;
            }
            else
            {
                currentTurn = CheckerColour.Red;
            }

            // check victory of one side
            int redCount = 0;
            int blackCount = 0;
            for(int i=0;i<8;i++)
                for(int j=0; j<8; j++) {
                    if (Board[i, j].Colour == CheckerColour.Red)
                        redCount++;
                    if (Board[i, j].Colour == CheckerColour.Black)
                        blackCount++;
                }
            
            // If a winner is present
            if (blackCount == 0 || redCount == 0)
            {
                String side = blackCount == 0 ? "rouges" : "noirs";
                // display the message and ask the user want to replay
                if (MessageBox.Show("Les " + side + " ont gagnés voulez-vous rejouer ?", "Victoire des " + side,
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    init_game();
                else
                    System.Windows.Forms.Application.Exit();

            }

            
            //// Let the AI play
            //if (AI != null && AI.Colour == currentTurn)
            //{
            //    Move aiMove = AI.Process(Board);
            //    MoveChecker(aiMove);
            //}

            //if (AI2 != null && AI2.Colour == currentTurn)
            //{
            //    Move aiMove = AI2.Process(Board);
            //    MoveChecker(aiMove);
            //}
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
