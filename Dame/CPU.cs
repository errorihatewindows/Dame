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

        private Board Board;
        private int ComputerColor;

        //temporäre Listen          
        private List<string> tempmove = new List<string>();
        private List<string> tempjump = new List<string>();

        private Form1 drawing;

        //Konstruktor
        public CPU(Form1 form)
        {
            drawing = form;
        }


        public string get_move(Board current_Board, int player)               // gibt validen move und einfachen SPrung zurück
        {
            ComputerColor = player;
            Board = new Board(current_Board);
            tempmove = new List<string>();
            tempjump = new List<string>();
            //Liste aller validen Züge
            List<string> valid = new List<string>();
            //Finaler move
            string final_move;


            foreach (KeyValuePair<Piece, char> position in Board)
            {
                //Steinfarbe passt nicht zu Computerfarbe
                if (((player == 0) && (position.Value != 'b' && position.Value != 'B')) || ((player == 1 && (position.Value != 'w' && position.Value != 'W'))))
                    continue;

                checkposition(position, Board);
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

            //wählt den besten move aus
            final_move = get_best_move(valid, current_Board);


            return final_move;
        }









        // Listet alle möglichen Züge + gibt nur valide Züge zurück
        private void checkposition(KeyValuePair<Piece, char> position, Board board) 
        {
            //Liste aller theoretisch möglichen Züge eines Spielsteins
            List<Piece> possiblemove = possible_moves(position.Key);
            List<Piece> possiblejump = possible_jumps(position.Key);

            //Invalide Züge löschen
            tempmove = tempmove.Concat(deleteInvalid_move(possiblemove, position.Key)).ToList();
            tempjump = tempjump.Concat(deleteInvalid_jump(possiblejump, position.Key, board)).ToList();
            
        }

        private List<Piece> possible_jumps(Piece position)
        {
            List<Piece> possiblejump = new List<Piece>();
            possiblejump.Add(Tuple.Create(position.Item1 + 2, position.Item2 + 2));
            possiblejump.Add(Tuple.Create(position.Item1 + 2, position.Item2 - 2));
            possiblejump.Add(Tuple.Create(position.Item1 - 2, position.Item2 - 2));
            possiblejump.Add(Tuple.Create(position.Item1 - 2, position.Item2 + 2));
            return possiblejump;
        }

        private List<Piece> possible_moves(Piece position)
        {
            List<Piece> possiblemove = new List<Piece>();
            possiblemove.Add(Tuple.Create(position.Item1 + 1, position.Item2 + 1));
            possiblemove.Add(Tuple.Create(position.Item1 + 1, position.Item2 - 1));
            possiblemove.Add(Tuple.Create(position.Item1 - 1, position.Item2 - 1));
            possiblemove.Add(Tuple.Create(position.Item1 - 1, position.Item2 + 1));
            return possiblemove;
        }

        private List<string> deleteInvalid_move(List<Piece> possiblemove, Piece position) //Löscht invalide Züge und gibt valide zurück
        {
            List<string> validmove = new List<string>(); 

            foreach (Piece option in possiblemove)
            {
                //Move ist invalid wenn                         

                //Spielzug ausßerhalb Spielfeld
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                // normale Steine nur vorwärts
                if (Board[position] == 'b') { if (position.Item2 - 1 == option.Item2) { continue; } }
                if (Board[position] == 'w') { if (position.Item2 + 1 == option.Item2) { continue; } }

                //Feld bereits belegt
                if (Board[option] != '.') { continue; }

                validmove.Add(drawing.TupleToString(position) + "," +  drawing.TupleToString(option));
            }

            return validmove;
        }

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
        private int calcuteBoard_Value(Board board, int opponentjumpsbefore, int opponentjumpsafter, int ownjumpsbefore, int ownjumpsafter, int NumBefore, int NumAfter)
        {
            int Value = 0;

            //ANzahl der Steine - Bewertung
            foreach (KeyValuePair<Piece, char> kvp in board)
            {
                //CPU ist Schwarz
                if (ComputerColor == 0)
                {
                    if (kvp.Value == 'b') { Value += 50; }                       
                    if (kvp.Value == 'B') 
                    { Value += 100;
                        int Distance = calculateDistance(kvp.Key, board);
                        if (Distance > 3)
                            Value += (3 - Distance) * 2;
                    }                        
                    if (kvp.Value == 'w') { Value -= 50; }                       
                    if (kvp.Value == 'W') { Value -= 100; }                 
                } 
                // CPU ist weiß
                else if (ComputerColor == 1)
                {
                    if (kvp.Value == 'b') { Value -= 50; }
                    if (kvp.Value == 'B') { Value -= 100; }                        
                    if (kvp.Value == 'w') { Value += 50; }                        
                    if (kvp.Value == 'W') 
                    { 
                        Value += 100;
                        int Distance = calculateDistance(kvp.Key, board);
                        if (Distance > 3)
                            Value += (3 - Distance) * 2;
                    }                        
                }
            }

            //Anzahl gegnerischer Sprünge bewerten
            Value += (opponentjumpsbefore - opponentjumpsafter) * 40;

            //Wenn keine Gegnersprünge verhindert werden können bevorzuge den Zug bei dem DU danach springen kannst wenn du somit keinen Gegnersprung ermöglichst
            Value += (ownjumpsafter - ownjumpsbefore) * 10;

            //Bewege Steine aus der Damenreihe eher seltener (Damen werden außenvor gelassen)
            Value += (NumBefore - NumAfter) * -10;


            Console.Write(" = " + Value + " , " + (opponentjumpsafter - opponentjumpsbefore) + " , " + (ownjumpsafter - ownjumpsbefore));
            Console.WriteLine();
  

            return Value;
        }

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

        private int count_Pieces_on_Base(Board board)
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

        private string get_best_move(List<string> valid, Board board)
        {
            Console.WriteLine();

            List<string> best_moves = new List<string>();
            int highest_Value = 0, current_Value;

            //Zählt Anzahl der möglichen Gegnersteine und eigene, die Springen können
            int opponentjumpsbefore = count_opponent_jumps(board);
            int ownjumpsbefore = count_own_jumps(board);
            int NumBefore = count_Pieces_on_Base(board);

            //temporäres Board zum ausführen der Züge
            Board tempBoard = board;

            foreach (string move in valid)
            {
                //temporäres board updaten
                tempBoard = update_Board(move, board);

                //Gegner Sprünge nach update zählen
                int opponentjumpsafter = count_opponent_jumps(tempBoard);
                int ownjumpsafter = count_own_jumps(tempBoard);
                int NumAfter = count_Pieces_on_Base(tempBoard);

                Console.Write(move);

                current_Value = calcuteBoard_Value(tempBoard, opponentjumpsbefore, opponentjumpsafter, ownjumpsbefore, ownjumpsafter, NumBefore, NumAfter);

                if ((current_Value == highest_Value) || best_moves.Count == 0)
                {
                    highest_Value = current_Value;
                    best_moves.Add(move);
                }

                if (current_Value > highest_Value)
                {
                    best_moves.Clear();
                    best_moves.Add(move);
                    highest_Value = current_Value;
                }
            }

            Console.WriteLine();
            Console.WriteLine("------" + highest_Value + "------");

            Random Zufall = new Random();
            return best_moves[Zufall.Next(best_moves.Count)];
        }

        //Berechnet den Abstand einer Dame zum nächsten Gegnerstein
        private int calculateDistance(Piece Dameposition, Board board)
        {
            int DistanceMoves = 10, tempMovedistance;

            foreach (KeyValuePair<Piece,char> kvp in Board)
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