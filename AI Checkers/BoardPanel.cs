using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using AICheckers.Properties;

namespace AICheckers
{
    
    public class BoardPanel : Panel
    {
        private IAI _ai;
        private IAI _ai2;

        public static int sizeCheckers = 10; 

        //Assets
        readonly Image _checkerRed = Resources.checkerred;
        readonly Image _checkerRedKing = Resources.checkerredking;
        readonly Image _checkerBlack = Resources.checkerblack;
        private readonly Image _checkerBlackKing = Resources.checkerblackking;

        private readonly Color _lightSquare = Color.Gainsboro;

        private Point _oldPoint = new Point(-1, -1);
        private Point _newPoint = new Point(-1, -1);


        private int _squareWidth;

        private Point _selectedChecker = new Point(-1, -1);

        private readonly List<Move> _possibleMoves = new List<Move>();
        //List<Point> highlightedSquares = new List<Point>();

        private List<Tuple<CheckerColour , Move>> moves_played;

        private CheckerColour _currentTurn = CheckerColour.Black;

        private readonly Square[,] _board = new Square[sizeCheckers,sizeCheckers];
        
        public BoardPanel()
        {
            //this.DoubleBuffered = true;
            ResizeRedraw = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);

            init_game();

            AdvanceTurn();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Logic

            //Draw
            base.OnPaint(e);                        
            e.Graphics.Clear(_lightSquare);

            //Draw the board
            _squareWidth = (Width) / sizeCheckers;
            for (int c = 0; c < Width; c += _squareWidth)
            {
                int offset = 0;
                if ((c / _squareWidth) % 2 != 0)
                {
                    offset += _squareWidth;
                }
                for (int i = offset; i < Width; i += (_squareWidth * 2))
                {
                    e.Graphics.FillRectangle(Brushes.DarkGray, c, i, _squareWidth, _squareWidth);
                }
            }

            //Draw possible moves
            foreach (Move move in _possibleMoves)
            {
                e.Graphics.FillRectangle(Brushes.PaleTurquoise, move.Destination.X * _squareWidth, move.Destination.Y * _squareWidth, _squareWidth, _squareWidth);
            }

            //Draw selected checker
            if (_selectedChecker.X >= 0 && _selectedChecker.Y >= 0)
            {
                e.Graphics.FillRectangle(Brushes.PeachPuff, _selectedChecker.X * _squareWidth, _selectedChecker.Y * _squareWidth, _squareWidth, _squareWidth);
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
            for (int i = 0; i < sizeCheckers; i++)
            {
                for (int j = 0; j < sizeCheckers; j++)
                {
                    if (_board[i,j].Colour == CheckerColour.Red)
                    {
                        e.Graphics.DrawImage(_board[i, j].King ? _checkerRedKing : _checkerRed,
                            new Rectangle(j * _squareWidth, i * _squareWidth, _squareWidth, _squareWidth));
                    }
                    else if (_board[i, j].Colour == CheckerColour.Black)
                    {
                        e.Graphics.DrawImage(_board[i, j].King ? _checkerBlackKing : _checkerBlack,
                            new Rectangle(j * _squareWidth, i * _squareWidth, _squareWidth, _squareWidth));
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
            int clickedX = (int)(((double)e.X / Width) * sizeCheckers);
            int clickedY = (int)(((double)e.Y / Height) * sizeCheckers);

            Point clickedPoint = new Point(clickedX, clickedY);

            //Determine if this is the correct player
            if (_board[clickedY, clickedX].Colour != CheckerColour.Empty
                && _board[clickedY, clickedX].Colour != _currentTurn)
                return;

            //Determine if this is a move or checker selection
            List<Move> matches = _possibleMoves.Where(m => m.Destination == clickedPoint).ToList();
            if (matches.Count > 0)
            {
                //Move the checker to the clicked square
                MoveChecker(matches[0]);
            }
            else if (_board[clickedY, clickedX].Colour != CheckerColour.Empty)
            {
                //Select the clicked checker
                _selectedChecker.X = clickedX;
                _selectedChecker.Y = clickedY;
                _possibleMoves.Clear();

                Console.WriteLine(@"Selected Checker: {0}",_selectedChecker);
                Console.WriteLine(@"current turn: {0}",_currentTurn.ToString());
                
                Move[] openSquares = Utils.GetOpenSquares(_board, _selectedChecker);
                // Check if we have captued checkes
                Move[] moveWithCaptures = Utils.GetAllMoveCapturedByColor(_board, _currentTurn);

                _possibleMoves.AddRange(moveWithCaptures.Any() ? moveWithCaptures : openSquares);

                Invalidate();
            }            
        }

        private void MoveChecker(Move move)
        {
            // Check if we have captued checkes
            Move[] moveWithCaptures = Utils.GetAllMoveCapturedByColor(_board, _currentTurn);
            Console.WriteLine(@"Tour actuelle : "+move.Captures.Any());
            Console.WriteLine(@"Tour actuelle : "+moveWithCaptures.Length);

            if (!move.Captures.Any() && moveWithCaptures.Any())
            {
//                MessageBox.Show("Une capture est possible !!!");
                return;
            }

            // Saved the played move
            moves_played.Add(new Tuple<CheckerColour, Move>(_currentTurn, move));

            _board[move.Destination.Y, move.Destination.X].Colour = _board[move.Source.Y, move.Source.X].Colour;
            _board[move.Destination.Y, move.Destination.X].King = _board[move.Source.Y, move.Source.X].King;
            ResetSquare(move.Source);

            foreach (Point point in move.Captures)
                ResetSquare(point);

            _selectedChecker.X = -1;
            _selectedChecker.Y = -1;

            //Kinging
            if ((move.Destination.Y == 9 && _board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && _board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                _board[move.Destination.Y, move.Destination.X].King = true;
            }


            _possibleMoves.Clear();

            _oldPoint.X = move.Source.Y * _squareWidth;
            _oldPoint.Y = move.Source.X * _squareWidth;
            _newPoint.X = move.Destination.Y * _squareWidth;
            _newPoint.Y = move.Destination.X * _squareWidth;

            Invalidate();

            AdvanceTurn();
        }

        private void ResetSquare(Point square)
        {
            //Reset the square and the selected checker
            _board[square.Y, square.X].Colour = CheckerColour.Empty;
            _board[square.Y, square.X].King = false;
        }

        private void init_game()
        {
            //Initialize board
            for (int i = 0; i < sizeCheckers; i++)
            {
                for (int j = 0; j < sizeCheckers; j++)
                {
                    _board[i, j] = new Square {Colour = CheckerColour.Empty};
                }
            }

            //Setup Pieces
            for (int i = 0; i < sizeCheckers; i += 1)
            {
                int offset = 0;
                if (i % 2 != 0)
                {
                    offset++;
                }
                for (int j = offset; j < sizeCheckers; j += 2)
                {
                    if (i < 4) _board[i, j].Colour = CheckerColour.Red;
                    if (i > 5) _board[i, j].Colour = CheckerColour.Black;
                }
            }

            _ai = new AI_Learn {Colour = CheckerColour.Red};

            _ai2 = new AI_Learn {Colour = CheckerColour.Black};

            moves_played = new List<Tuple<CheckerColour, Move>>();
        }

        private void AdvanceTurn()
        {
            _currentTurn = _currentTurn == CheckerColour.Red ? CheckerColour.Black : CheckerColour.Red;

            // check victory of one side
            int redCount = 0;
            int blackCount = 0;
            for(int i=0;i<sizeCheckers;i++)
                for(int j=0; j<sizeCheckers; j++) {
                    if (_board[i, j].Colour == CheckerColour.Red)
                        redCount++;
                    if (_board[i, j].Colour == CheckerColour.Black)
                        blackCount++;
                }
            
            // If a winner is present
            if (blackCount == 0 || redCount == 0)
            {
                String side = blackCount == 0 ? "rouges" : "noirs";
                // display the message and ask the user want to replay
                if (MessageBox.Show(@"Les " + side + @" ont gagnés voulez-vous rejouer ?", @"Victoire des " + side,
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question) == DialogResult.Yes)
                    init_game();
                else
                    Application.Exit();

            }

            
            // Let the AI play
            /*if (_ai != null && _ai.Colour == _currentTurn)
            {
                Move aiMove = _ai.Process(_board);
                MoveChecker(aiMove);
            }

            if (_ai2 != null && _ai2.Colour == _currentTurn)
            {
                Move aiMove = _ai2.Process(_board);
                MoveChecker(aiMove);
            }*/
        }
    }
}
