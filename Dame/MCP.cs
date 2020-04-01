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
            char top;

            //nur reihe 0-3 besetzt (und jeweils gespiegelt)
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 8; x += 2)
                {
                    if (y % 2 == 1)
                    {
                        board.Add(Tuple.Create((x + 1), y), 'b');
                        //gegnerische Steine
                        board.Add(Tuple.Create(x, (7 - y)), 'w');
                    }
                    else
                    {
                        board.Add(Tuple.Create(x, y), 'b');
                        board.Add(Tuple.Create((x + 1), (7 - y)), 'w');
                    }
                }

        }

        private List<Piece> move(Piece piece)             //Liste aller Felder auf piece ziehen kann, egal was sich dort befindet
        {
            List<Piece> output = new List<Piece>();
            return output;
        }
        
        private bool jump(Piece start, Piece target)      //true wenn start über target springen kann
        {
            return false;
        }

        private List<Piece> jumping(Piece piece)          //Liste aller Felder, die durch springen mit piece erreichbar sind
        {
            List<Piece> output = new List<Piece>();
            return output;
        }

        public void run()
        {
            Generate_Board();
            drawing.Draw_Board(board);
            drawing.wait(1000);
            Generate_Board();
            drawing.Draw_Board(board);

        }
    }
}