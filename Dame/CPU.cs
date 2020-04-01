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
        public string get_move(Board Board, int player)               // int = 0 => Schwarz (unten), fängt an
        {
            foreach (KeyValuePair<Piece, char> kvp in Board)
            {
                
            }
                



                return;
        }

    }
}