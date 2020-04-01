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
        //Liste aller möglichen Outputs
        string final_move;

        
        private Form1 drawing;
        //Konstruktor
        public CPU(Form1 form)
        {
           drawing = form;
        }


        public string get_move(Board Board, int player)               // int = 0 => Schwarz (unten), fängt an
        {
            List<string> valid = new List<string>();

            
            foreach (KeyValuePair <Piece,char> position in Board)
            {
                valid = valid.Concat(checkposition(position, player)).ToList();
            }

            for(int i = 0; i < 10; i++) 
                Console.WriteLine(valid[i]);

            return final_move;
        }

        private List<string> checkposition(KeyValuePair<Piece, char> position, int player)
        {
            List<Piece> possible = new List<Piece>();
            //Computer speichert alle Diagonalen der Steine seiner Farbe
            if (((player == 0) && (position.Value == 'b' || position.Value == 'B')) || ((player == 1 && position.Value == 'w' || position.Value == 'W')))
            {
                Console.WriteLine("YAY");
                //vorerst mögliche diagonalen
                possible.Add(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 + 1));
                possible.Add(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 - 1));
                possible.Add(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 - 1));
                possible.Add(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 + 1));
            }

            //Invalide moves Löschen
            return deleteInvalid(possible,position.Key);
        }

        private List<string> deleteInvalid(List<Piece> possible,Piece position)
        {
            List<string> valid = new List<string>();
            foreach (Piece option in possible)
            {
                valid.Add(drawing.TupleToString(option) + drawing.TupleToString(position));
            }

            return valid;
        }
    }
}