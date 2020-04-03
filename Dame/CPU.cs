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

        private bool Complete = false;

        private Form1 drawing;
        
        //Konstruktor
        public CPU(Form1 form)
        {
           drawing = form;
        }


        public string get_move(Board current_Board, int player)               // gibt validen move und einfachen SPrung zurück
        {
            ComputerColor = player;
            Board = current_Board;

            //Liste aller validen Züge
            List<string> valid = new List<string>();

            //Finaler move
            string final_move = "";


            foreach (KeyValuePair <Piece,char> position in Board)
            {
                //Steinfarbe passt nicht zu Computerfarbe
                if (((player == 0) && (position.Value != 'b' && position.Value != 'B')) || ((player == 1 && (position.Value != 'w' && position.Value != 'W'))))
                    continue;

                checkposition(position);

            }


            if (tempjump.Count == 0)
                valid = tempmove;
            else
            {   
                valid = tempjump;
            }


            for (int i = 0; i < valid.Count; i++)
                Console.WriteLine(valid[i]);

            //Zufälligen Valid Move auswählen
            Random Zufall = new Random();
            final_move = valid[Zufall.Next(0, valid.Count)];

            //auf korrekte Syntax bringen
            final_move = final_move.Insert(2, ",");

            return final_move;
        }

        private void checkposition(KeyValuePair<Piece, char> position) // Listet alle möglichen Züge + gibt nur valide Züge zurück
        {
            //Liste aller theoretisch möglichen Züge eines Spielsteins
            List<Piece> possiblemove = new List<Piece>();
            List<Piece> possiblejump = new List<Piece>();

            //vorerst mögliche move diagonalen
            possiblemove.Add(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 + 1));
            possiblemove.Add(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 - 1));
            possiblemove.Add(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 - 1));
            possiblemove.Add(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 + 1));

            //vorerst mögliche jumps
            possiblejump.Add(Tuple.Create(position.Key.Item1 + 2, position.Key.Item2 + 2));
            possiblejump.Add(Tuple.Create(position.Key.Item1 + 2, position.Key.Item2 - 2));
            possiblejump.Add(Tuple.Create(position.Key.Item1 - 2, position.Key.Item2 - 2));
            possiblejump.Add(Tuple.Create(position.Key.Item1 - 2, position.Key.Item2 + 2));


            //Invalide Züge löschen
            tempmove = tempmove.Concat(deleteInvalid_move(possiblemove, position)).ToList();
            tempjump = tempjump.Concat(deleteInvalid_jump(possiblejump, position)).ToList();
            
        }

        private List<string> deleteInvalid_move(List<Piece> possiblemove, KeyValuePair<Piece, char> position) //Löscht invalide Züge und gibt valide zurück
        {
            List<string> validmove = new List<string>(); 

            foreach (Piece option in possiblemove)
            {
                //Move ist invalid wenn                         

                //Spielzug ausßerhalb Spielfeld
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                // normale Steine nur vorwärts
                if (position.Value == 'b') { if (position.Key.Item2 - 1 == option.Item2) { continue; } }
                if (position.Value == 'w') { if (position.Key.Item2 + 1 == option.Item2) { continue; } }

                //Feld bereits belegt
                if (Board[option] != '.') { continue; }

                validmove.Add(drawing.TupleToString(position.Key) + drawing.TupleToString(option));
            }

            return validmove;
        }

        private List<string> deleteInvalid_jump(List<Piece> possiblejump, KeyValuePair<Piece, char> position)
        {
            List<string> validjump = new List<string>();

            foreach (Piece option in possiblejump)
            {
                //Jump ist invalid wenn                         

                //Spielzug ausßerhalb Spielfeld
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                // normale Steine nur vorwärts
                if (position.Value == 'b') { if (position.Key.Item2 - 2 == option.Item2) { continue; } }
                if (position.Value == 'w') { if (position.Key.Item2 + 2 == option.Item2) { continue; } }

                //Feld bereits belegt
                if (Board[option] != '.') { continue; }


                //Dazwischen kein Stein oder eigener Stein
                //Berechne Feld das übersprungen wird

                int x = (position.Key.Item1 + option.Item1) / 2;
                int y = (position.Key.Item2 + option.Item2) / 2;

                //leeres Feld zum Überspringen
                if (Board[Tuple.Create(x,y)] == '.') { continue; }
                
                //übersprungener Stein ist eigene Farbe
                if ((ComputerColor == 0)   &&   ((Board[Tuple.Create(x, y)] == 'b') || (Board[Tuple.Create(x, y)] == 'B'))) { continue; }
                if ((ComputerColor == 1)   &&   ((Board[Tuple.Create(x, y)] == 'w') || (Board[Tuple.Create(x, y)] == 'W'))) { continue; }


                validjump.Add(drawing.TupleToString(position.Key) + drawing.TupleToString(option));
            }

            return validjump;
        } // Löscht alle Invaliden Sprünge

         //private List<string> jumps(Piece position)
        
           /* List<string> valid = new List<string>;
            List<string> output = new List<string>;
            //if no valid jumps are possible, return a string with only this position
            if (valid_jumps.Count == 0)
            {
                valid.Add(drawing.TupleToString(position));
                return valid;
            }
            //else get the full jump of every possible option at this position
            else
            {
                foreach (Piece target in valid_jumps)
                {
                    valid.Concat(jumps(target));
                }
            }
            //add ur position 
            foreach (string move in valid)
            {
                output.Add(drawing.TupleToString(position) + move);
            }
            return output; */
    }





}