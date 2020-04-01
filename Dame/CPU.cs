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

        
        private Form1 drawing;
        //Konstruktor
        public CPU(Form1 form)
        {
           drawing = form;
        }


        public string get_move(Board current_Board, int player)               // int = 0 => Schwarz (unten), fängt an
        {
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

                valid = valid.Concat(checkposition(position, player)).ToList();
            }

            for(int i = 0; i < valid.Count; i++) 
               Console.WriteLine(valid[i]);



            return final_move;
        }

        private List<string> checkposition(KeyValuePair<Piece, char> position, int player) // Listet alle möglichen Züge + gibt nur valide Züge zurück
        {
            //Liste aller theoretisch möglichen Züge eines Spielsteins
            List<Piece> possible = new List<Piece>();
            
            {

                //vorerst mögliche diagonalen
                possible.Add(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 + 1));
                possible.Add(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 - 1));
                possible.Add(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 - 1));
                possible.Add(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 + 1));
            }

            //Invalide moves Löschen
            return deleteInvalid(possible, position);
        }

        private List<string> deleteInvalid(List<Piece> possible, KeyValuePair<Piece, char> position) //Löscht invalide Züge und gibt valide zurück
        {
            List<string> valid = new List<string>();

            foreach (Piece option in possible)
            {
                //Zug ist invalid                          

                //Spielzug ausßerhalb Spielfeld
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                // normale Steine nur vorwärts
                if (position.Value == 'b') { if (position.Key.Item2 - 1 == option.Item2) { continue; } }
                if (position.Value == 'w') { if (position.Key.Item2 + 1 == option.Item2) { continue; } }

                //Feld bereits belegt
                if (Board[option] != '.') { continue; }

                valid.Add(drawing.TupleToString(position.Key) + drawing.TupleToString(option));
            }

            return valid;
        }
    }
}