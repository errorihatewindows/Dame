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
        public bool is_cpu;
        public CPU cpu;
        public random_player rando;
    }
    public class MCP
    {
        private int reversible_moves = 0;
        private Board board;
        private Form1 drawing;
        Player_Data[] Player;
        //Constructor
        public MCP(Form1 form)
        {
            drawing = form;
            Generate_Board();
        }

        public void set_user(string player1, string player2)    //sets name and delegate for players
        {
            Player = new Player_Data[2];
            string[] player = { player1, player2 };
            Player[0].name = player[0];
            Player[1].name = player[1];
            for (int i = 0; i < 2; i++) 
            {
                //set delegate (and class instance if needed) for player1
                if (player[i].StartsWith("CPU"))
                {
                    Player[i].cpu = new CPU(drawing);
                    Player[i].move = Player[i].cpu.get_move;
                    Player[i].is_cpu = true;
                }
                else if (player[i].StartsWith("RAND"))
                {
                    Player[i].rando = new random_player(drawing);
                    Player[i].move = Player[i].rando.get_move;
                    Player[i].is_cpu = true;
                }
                else
                {
                    Player[i].move = drawing.get_move;
                    Player[i].is_cpu = false;
                }
            }

            Generate_Board();

        }   

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
            //move is a normal move
            if (!is_jump(move[0], move[1]))
            {
                //moves are always 5 chars long
                if (smove.Length != 5) { return false; }
                foreach (KeyValuePair<Piece, char> kvp in board)
                {
                    if (!(kvp.Value == color(player) || kvp.Value == (color(player) - 32))) { continue; }
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
            //reversible moves resets to 0 if moved with a uncrowned piece
            reversible_moves++;
            if (board[move[0]] == 'b' || board[move[0]] == 'w') { reversible_moves = 0; }
            for (int i = 0; i < (move.Count-1); i++)
            {
                //if its a jump remove the middle piece and set reversible moves to 0
                if (is_jump(move[i],move[i+1])) { reversible_moves = 0; board[between(move[i], move[i + 1])] = '.'; }
                board[move[i + 1]] = board[move[i]];
                board[move[i]] = '.';
            }
            //men that reach the kingsrow are crowned
            Piece final = move[move.Count - 1];
            if (final.Item2 == 7 && player == 0) { board[final] = 'B'; }
            if (final.Item2 == 0 && player == 1) { board[final] = 'W'; }
        }

        bool is_lost(int player)                  //is current board lost for player?
        {
            //checks if a piece is left and if it can move
            //assumes true, if a piece that can move is found return false
            foreach (KeyValuePair<Piece, char> position in board)
            {
                //skip if not own color
                if (!(position.Value == color(player) || position.Value == Convert.ToChar(color(player) - 32))) { continue; }
                //can we jump from this position?
                if (possible_jumps(position.Key,player).Count > 0) { return false; }
                //can we do a normal move?
                foreach (Piece option in Move(position.Key))
                {
                    if (board[option] == '.') { return false; }
                }
            }
            return true;
        }

        public int run(bool output)
        {
            //only wait or draw boards if output is set to true
            string move = "";
            bool valid;
            Generate_Board();
            reversible_moves = 0;
            //main gameloop
            int player = 0;
            while (!is_lost(player) && reversible_moves < 30)
            {
                if (output)
                {
                    Console.WriteLine(Player[player].name + " am Zug");
                    drawing.labelText(Player[player].name + " am Zug");
                }
                valid = false;
                while (!valid)
                {
                    if (output) { drawing.Draw_Board(board); }
                    move = Player[player].move(new Board(board), player);
                    valid = Check_Move(move, player);
                    if ((Player[player].is_cpu) && (!valid)) 
                    { 
                        Console.WriteLine("CPU invalid move");
                        return -2; 
                    }
                }
                if (Player[player].is_cpu && output) { drawing.wait(20000); }
                Perform_Move(move, player);
                //next player
                player = 1 - player;
            }
            if (reversible_moves < 30)  //game ended in a win/loss
            {
                if (output)
                {
                    Console.WriteLine(Player[1 - player].name + " hat gewonnen");
                    drawing.labelText(Player[1 - player].name + " hat gewonnen");
                }
                return (1 - player);
            }
            else    
            {
                if (output)
                {
                    Console.WriteLine("Unentschieden!");
                    drawing.labelText("Unentschieden");
                }
                return -1;
            }
        }

        public void simulate(int repeats)
        {
            drawing.labelText("Simulating...");
            Dictionary<int, int> results = new Dictionary<int, int>();
            results[1] = 0;     //weiß    gewinnt
            results[0] = 0;     //schwarz gewinnt
            results[-1] = 0;    //unentschieden
            results[-2] = 0;    //ungültiger Zug
            string output;
            for (int i = 0; i < repeats; i++)
            {
                results[run(false)]++;
            }
            output = "schwarz: " + results[0].ToString() +
                     "\nweiß: " + results[1].ToString() +
                     "\nunentschieden: " + results[-1].ToString();
            if (results[-2] != 0) { output += "\n invalide: " + results[-2].ToString(); }
            drawing.labelText(output);
        }
    }
}