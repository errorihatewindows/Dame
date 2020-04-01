using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Piece = System.Tuple<int,int>;
using Board = System.Collections.Generic.Dictionary<System.Tuple<int,int>, char>;

namespace Dame
{
    public class MCP
    {
        private Board board;
        private Form1 drawing;
        //Constructor
        public MCP(Form1 form)
        {
            drawing = form;
            Generate_Board();
        }

        public Board Get_Board()
        {
            return board;
        }

        private void Generate_Board()       //creates the initial Board State
        {
            board = new Board();

            //nur reihe 0-3 , 5-8 ist gespiegelt
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 8; x += 2)
                {
                    if (y == 1)
                    {
                        board.Add(Tuple.Create((x + 1), y), 'b');
                        //gegnerische Steine
                        board.Add(Tuple.Create(x, (7 - y)), 'w');
                    }
                    else if (y == 0 || y == 2)
                    {
                        board.Add(Tuple.Create(x, y), 'b');
                        board.Add(Tuple.Create((x + 1), (7 - y)), 'w');
                    }
                    else
                    {
                        board.Add(Tuple.Create((x + 1), y), '.');
                        board.Add(Tuple.Create(x, (7 - y)), '.');
                    }
                }

        }

        private List<Piece> Move(Piece piece)               //Liste of all valid Fields, no matter wich piece is on them
        {
            //output liste
            List<Piece> output = new List<Piece>();
            List<Piece> possible = new List<Piece>();
            //in frage kommen die 4 diagonalen
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 + 1));
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 + 1));
            //wenn keine der "ungültig" conditions aktiviert werden wird option in output packt
            foreach (Piece option in possible)
            {
                //normal Steine können nur Forwärts
                if (board[piece] == 'b') { if (piece.Item2 - 1 == option.Item2) { continue; } }
                if (board[piece] == 'w') { if (piece.Item2 + 1 == option.Item2) { continue; } }

                //Feld nur zwischen 0 und 7
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                //geschafft!
                output.Add(option);
            }
            return output;
        }
       
        private List<Piece> Split(string move)              //splits string to list of tuples
        {
            List<Piece> output = new List<Piece>();
            string position = "";
            for (int i = 0; i < move.Length; i += 3)
            {
                position = "";
                position += move[i];
                position += move[i + 1]; 
            }
            return output;
        }

        private bool is_black(Piece position)               //returns true if the position is a black square on the board
        {
            //check if the given position is in the field dictionary
            try     { char test = board[position]; }
            catch (System.Collections.Generic.KeyNotFoundException) { return false; }
            return true;
        }

        private bool is_jump(Piece start, Piece end)        //returns true if start -> is a jump
        {
            int xdistance = Math.Abs(start.Item1 - end.Item1);
            int ydistance = Math.Abs(start.Item2 - end.Item2);
            if (xdistance == 2 && ydistance == 2)   { return true; }
            else     { return false; }
        }

        private bool Check_Move(string smove, int player)               //checks if move is valid (player 0 is black)
        {
            //check move format
            //Check if move string is of valid format
            //convert move to list of Tuples
            List<Piece> move = Split(smove);
            //every position in list on a black field?
            foreach (Piece position in move)
            {
                if (!is_black(position)) { return false; }
            }
            //move is valid format, next check if it complies by the rules
            //check if its moving an own piece
            if (player == 0 && (board[move[0]] == 'w' || board[move[0]] == 'W')) { return false; }
            if (player == 1 && (board[move[0]] == 'b' || board[move[0]] == 'B')) { return false; }
            return true;
        }

        public void run()
        {
            Generate_Board();
            drawing.Draw_Board(board);
            Check_Move("A1,B2", 0);
        }
    }
}