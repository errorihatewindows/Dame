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

        private char color(int player)      //player 0 -> 'b'
        {
            if (player==1) { return 'b'; }
            else           { return 'w'; }
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
            //4 diagonals possible
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 + 1));
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 + 1));
            foreach (Piece option in possible)
            {
                //men only move forward
                if (board[piece] == 'b') { if (piece.Item2 - 1 == option.Item2) { continue; } }
                if (board[piece] == 'w') { if (piece.Item2 + 1 == option.Item2) { continue; } }

                //Field only in range 0-7
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                //option is valid!
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
                output.Add(drawing.StringToTuple(position));
            }
            return output;
        }

        private bool is_black(Piece position)                           //returns true if the position is a black square on the board
        {
            //check if the given position is in the field dictionary
            try     { char test = board[position]; }
            catch (System.Collections.Generic.KeyNotFoundException) { return false; }
            return true;
        }

        private bool is_jump(Piece start, Piece end)                    //returns true if start -> is a jump
        {
            int xdistance = Math.Abs(start.Item1 - end.Item1);
            int ydistance = Math.Abs(start.Item2 - end.Item2);
            if (xdistance == 2 && ydistance == 2)   { return true; }
            else     { return false; }
        }

        private List<Piece> possible_jumps(Piece position,int player)   //returns a list of possible jumps from current position (only direct jumps)
        {
            List<Piece> output = new List<Piece>();
            List<Piece> possible = Move(position);  //fields this piece can move to
            foreach (Piece option in possible)
            {
                //can only jump over enemy pieces
                if (!(board[option] == color(1-player) || board[option] == (color(1-player)-20))) { continue; }
                //target is where to jump to jump over "option"
                Piece target = new Piece((2 * option.Item1) - position.Item1, (2 * option.Item2) - position.Item2);
                //jump is valid if target is an empty field
                if (board[target] == '.') { output.Add(target); }
            }
            return output;
        }

        private bool Check_Move(string smove, int player)               //checks if move is valid (player 0 is black)
        {
            //check move syntax
            //Check if move string is of valid format
            if (!drawing.check_Syntax(smove)) { return false; }
            List<Piece> move = Split(smove);
            //every position in list on a black field?
            foreach (Piece position in move)
            {
                if (!is_black(position)) { return false; }
            }   //moves which made it this far are syntacticly correct, next check if they comply by the rules
            //check if its moving an own piece ( -20 converts lowercase to uppercase)
            if (board[move[0]] == color(player) || board[move[0]] == (color(player) - 20)) { return false; }
            //players MUST jump if possible
            if (!is_jump(move[0],move[1]))
            {
                foreach (KeyValuePair<Piece, char> kvp in board)
                {
                    if (kvp.Value != color(player) && kvp.Value != (color(player) - 20)) { continue; }
                    //if a jump is possible, the move should have been a jump
                    if (possible_jumps(kvp.Key, player).Count != 0) { return false; }
                }
            }
            return true;
        }

        public void run()
        {
            Generate_Board();
            drawing.Draw_Board(board);
            Console.WriteLine(Check_Move("A1B3", 0).ToString());
            Console.WriteLine(Check_Move("a1,B2", 0).ToString());
            Console.WriteLine(Check_Move("A1,B3", 0).ToString());
            Console.WriteLine(Check_Move("A3,B4", 0).ToString());
        }
    }
}