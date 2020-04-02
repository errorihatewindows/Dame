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
    public delegate string gets_moves(Board boardstate, int player);
    struct Player_Data
    {
        public gets_moves move;
        public string name;
        public CPU cpu;
    }
    public class MCP
    {
        private Board board;
        private Form1 drawing;
        Player_Data[] Player;
        //Constructor
        public MCP(Form1 form)
        {
            drawing = form;
            Generate_Board();
        }

        public void set_user(string player1, string player2)
        {
            Player = new Player_Data[2];
            Player[0].name = player1;
            Player[1].name = player2;
            //set delegate (and class instance if needed) for player1
            if (player1.StartsWith("CPU")) 
            {
                Player[0].cpu = new CPU(drawing);
                Player[0].move = Player[0].cpu.get_move;
            }
            else { Player[0].move = drawing.get_move; }
            //do it again for player 2 ;D
            if (player2.StartsWith("CPU"))
            {
                Player[1].cpu = new CPU(drawing);
                Player[1].move = Player[1].cpu.get_move;
            }
            else { Player[1].move = drawing.get_move; }
        }   //sets name and delegate for players

        public Board Get_Board()                //getter for internal board state
        {
            return board;
        }           

        private char color(int player)          //player 0 -> 'b'
        {
            if (player==0) { return 'b'; }
            else           { return 'w'; }
        }

        private void Generate_Board()           //creates the initial Board State
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
            if (xdistance == 2 && ydistance == 2) { return true; }
            else { return false; }
        }

        private bool is_move(Piece start, Piece end)                    //returns true if start -> is a normal move
        {
            List<Piece> possible = Move(start);
            return possible.Contains(end);
        }

        private List<Piece> possible_jumps(Piece position,int player)   //returns a list of possible jumps from current position (only direct jumps)
        {
            List<Piece> output = new List<Piece>();
            List<Piece> possible = Move(position);  //fields this piece can move to
            foreach (Piece option in possible)
            {
                //can only jump over enemy pieces
                if (!(board[option] == color(1-player) || board[option] == Convert.ToChar(color(1-player)-32))) { continue; }
                //target is where to jump to jump over "option"
                Piece target = new Piece((2 * option.Item1) - position.Item1, (2 * option.Item2) - position.Item2);
                //check if target is still in the board
                if (target.Item1 < 0 || target.Item1 > 7) { continue; }
                if (target.Item2 < 0 || target.Item2 > 7) { continue; }
                //jump is valid if target is an empty field
                if (board[target] == '.') { output.Add(target); }
            }
            return output;
        }

        private Piece between(Piece start, Piece end)                   //returns the char between start and target of a jump, returns (-1,-1) if its not a jump
        {
            if (!is_jump(start,end)) { return new Piece(-1, -1); }
            int xdiff = end.Item1 - start.Item1;
            int ydiff = end.Item2 - start.Item2;
            return Tuple.Create(start.Item1 + (xdiff / 2), start.Item2 + (ydiff / 2));
        }

        private bool Check_Move_handled(string smove, int player)               //DONT CALL WITHOUT THE HANDLER, Check_Move
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
            if (!(board[move[0]] == color(player) || board[move[0]] == Convert.ToChar(color(player) - 32))) { return false; }
            //target must be empty
            if (board[move[1]] != '.') { return false; }
            //players MUST jump if possible
            if (!is_jump(move[0],move[1]))
            {
                foreach (KeyValuePair<Piece, char> kvp in board)
                {
                    if (!(kvp.Value == color(player) || kvp.Value == (color(player) - 20))) { continue; }
                    //if a jump is possible, the move should have been a jump
                    if (possible_jumps(kvp.Key, player).Count != 0) { return false; }
                }
                //there is no valid jump, is the move valid?
                if (is_move(move[0],move[1])) { return true; }
                else                           { return false; }
            }   //move is a jump
            //check if every jump until the last one is valid
            int i = 0;
            while (i < (move.Count-1))
            {
                //is the jump valid?
                if (!possible_jumps(move[i], player).Contains(move[i + 1])) { return false; }
                //perform jump
                board[move[i+1]] = board[move[i]];
                board[move[i]] = '.';
                board[between(move[i], move[i+1])] = '.';
                i++;
            }
            //if the player can still jump it is invalid (player has to jump as far as his current chain allows)
            if (possible_jumps(move[i],player).Count != 0) { return false; }
            else { return true; }
        }

        private bool Check_Move(string move, int player)                        //checks if move is valid (player 0 is black)
        {
            Board safecopy = new Board(board);
            bool output = Check_Move_handled(move, player);
            board = new Board(safecopy);
            return output;
        }

        private void Perform_Move(string smove, int player)                      //performs the move (doesnt check for valid)
        {
            List<Piece> move = Split(smove);
            for (int i = 0; i < (move.Count-1); i++)
            {
                //if its a jump remove the middle piece
                if (is_jump(move[i],move[i+1])) { board[between(move[i], move[i + 1])] = '.'; }
                board[move[i + 1]] = board[move[i]];
                board[move[i]] = '.';
            }
            //men that reach the kingsrow are crowned
            Piece final = move[move.Count - 1];
            if (final.Item2 == 7 && player == 0) { board[final] = 'B'; }
            if (final.Item2 == 0 && player == 1) { board[final] = 'W'; }
        }

        public void run()
        {
            string move = "";
            bool valid;
            Generate_Board();
            drawing.Draw_Board(board);
            //make sure there are players set that are able to play
            if (Player == null) { return; }
            //main gameloop
            int player = 0;
            while (true)
            {
                Console.WriteLine(Player[player].name + " am Zug");
                valid = false;
                while (!valid)
                {
                    move = Player[player].move(board, player);
                    valid = Check_Move(move, player);
                }
                Perform_Move(move, player);
                drawing.Draw_Board(board);
                //next player
                player = (player + 1) % 2;
            }
        }
    }
}