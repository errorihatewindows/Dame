﻿using System;
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

        private double[] adjustable_weights = new double[4] {
            50,     // Wert eigener Stein 50
            300,    // Wert eigene Dame 100
            -50,    // Wert gegnerischer Stein -20
            -300,    // Wert gegnerische Dame -80      
        };



        public void set_weights(double[] new_weights)
        {
            adjustable_weights = new_weights;
        }

        string final_move;

        int wishdepth = 8;
        //letzten 6 EIGENEN züge
        private List<string> history = new List<string>();

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
            tempmove = new List<string>();
            tempjump = new List<string>();
            final_move = "";

            //wählt den besten move aus, 
            PlayerA(current_Board, 0, player, double.PositiveInfinity);
            //wenn der move nicht geupdated wurde führen alle züge zu verlust
            if (final_move == "") { final_move = getAllValid(current_Board, player)[0]; }
            //update history
            history.Add(final_move);
            if (history.Count > 6) { history.RemoveAt(0); }
            //increase search depth
            return final_move;
        }

        //Minimax ALogorithmus
        private double PlayerA(Board board, int tiefe, int spieler, double beta)
        {
            List<string> valid = new List<string>();

            if (tiefe == wishdepth)
                return calcuteBoard_Value(board, spieler);

            double alpha = double.NegativeInfinity;
            valid = getAllValid(board, spieler);
             if (valid.Count != 0)
             {
                valid = Heuristic(valid, board, spieler);
                foreach (string move in valid)
                {
                    Board tempboard = update_Board(move, board, spieler, true);
                    double wert = PlayerB(tempboard, tiefe + 1, 1 - spieler,alpha);
                    //beta cutoff
                    if (wert >= beta)
                    {
                        if (tiefe == 0)
                            final_move = move;
                        return beta;
                    }
                    if (wert > alpha)
                    {
                        alpha = wert;
                        if (tiefe == 0)
                            final_move = move;
                    }

                }
            }
                                                      
            return alpha;
        }
      
        private double PlayerB(Board board, int tiefe, int spieler,double alpha)
        {
            List<string> valid = new List<string>();

            if (tiefe == wishdepth)
                return calcuteBoard_Value(board, 1- spieler);
            double beta = double.PositiveInfinity;
            valid = getAllValid(board, spieler);
            if (valid.Count != 0) {
                valid = Heuristic(valid, board, spieler);
                foreach (string move in valid)
                { 
                    Board tempboard = update_Board(move, board, spieler, true);
                    double wert = PlayerA(tempboard, tiefe + 1, 1 - spieler,beta);
                    //alpha cutoff
                    if (wert <= alpha) return alpha;
                    if (wert < beta)
                    {
                        beta = wert;
                    }
                }
            }
            return beta;
        }

        //Sortiert eine Liste mit Zügen nach einer Heuristik
        private List<string> Heuristic(List<string> valid, Board board, int player)
        {
            //output builds from best to worst, reverse from worst to best
            List<string> reverse = new List<string>();
            List<string> output = new List<string>();
            List<string> False = new List<string>();

            //keinen Zug der gemerkten History wiederholen
            foreach (string move in valid)
            {
                if (history.Contains(move)) { reverse.Insert(0, move); }
                else { False.Add(move); }
            }
            valid = False;
            False = new List<string>();
            //Dame bewegen
            foreach (string move in valid)
            {
                if (board[drawing.StringToTuple(move.Substring(0, 2))] == Convert.ToChar('B'+21*player)) { output.Add(move); }
                else { False.Add(move); }
            }
            valid = False;
            False = new List<string>();
            //keine steine aus der grundreihe
            foreach (string move in valid)
            {
                if (move[1] == Convert.ToChar('1'+8*player)) { reverse.Insert(0, move); }
                else { False.Add(move); }
            }
            valid = False;
            False = new List<string>();
            //moves that dont apply to any conditions are insertet between output and reverse
            output = output.Concat(valid).ToList();
            output = output.Concat(reverse).ToList();
            return output;
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
            } else
            {   //Überprüft Mehrfachsprung und gibt Liste aller validen Sprünge zurück (korrekte Syntax)
                foreach (string jump in tempjump)
                {
                    valid = valid.Concat(jumps(drawing.StringToTuple(jump), player, board)).ToList();

                }
                    
            }

            return valid;
        }

        // Checkt jede Position nach Move oder Sprung
        private void checkposition(KeyValuePair<Piece, char> position, Board board, int player) 
        {
            //Invalide Züge löschen
            tempmove = tempmove.Concat(deleteInvalid_move(possible_moves(position.Key), position.Key, board, player)).ToList();
            tempjump = tempjump.Concat(deleteInvalid_jump(possible_jumps(position.Key), position.Key, board, player)).ToList();
            
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
        private List<string> jumps(Piece position, int Color, Board board)
        {
            List<string> valid = new List<string>();
            List<string> output = new List<string>();
            List<Piece> possiblejumps = possible_jumps(position);
            Piece target;

            //Stop condition of recursion
            //if no valid jumps are possible, return a string with only this position
            if (deleteInvalid_jump(possiblejumps,position, board, Color).Count == 0)
            {
                valid.Add(drawing.TupleToString(position));
                return valid;
            }
            //else get the full jump of every possible option at this position
            else
            {
                foreach (string jump in deleteInvalid_jump(possiblejumps, position, board, Color))
                {
                    target = drawing.StringToTuple(jump[3].ToString() + jump[4].ToString());
                    Board tempboard = update_Board(jump, board, Color, false);
                    valid = valid.Concat(jumps(target, Color, tempboard)).ToList(); 
 
                }
            }
            //add ur position 
            foreach (string move in valid)
            {
                output.Add(drawing.TupleToString(position) + "," + move);
            }

            return output;

        }

        //Führt einen gegebenen Move oder Sprung auf TEMPBOARD aus und gibt diese zurück
        private Board update_Board(string Move, Board board, int Color, bool Dame)
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

            if (Dame)
            {
                //Damen setzten wenn Move auf Königsreihe führt
                if (positionnew.Item2 == 0 || positionnew.Item2 == 7)
                {
                    if (Color == 0) { tempBoard[positionnew] = 'B'; }
                    if (Color == 1) { tempBoard[positionnew] = 'W'; }
                }
            }

            

            return tempBoard;

        }

        //berwertet Boards für schwarz und weiß
        public double calcuteBoard_Value(Board board, int player)
        {
            double Value = 0;

            //ANzahl der Steine - Bewertung
            foreach (KeyValuePair<Piece, char> kvp in board)
            {
                //CPU ist Schwarz
                if (player == 0)
                {
                    if (kvp.Value == 'b') { Value += adjustable_weights[0]; }
                    if (kvp.Value == 'B') { Value += adjustable_weights[1]; }                 
                    if (kvp.Value == 'w') { Value += adjustable_weights[2]; }                       
                    if (kvp.Value == 'W') { Value += adjustable_weights[3]; }                 
                } 
                // CPU ist weiß
                else if (player == 1)
                {
                    if (kvp.Value == 'b') { Value += adjustable_weights[2]; }
                    if (kvp.Value == 'B') { Value += adjustable_weights[3]; }                        
                    if (kvp.Value == 'w') { Value += adjustable_weights[0]; }                        
                    if (kvp.Value == 'W') { Value += adjustable_weights[1]; }
                }
            }
            return Value;
        }

    }
}