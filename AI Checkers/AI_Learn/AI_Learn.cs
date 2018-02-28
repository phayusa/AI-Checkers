using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    class AI_Learn : IAI
    {
        //Offensive
        int WEIGHT_CAPTUREPIECE = 2;
        int WEIGHT_CAPTUREKING = 1;
        int WEIGHT_CAPTUREDOUBLE = 5;
        int WEIGHT_CAPTUREMULTI = 10;

        //Defensive
        int WEIGHT_ATRISK = 3;
        int WEIGHT_KINGATRISK = 4;

        static Dictionary<Square[,], List<Move>> memory = new Dictionary<Square[,], List<Move>>();

        CheckerColour colour;

        public CheckerColour Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        // Get the best move by searching in the graph
        private Move GetBestMove(Square[,] board)
        {
            // We get all the possibilities for this state
            Move[] possibilities = Utils.GetAllMovedByColor(board, colour);

            foreach(Move p in possibilities){
                Console.WriteLine("x "+p.Source.X+" y "+p.Source.Y);
                Console.WriteLine("cap "+p.Captures);
            }
            
            if (memory.ContainsKey(board))
            {
                List<Move> played_moved = memory[board];
                if (played_moved.Count < possibilities.Length)
                {
                    int indexToGet = played_moved.Any() ? played_moved.Count: 0;
                    played_moved.Add(possibilities[indexToGet]);
                    memory.Remove(board);
                    memory.Add(board, played_moved);

                    return possibilities[indexToGet];
                }

                int best_score = ScoreByChild(board, played_moved[0], colour);
                Move best_move = played_moved[0];

                // Looking for the best move in child
                foreach(Move move in played_moved){
                    int test_score = ScoreByChild(board, move, colour);
                    if (best_score < test_score){
                        best_score = test_score;
                        best_move = move;
                    }
                }
                return best_move;



            }

            List<Move> to_play = new List<Move>();
            to_play.Add(possibilities[0]);
            memory.Add(board, to_play);

            Console.WriteLine(possibilities[0].Source.X+" "+possibilities[0].Source.Y);

            return possibilities[0];
        }

        private int ScoreByChild(Square[,] board, Move move, CheckerColour colour_to_test){
            int count = 0;
            int count_opponent = 0;

            CheckerColour opponent_color = colour_to_test == CheckerColour.Black ? CheckerColour.Red : CheckerColour.Black;


            for (int i = 0; i < BoardPanel.sizeCheckers; i++)
                for (int j = 0; j < BoardPanel.sizeCheckers; j++)
                    if (board[j, i].Colour == colour)
                        count++;
                    else
                        if (board[j, i].Colour == opponent_color)
                        count_opponent++;
            if (count == 0)
                return -999;
            if (count_opponent == 0)
                return 999;
            
            // Avoid to pass the object by reference
            Square[,] test_board = DeepCopy(board);

            // Make the move on the saved board
            ExecuteVirtualMove(move, test_board);
            
            // We get all the possibilities for this state
            Move[] possibilities = Utils.GetAllMovedByColor(board, colour_to_test);

            if (possibilities.Count() == 0)
                return 0;

            int score = ScoreMove(move, board, colour_to_test);

            // We limit our search at our known states
            if (memory.ContainsKey(board))
            {
                List<Move> played_moved = memory[board];
                // We have not enough information on the state we must try all the possibilites
                if (played_moved.Count < possibilities.Length)
                {
                    int indexToGet = played_moved.Any() ? played_moved.Count : 0;
                    played_moved.Add(possibilities[indexToGet]);
                    memory.Remove(board);
                    memory.Add(board, played_moved);

                    Square[,] test_board_bis = DeepCopy(test_board);
                    ExecuteVirtualMove(possibilities[indexToGet], test_board_bis);
                    score += ScoreByChild(test_board, possibilities[indexToGet], opponent_color);

                }else{
                    
                    foreach(Move test_move in played_moved){
                        Square[,] test_board_bis = DeepCopy(test_board);
                        ExecuteVirtualMove(test_move, test_board_bis);
                        score += ScoreByChild(test_board, test_move, opponent_color);
                    }
                }

                return score;


            }else{
                List<Move> to_play = new List<Move>();
                to_play.Add(possibilities[0]);
                memory.Add(board, to_play);

                Square[,] test_board_bis = DeepCopy(test_board);
                ExecuteVirtualMove(possibilities[0], test_board_bis);
                return ScoreByChild(test_board, possibilities[0], opponent_color);
            }
        }

        public Move Process(Square[,] board)
        {
            Console.WriteLine();
            Console.WriteLine("AI: Building Game Tree...");

            return GetBestMove(board);
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

        private Square[,] ExecuteVirtualMove(Move move, Square[,] board)
        {
            board[move.Destination.Y, move.Destination.X].Colour = board[move.Source.Y, move.Source.X].Colour;
            board[move.Destination.Y, move.Destination.X].King = board[move.Source.Y, move.Source.X].King;
            board[move.Source.Y, move.Source.X].Colour = CheckerColour.Empty;
            board[move.Source.Y, move.Source.X].King = false;

            //Kinging
            if ((move.Destination.Y == (BoardPanel.sizeCheckers - 1) && board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                board[move.Destination.Y, move.Destination.X].King = true;
            }

            // Clean all the captures points
            foreach (Point point in move.Captures)
            {
                //Reset the square and the selected checker
                board[point.Y, point.X].Colour = CheckerColour.Empty;
                board[point.Y, point.X].King = false;
            }

            return board;
        }

        private int ScoreMove(Move move, Square[,] board, CheckerColour test_color)
        {
            int score = 0;

            int count_opponent = 0;
            int count_piece = 0;

            CheckerColour opponent_color = test_color == CheckerColour.Black ? CheckerColour.Red : CheckerColour.Black;

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
                    if (board[j, i].Colour == test_color)
                    {
                        count_piece++;
                        foreach (Move opponentMove in Utils.GetAllMovedByColor(board, opponent_color))
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

                    if(board[j, i].Colour == opponent_color){
                        count_opponent++;
                    }
                }
            }

            if (count_opponent == 0)
                score += 999;
            if (count_piece == 0)
                score -= 999;


            //Subtract score if we are evaluating an opponent's piece
            if (board[move.Source.Y, move.Source.X].Colour != test_color) score *= -1;

            Console.WriteLine("Score : " + count_piece + " opponent " + count_opponent);

            return score;
        }

    }
}
