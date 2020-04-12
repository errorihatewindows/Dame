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
            50,     // Wert eigener Stein 50
            100,    // Wert eigene Dame 100
            -20,    // Wert gegnerischer Stein -20
            -80,    // Wert gegnerische Dame -80
            -2,     // Distance Faktor Dame-Stein -2
            -40,   // Bewertung gegnerischer Sprunganzahl -40
            5,     // Zug der einen eigenen Sprung ermöglicht 5
            10     // Berwertung Anzahl der Steine in Königsreihe 10       
         
        };



        public void set_weights(double[] new_weights)
        {
            adjustable_weights = new_weights;
        }

        private Board Board;
        string final_move;

        int wishdepth = 3;

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
            Board = new Board(current_Board);

            tempmove = new List<string>();
            tempjump = new List<string>();

            //wählt den besten move aus
            negaMax(current_Board, 0, player);

            return final_move;
        }


        //Minimax ALogorithmus
        private double max(int spieler, int tiefe, Board board)
        {
            List<string> valid = new List<string>();

            if (tiefe == 0)
                return calcuteBoard_Value(board, spieler);

            double maxWert = double.NegativeInfinity;
            valid = getAllValid(board, spieler);
             if (valid.Count != 0)
             {
                foreach (string move in valid)
                {
                    Board tempboard = update_Board(move, board, spieler);
                    double wert = min(1 - spieler, tiefe - 1, tempboard);
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

            if (tiefe == 0)
                return calcuteBoard_Value(board, 1- spieler);
            double minWert = double.PositiveInfinity;
            valid = getAllValid(board, spieler);
            if (valid.Count != 0) {
                foreach (string move in valid)
                {
                    Board tempboard = update_Board(move, board, spieler);
                    double wert = max(1 - spieler, tiefe - 1, tempboard);
                    if (wert < minWert)
                    {
                        minWert = wert;
                    }
                }
            }
            return minWert;
        }


        private double negaMax(Board board, int depth, int player)
        {          
            if (depth == wishdepth)
                return calcuteBoard_Value(board, player);
            List<string> valid = getAllValid(board, player);
            double maxWert = double.NegativeInfinity;
            foreach (string move in valid)
            {
                Board tempboard = update_Board(move, board, player);
                double value = -negaMax(tempboard, depth + 1, 1 - player);

                if (value > maxWert)
                {
                    maxWert = value;
                    if (depth == 0)
                        final_move = move;
                }

            }

            return maxWert;
        }

        //erstellt eine Liste aller Validen Züge
        private List<string> getAllValid(Board board, int player)
        {
            List<string> valid = new List<string>();
            //Liste aller validen Züge
            tempjump.Clear();
            tempmove.Clear();

            foreach (KeyValuePair<Piece, char> position in board)
            {
                //Steinfarbe passt nicht zu Computerfarbe
                if (((player == 0) && (position.Value != 'b' && position.Value != 'B')) || ((player == 1 && (position.Value != 'w' && position.Value != 'W'))))
                    continue;

                //erstellt Liste tempjump, tempmove für eigene Steine
                checkposition(position, board, player);
            }


            if (tempjump.Count == 0)
            {
                valid = new List<string>(tempmove);
            }
            else
            {   //Überprüft Mehrfachsprung und gibt Liste aller validen Sprünge zurück (korrekte Syntax)
                foreach (string jump in tempjump)
                    valid = valid.Concat(jumps(drawing.StringToTuple(jump), player)).ToList();
            }

            return valid;
        }

        // Checkt jede Position nach Move oder Sprung
        private void checkposition(KeyValuePair<Piece, char> position, Board board, int player) 
        {
            //Liste aller theoretisch möglichen Züge eines Spielsteins
            List<Piece> possiblemove = possible_moves(position.Key);
            List<Piece> possiblejump = possible_jumps(position.Key);

            //Invalide Züge löschen
            tempmove = tempmove.Concat(deleteInvalid_move(possiblemove, position.Key, board, player)).ToList();
            tempjump = tempjump.Concat(deleteInvalid_jump(possiblejump, position.Key, board, player)).ToList();
            
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
        private List<string> deleteInvalid_move(List<Piece> possiblemove, Piece position, Board board, int player) //Löscht invalide Züge und gibt valide zurück
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
        private List<string> deleteInvalid_jump(List<Piece> possiblejump, Piece position, Board board, int Color)
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
                if ((Color == 0)   &&   ((board[Tuple.Create(x, y)] == 'b') || (board[Tuple.Create(x, y)] == 'B'))) { continue; }
                if ((Color == 1)   &&   ((board[Tuple.Create(x, y)] == 'w') || (board[Tuple.Create(x, y)] == 'W'))) { continue; }


                validjump.Add(drawing.TupleToString(position) + "," +  drawing.TupleToString(option));
            }

            return validjump;
        }
        
        //findet alle kompletten Sprungketten an gegebener Position
        private List<string> jumps(Piece position, int Color)
        {
            List<string> valid = new List<string>();
            List<string> output = new List<string>();
            List<Piece> possiblejumps = possible_jumps(position);
            Piece target;
            //save this recursion levels boardstate
            Board currentboard = new Board(Board);

            //Stop condition of recursion
            //if no valid jumps are possible, return a string with only this position
            if (deleteInvalid_jump(possiblejumps,position, currentboard, Color).Count == 0)
            {
                valid.Add(drawing.TupleToString(position));
                return valid;
            }
            //else get the full jump of every possible option at this position
            else
            {
                foreach (string jump in deleteInvalid_jump(possiblejumps, position, currentboard, Color))
                {
                    target = drawing.StringToTuple(jump[3].ToString() + jump[4].ToString());
                    update_Board(jump, Color);
                    valid = valid.Concat(jumps(target, Color)).ToList(); 
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
        private void update_Board(string Move, int Color)
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
        private Board update_Board(string Move, Board board, int Color)
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
                for (int i = 0; i < ((Move.Length + 1) / 3) - 1; i++)
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
                if (Color == 0) { tempBoard[positionnew] = 'B'; }
                if (Color == 1) { tempBoard[positionnew] = 'W'; }
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
                        int Distance = calculateDistance(kvp.Key, board, player);
                        if (Distance > 3)
                            Value += Math.Abs(3 - Distance) * adjustable_weights[4];
                    }                        
                    if (kvp.Value == 'w') { Value += adjustable_weights[2]; }                       
                    if (kvp.Value == 'W') { Value += adjustable_weights[3]; }                 
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
                        int Distance = calculateDistance(kvp.Key, board, player);
                        if (Distance > 3)
                            Value += Math.Abs(3 - Distance) * adjustable_weights[4];
                    }                        
                }
            }

            //Bewege Steine aus der Damenreihe eher seltener (Damen werden außenvor gelassen)
            Value += (count_Pieces_on_Baseline(board, player)) * adjustable_weights[7];

            return Value;
        }

        //Zählt für gegebenes Board die Steine auf der Königsreihe
        private int count_Pieces_on_Baseline(Board board, int player)
        {
            int amount = 0;

            if (player == 0)
            {
                //Damen werden außenvor gelassen
                for(int i = 0; i <= 6; i += 2)
                {
                    if (board[Tuple.Create(i, 0)] == 'b')
                        amount++;
                }
            }

            if (player == 1)
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
        private int calculateDistance(Piece Dameposition, Board board, int Color)
        {
            int DistanceMoves = 8, tempMovedistance;

            foreach (KeyValuePair<Piece,char> kvp in board)
            {
                if (Color == 0 && (kvp.Value == 'w' || kvp.Value == 'W') || (Color == 1 && (kvp.Value == 'b' || kvp.Value == 'B')))
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