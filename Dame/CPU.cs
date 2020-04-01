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
        List<string> possible = new List<string>();
        string final_move;

        
        private Form1 drawing;
        //Konstruktor
        public CPU(Form1 form)
        {
           drawing = form;
        }


        public string get_move(Board Board, int player)               // int = 0 => Schwarz (unten), fängt an
        {
            possible.Clear();

            
            foreach (KeyValuePair <Piece,char> position in Board)
            {
                checkposition(position, player);
            }

            for(int i = 0; i < 10; i++) 
                Console.WriteLine(possible[i]);

            return final_move;
        }

        private List<string> checkposition(KeyValuePair<Piece, char> position, int player)
        {
            //Computer speichert alle Diagonalen der Steine seiner Farbe
            if (((player == 0) && (position.Value == 'b' || position.Value == 'B')) || ((player == 1 && position.Value == 'w' || position.Value == 'W')))
            {
                Console.WriteLine("YAY");
                //vorerst mögliche diagonalen
                possible.Add(drawing.TupleToString(Tuple.Create(position.Key.Item1, position.Key.Item2)) + drawing.TupleToString(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 + 1)));
                possible.Add(drawing.TupleToString(Tuple.Create(position.Key.Item1, position.Key.Item2)) + drawing.TupleToString(Tuple.Create(position.Key.Item1 + 1, position.Key.Item2 - 1)));
                possible.Add(drawing.TupleToString(Tuple.Create(position.Key.Item1, position.Key.Item2)) + drawing.TupleToString(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 - 1)));
                possible.Add(drawing.TupleToString(Tuple.Create(position.Key.Item1, position.Key.Item2)) + drawing.TupleToString(Tuple.Create(position.Key.Item1 - 1, position.Key.Item2 + 1)));
            }

            //Invalide moves Löschen
            deleteInvalid(possible);


            return possible;
        }

        private List<string> deleteInvalid(List<string> possible)
        {





            return possible;
        }
    }
}