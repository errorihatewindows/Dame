using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Piece = System.Tuple<int, int>;
using Board = System.Collections.Generic.Dictionary<System.Tuple<int, int>, char>;

namespace Dame
{
    public class CPU
    {

        private double[] adjustable_weights = new double[8] {
            42.099,     // Wert eigener Stein 50
            117.009,    // Wert eigene Dame 100
            -19.5367,    // Wert gegnerischer Stein -20
            -66.34856,    // Wert gegnerische Dame -80
            -2.722,     // Distance Faktor Dame-Stein -2
            -42.458,    // Bewertung gegnerischer Sprunganzahl -40
            4.92922,      // Zug der einen eigenen Sprung ermöglicht 5
            11.56381     // Berwertung Anzahl der Steine in Königsreihe 10       
         
        };



        public void set_weights(double[] new_weights)
        {
            adjustable_weights = new_weights;
        }

        private Board Board;
        private int ComputerColor;
        string final_move = "";

        int wishdepth = 2;

        //temporäre Listen          
        private List<string> tempmove = new List<string>();
        private List<string> tempjump = new List<string>();

        private Form1 drawing;

        //Konstruktor
        public CPU(Form1 form)
        {
            drawing = form;
        }

        //gibt den Finalen Zug zurück
        public string get_move(Board current_Board, int player)
        {
            ComputerColor = player;
            Board = new Board(current_Board);

            tempmove = new List<string>();
            tempjump = new List<string>();

            //wählt den besten move aus
            max(ComputerColor, wishdepth, current_Board);

            return final_move;
        }







        private double max(int spieler, int tiefe, Board board)
        {
            List<string> valid = new List<string>();

            if (tiefe == 0 || (count_own_jumps(board) + count_own_move(board) == 0))
                return calcuteBoard_Value(board, spieler);

            double maxWert = double.NegativeInfinity;
            valid = getAllValid(board, spieler);
             if (valid.Count != 0)
             {
                foreach (string move in valid)
                {
                    Board boardbefore = new Board(board);
                    Board tempboard = update_Board(move, board);
                    double wert = min(1 - spieler, tiefe - 1, tempboard);
                    board = new Board(boardbefore);
                    if (wert > maxWert)
                    {
                        maxWert = wert;
                        if (tiefe == wishdepth)
                            final_move = move;
                    }
                }
            }
                
                              
                
            return maxWert;
        }
        private double min(int spieler, int tiefe, Board board)
        {
            List<string> valid = new List<string>();

            if (tiefe == 0 || (count_opponent_jumps(board) + count_opponent_move(board) == 0))
                return calcuteBoard_Value(board, 1 - spieler);
            double minWert = double.PositiveInfinity;
            valid = getAllValid(board, spieler);
            if (valid.Count != 0) {
                foreach (string move in valid)
                {
                    Board boardbefore = new Board(board);
                    Board tempboard = update_Board(move, board);
                    double wert = max(1 - spieler, tiefe - 1, tempboard);
                    board = new Board(boardbefore);
                    if (wert < minWert)
                    {
                        minWert = wert;
                    }
                }
            }
            return minWert;
        }


        //erstellt eine Liste aller Validen Züge
        private List<string> getAllValid(Board board, int player)
        {
            //Liste aller validen Züge
            List<string> valid = new List<string>();

            foreach (KeyValuePair<Piece, char> position in board)
            {
                //Steinfarbe passt nicht zu Computerfarbe
                if (((player == 0) && (position.Value != 'b' && position.Value != 'B')) || ((player == 1 && (position.Value != 'w' && position.Value != 'W'))))
                    continue;

                //erstellt Liste tempjump, tempmove für eigene Steine
                checkposition(position, board);
            }


            if (tempjump.Count == 0)
            {
                valid = tempmove;
            }
            else
            {   //Überprüft Mehrfachsprung und gibt Liste aller validen Sprünge zurück (korrekte Syntax)
                foreach (string jump in tempjump)
                    valid = valid.Concat(jumps(drawing.StringToTuple(jump))).ToList();
            }

            return valid;
        }

        // Checkt jede Position nach Move oder Sprung
        private void checkposition(KeyValuePair<Piece, char> position, Board board) 
        {
            //Liste aller theoretisch möglichen Züge eines Spielsteins
            List<Piece> possiblemove = possible_moves(position.Key);
            List<Piece> possiblejump = possible_jumps(position.Key);

            //Invalide Züge löschen
            tempmove = tempmove.Concat(deleteInvalid_move(possiblemove, position.Key, board)).ToList();
            tempjump = tempjump.Concat(deleteInvalid_jump(possiblejump, position.Key, board)).ToList();
            
        }

        //Listet alle theoretisch möglichen EiNFACH-Sprünge auf
        private List<Piece> possible_jumps(Piece position)
        {
            List<Piece> possiblejump = new List<Piece>();
            possiblejump.Add(Tuple.Create(position.Item1 + 2, position.Item2 + 2));
            possiblejump.Add(Tuple.Create(position.Item1 + 2, position.Item2 - 2));
            possiblejump.Add(Tuple.Create(position.Item1 - 2, position.Item2 - 2));
            possiblejump.Add(Tuple.Create(position.Item1 - 2, position.Item2 + 2));
            return possiblejump;
        }

        //Listet alle theoretisch möglichen Moves auf
        private List<Piece> possible_moves(Piece position)
        {
            List<Piece> possiblemove = new List<Piece>();
            possiblemove.Add(Tuple.Create(position.Item1 + 1, position.Item2 + 1));
            possiblemove.Add(Tuple.Create(position.Item1 + 1, position.Item2 - 1));
            possiblemove.Add(Tuple.Create(position.Item1 - 1, position.Item2 - 1));
            possiblemove.Add(Tuple.Create(position.Item1 - 1, position.Item2 + 1));
            return possiblemove;
        }

        //Löscht alle Invaliden Moves aus gegebener Liste
        private List<string> deleteInvalid_move(List<Piece> possiblemove, Piece position, Board board) //Löscht invalide Züge und gibt valide zurück
        {
            List<string> validmove = new List<string>(); 

            foreach (Piece option in possiblemove)
            {
                //Move ist invalid wenn                         

                //Spielzug ausßerhalb Spielfeld
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                // normale Steine nur vorwärts
                if (board[position] == 'b') { if (position.Item2 - 1 == option.Item2) { continue; } }
                if (board[position] == 'w') { if (position.Item2 + 1 == option.Item2) { continue; } }

                //Feld bereits belegt
                if (board[option] != '.') { continue; }

                validmove.Add(drawing.TupleToString(position) + "," +  drawing.TupleToString(option));
            }

            return validmove;
        }

        //Löscht alle Invaliden EINFACH-SPrünge aus gegebener Liste
        private List<string> deleteInvalid_jump(List<Piece> possiblejump, Piece position, Board board)
        {
            List<string> validjump = new List<string>();

            foreach (Piece option in possiblejump)
            {
                //Jump ist invalid wenn                         

                //Spielzug ausßerhalb Spielfeld
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                // normale Steine nur vorwärts
                if (board[position] == 'b') { if (position.Item2 - 2 == option.Item2) { continue; } }
                if (board[position] == 'w') { if (position.Item2 + 2 == option.Item2) { continue; } }

                //Feld bereits belegt
                if (board[option] != '.') { continue; }

                //Berechne Feld das übersprungen wird

                int x = (position.Item1 + option.Item1) / 2;
                int y = (position.Item2 + option.Item2) / 2;

                //leeres Feld zum Überspringen
                if (board[Tuple.Create(x,y)] == '.') { continue; }
                
                //übersprungener Stein ist eigene Farbe
                if ((ComputerColor == 0)   &&   ((board[Tuple.Create(x, y)] == 'b') || (board[Tuple.Create(x, y)] == 'B'))) { continue; }
                if ((ComputerColor == 1)   &&   ((board[Tuple.Create(x, y)] == 'w') || (board[Tuple.Create(x, y)] == 'W'))) { continue; }


                validjump.Add(drawing.TupleToString(position) + "," +  drawing.TupleToString(option));
            }

            return validjump;
        }
        
        //findet alle kompletten Sprungketten an gegebener Position
        private List<string> jumps(Piece position)
        {
            List<string> valid = new List<string>();
            List<string> output = new List<string>();
            List<Piece> possiblejumps = possible_jumps(position);
            Piece target;
            //save this recursion levels boardstate
            Board currentboard = new Board(Board);

            //Stop condition of recursion
            //if no valid jumps are possible, return a string with only this position
            if (deleteInvalid_jump(possiblejumps,position, currentboard).Count == 0)
            {
                valid.Add(drawing.TupleToString(position));
                return valid;
            }
            //else get the full jump of every possible option at this position
            else
            {
                foreach (string jump in deleteInvalid_jump(possiblejumps, position, currentboard))
                {
                    target = drawing.StringToTuple(jump[3].ToString() + jump[4].ToString());
                    update_Board(jump);
                    valid = valid.Concat(jumps(target)).ToList(); 
                    Board = new Board(currentboard);
                }
            }
            //add ur position 
            foreach (string move in valid)
            {
                output.Add(drawing.TupleToString(position) + "," + move);
            }

            return output;

        }




        //Führt einen gegebenen Move oder Sprung aus auf dem TempBoard der CPU
        private void update_Board(string Move)
        {
            Piece positionold = new Piece(0, 0);
            Piece positionnew = new Piece(0,0);

            //wenn das Update einen normalen Move enthält
            if (Move[1] + 1 == Move[Move.Length - 1] || Move[1] -1 == Move[Move.Length - 1])
            {
                positionold = drawing.StringToTuple(Move[0].ToString() + Move[1].ToString());
                positionnew = drawing.StringToTuple(Move[3].ToString() + Move[4].ToString());

                //neuen Stein setzten
                Board[positionnew] = Board[positionold];
                //alten Stein entfernen
                Board[positionold] = '.';

            } 
            
            //Wenn das update einen Sprung der Länge x enthält
            else
            {          
                //für Anzahl an Updates 
                for (int i = 0; i < ((Move.Length + 1) / 3) - 1; i++)
                {
                    positionold = drawing.StringToTuple((Move[(i * 3)].ToString() + Move[(i * 3) + 1].ToString()));
                    positionnew = drawing.StringToTuple((Move[(i * 3) + 3].ToString() + Move[(i * 3) + 4].ToString()));

                    //neuen Stein setzten
                    Board[positionnew] = Board[positionold];

                    //Übersprungenen Stein entfernen
                    Piece positionCaptured = new Piece((positionold.Item1 + positionnew.Item1) / 2, (positionold.Item2 + positionnew.Item2) / 2);
                    Board[positionCaptured] = '.';

                    //Alte Position updaten
                    Board[positionold] = '.';

                }
            }
        }

        //Führt einen gegebenen Move oder Sprung auf TEMPBOARD aus und gibt diese zurück
        private Board update_Board(string Move, Board board)
        {
            //temporäres Board auf currentBoard state setzen
            Board tempBoard = new Board(board);

            Piece positionold = new Piece(0, 0);
            Piece positionnew = new Piece(0, 0);

            //wenn das Update einen normalen Move enthält
            if (Move[1] + 1 == Move[Move.Length - 1] || Move[1] - 1 == Move[Move.Length - 1])
            {
                positionold = drawing.StringToTuple(Move[0].ToString() + Move[1].ToString());
                positionnew = drawing.StringToTuple(Move[3].ToString() + Move[4].ToString());

                //neuen Stein setzten
                tempBoard[positionnew] = tempBoard[positionold];
                //alten Stein entfernen
                tempBoard[positionold] = '.';

            }

            //Wenn das update einen Sprung der Länge x enthält 
            else
            {
                //für Anzahl an Updates 
                for (int i = 0; i < ((Move.Length + 1) % 3) - 1; i++)
                {
                    positionold = drawing.StringToTuple((Move[(i * 3)].ToString() + Move[(i * 3) + 1].ToString()));
                    positionnew = drawing.StringToTuple((Move[(i * 3) + 3].ToString() + Move[(i * 3) + 4].ToString()));

                    //neuen Stein setzten

                    tempBoard[positionnew] = tempBoard[positionold];

                    //Übersprungenen Stein entfernen
                    Piece positionCaptured = new Piece((positionold.Item1 + positionnew.Item1) / 2, (positionold.Item2 + positionnew.Item2) / 2);
                    tempBoard[positionCaptured] = '.';

                    //Alte Position updaten
                    tempBoard[positionold] = '.';

                }
            }

            //Damen setzten wenn Move auf Königsreihe führt
            if (positionnew.Item2 == 0 || positionnew.Item2 == 7)
            {
                if (ComputerColor == 0) { tempBoard[positionnew] = 'B'; }
                if (ComputerColor == 1) { tempBoard[positionnew] = 'W'; }
            }

            return tempBoard;

        }

        //berwertet Boards für schwarz und weiß
        private double calcuteBoard_Value(Board board, int player)
        {
            double Value = 0;

            //ANzahl der Steine - Bewertung
            foreach (KeyValuePair<Piece, char> kvp in board)
            {
                //CPU ist Schwarz
                if (player == 0)
                {
                    if (kvp.Value == 'b') { Value += adjustable_weights[0]; }                       
                    if (kvp.Value == 'B') 
                    { 
                        Value += adjustable_weights[1];
                        int Distance = calculateDistance(kvp.Key, board);
                        if (Distance > 3)
                            Value += Math.Abs(3 - Distance) * adjustable_weights[4];
                    }                        
                    if (kvp.Value == 'w') { Value += adjustable_weights[2]; }                       
                    if (kvp.Value == 'W') { Value += adjustable_weights[2]; }                 
                } 
                // CPU ist weiß
                else if (player == 1)
                {
                    if (kvp.Value == 'b') { Value += adjustable_weights[2]; }
                    if (kvp.Value == 'B') { Value += adjustable_weights[3]; }                        
                    if (kvp.Value == 'w') { Value += adjustable_weights[0]; }                        
                    if (kvp.Value == 'W') 
                    { 
                        Value += adjustable_weights[1];
                        int Distance = calculateDistance(kvp.Key, board);
                        if (Distance > 3)
                            Value += Math.Abs(3 - Distance) * adjustable_weights[4];
                    }                        
                }
            }

            //Anzahl gegnerischer Sprünge bewerten
            Value += count_opponent_jumps(board) * adjustable_weights[5];

            //bevorzuge den Zug bei dem DU danach springen kannst 
            Value += count_own_jumps(board) * adjustable_weights[6];

            //Bewege Steine aus der Damenreihe eher seltener (Damen werden außenvor gelassen)
            Value += (count_Pieces_on_Baseline(board)) * adjustable_weights[7];

            //Gegner kann sich nicht mehr bewegen
            if (count_opponent_jumps(board) + count_opponent_move(board) == 0)
                Value += 200000.0;

                


            return Value;
        }

        //Anzahl der Einfachsprünge des Gegners für gegebenes Board
        private int count_opponent_jumps(Board board)
        {
            //ermittle die Anzahl der gegnerischen möglichen Sprünge
            List<string> tempjumps = new List<string>();
            //temporärer Farbentausch
            ComputerColor = 1 - ComputerColor;

            foreach (KeyValuePair<Piece, char> position in board)
            {
                //Stein hat Gegnerfarbe
                //Anzahl der möglichen Sprungrichtungen eines Steines
                if ((ComputerColor == 0 && (position.Value == 'b' || position.Value == 'B')) || (ComputerColor == 1 && (position.Value == 'w' || position.Value == 'W')))
                    tempjumps = tempjumps.Concat(deleteInvalid_jump(possible_jumps(position.Key), position.Key, board)).ToList();
                
            }

            //Rücktausch
            ComputerColor = 1 - ComputerColor;

            return tempjumps.Count;
        }

        //Anzahl der eigener Einfachsprünge für gegebenes Board
        private int count_own_jumps(Board board)
        {
            //ermittle die Anzahl der gegnerischen möglichen Sprünge
            List<string> tempjumps = new List<string>();

            foreach (KeyValuePair<Piece, char> position in board)
            {
                //Stein hat Gegnerfarbe
                //Anzahl der möglichen Sprungrichtungen eines Steines
                if ((ComputerColor == 0 && (position.Value == 'b' || position.Value == 'B')) || (ComputerColor == 1 && (position.Value == 'w' || position.Value == 'W')))
                    tempjumps = tempjumps.Concat(deleteInvalid_jump(possible_jumps(position.Key), position.Key, board)).ToList();

            }

            return tempjumps.Count;
        }

        private int count_opponent_move(Board board)
        {
            List<string> tempmoves = new List<string>();

            //temporärer Farbentausch
            ComputerColor = 1 - ComputerColor;

            foreach (KeyValuePair<Piece, char> kvp in board)
            {
                if ((ComputerColor == 0 && (kvp.Value == 'b' || kvp.Value == 'B')) || (ComputerColor == 1 && (kvp.Value == 'w' || kvp.Value == 'W')))
                    tempmoves = tempmoves.Concat(deleteInvalid_move(possible_moves(kvp.Key), kvp.Key, board)).ToList();
            }

            //Rücktausch 
            ComputerColor = 1 - ComputerColor;

            return tempmoves.Count;
        }
        
        private int count_own_move(Board board)
        {
            List<string> tempmoves = new List<string>();

            foreach (KeyValuePair<Piece, char> kvp in board)
            {
                if ((ComputerColor == 0 && (kvp.Value == 'b' || kvp.Value == 'B')) || (ComputerColor == 1 && (kvp.Value == 'w' || kvp.Value == 'W')))
                    tempmoves = tempmoves.Concat(deleteInvalid_move(possible_moves(kvp.Key), kvp.Key, board)).ToList();
            }

            return tempmoves.Count;
        }

        //Zählt für gegebenes Board die Steine auf der Königsreihe
        private int count_Pieces_on_Baseline(Board board)
        {
            int amount = 0;

            if (ComputerColor == 0)
            {
                //Damen werden außenvor gelassen
                for(int i = 0; i <= 6; i += 2)
                {
                    if (board[Tuple.Create(i, 0)] == 'b')
                        amount++;
                }
            }

            if (ComputerColor == 1)
            {
                for (int i = 1; i <= 7; i += 2)
                {
                    if (board[Tuple.Create(i, 7)] == 'w')
                        amount++;
                }
            }

            return amount;
        }

        //Berechnet den Abstand einer Dame zum nächsten Gegnerstein
        private int calculateDistance(Piece Dameposition, Board board)
        {
            int DistanceMoves = 8, tempMovedistance;

            foreach (KeyValuePair<Piece,char> kvp in board)
            {
                if (ComputerColor == 0 && (kvp.Value == 'w' || kvp.Value == 'W') || (ComputerColor == 1 && (kvp.Value == 'b' || kvp.Value == 'B')))
                {
                    if (Math.Abs(Dameposition.Item1 - kvp.Key.Item1) > Math.Abs(Dameposition.Item2 - kvp.Key.Item2))
                        tempMovedistance = Math.Abs(Dameposition.Item1 - kvp.Key.Item1);
                    else
                        tempMovedistance = Math.Abs(Dameposition.Item2 - kvp.Key.Item2);

                    if (tempMovedistance < DistanceMoves)
                        DistanceMoves = tempMovedistance;

                }
                
            }

            return DistanceMoves;
        }
    }
}