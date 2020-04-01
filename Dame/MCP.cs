using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Piece = System.Tuple<int,int>;
using Board = System.Collections.Generic.Dictionary<System.Tuple<int,int>, char>;

namespace Dame
{
    public class MCP
    {
        private Board board;
        private Form1 drawing;
        //Constructor
        public MCP(Form1 form)
        {
            drawing = form;
            Generate_Board();
        }

        public Board Get_Board()
        {
            return board;
        }

        private void Generate_Board() //erstellt Startposition für weiß und schwarz + zeichnen
        {
            board = new Board();

            //nur reihe 0-3 , 5-8 ist gespiegelt
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 8; x += 2)
                {
                    if (y == 1)
                    {
                        board.Add(Tuple.Create((x + 1), y), 'b');
                        //gegnerische Steine
                        board.Add(Tuple.Create(x, (7 - y)), 'w');
                    }
                    else if (y == 0 || y == 2)
                    {
                        board.Add(Tuple.Create(x, y), 'b');
                        board.Add(Tuple.Create((x + 1), (7 - y)), 'w');
                    }
                    else
                    {
                        board.Add(Tuple.Create((x + 1), y), '.');
                        board.Add(Tuple.Create(x, (7 - y)), '.');
                    }
                }

        }

        private List<Piece> Move(Piece piece)             //Liste aller Felder auf piece ziehen kann, egal was sich dort befindet
        {
            //output liste
            List<Piece> output = new List<Piece>();
            List<Piece> possible = new List<Piece>();
            //in frage kommen die 4 diagonalen
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 + 1));
            possible.Add(Tuple.Create(piece.Item1 + 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 - 1));
            possible.Add(Tuple.Create(piece.Item1 - 1, piece.Item2 + 1));
            //wenn keine der "ungültig" conditions aktiviert werden wird option in output packt
            foreach (Piece option in possible)
            {
                //normal Steine können nur Forwärts
                if (board[piece] == 'b') { if (piece.Item2 - 1 == option.Item2) { continue; } }
                if (board[piece] == 'w') { if (piece.Item2 + 1 == option.Item2) { continue; } }

                //Feld nur zwischen 0 und 7
                if (option.Item1 > 7 || option.Item1 < 0) { continue; }
                if (option.Item2 > 7 || option.Item2 < 0) { continue; }

                //geschafft!
                output.Add(option);
            }
            return output;
        }
        
        private bool Jump(Piece start, Piece target)      //true wenn start über target springen kann
        {
            return false;
        }

        private List<Piece> Jumping(Piece piece)          //Liste aller Felder, die durch springen mit piece erreichbar sind
        {
            List<Piece> output = new List<Piece>();
            return output;
        }

        public void run()
        {
            Generate_Board();
            drawing.Draw_Board(board);
            List<Piece> valid = Move(Tuple.Create(7,7));
            foreach (Piece move in valid)
            {
                Console.WriteLine(move.ToString());
            } 
        }
    }
}