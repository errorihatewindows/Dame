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
        List<Piece> output = new List<Piece>();
        List<Piece> possible = new List<Piece>();
        string final_move;

        //Konstruktor
        public CPU(Form1 form)
        {

        }

        public string get_move(Board Board, int player)               // int = 0 => Schwarz (unten), fängt an
        {
            output.Clear();
            possible.Clear();

            
            foreach (KeyValuePair <Piece,char> position in Board)
            {
                //Computer hat Schwarz + aktueller Stein ist Schwarz
                if ((player == 0) && ((position.Value == 'b') || (position.Value == 'B')))                  
                    checkAll_Black(position.Key);
               

                //Computer hat Weiß + aktueller Stein ist Weiß
                if (player == 1 && (position.Value == 'w' || position.Value == 'W'))
                    checkAll_White(position.Key);
            }

            Console.WriteLine(possible);




            return final_move;
        }

        private List<Piece> checkAll_Black(Piece piece)
        {
            //mögliche diagonalen nach oben

            Console.WriteLine("Black");
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 + 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 + 1));

            return possible;
        }
        
        private List<Piece> checkAll_White(Piece piece)
        {
            //mögliche Diagonalen nach unten

            Console.WriteLine("White");
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 - 1));

            return possible;
        }

    }
}