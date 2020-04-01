using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dame
{
    public class MCP
    {
        private Dictionary<Tuple<int, int>, char> Board = new Dictionary<Tuple<int,int>,char>();
        private Form1 drawing;
        //Constructor
        public MCP(Form1 form)
        {
            drawing = form;
        }

        private void Generate_Board(char bot) //erstellt Startposition für weiß und schwarz + zeichnen
        {
            char top;
            if (bot == 'b') { top = 'w'; }
            else { top = 'b'; }

            //nur reihe 0-3 besetzt (und jeweils gespiegelt)
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 8; x += 2)
                {
                    if (y % 2 == 1)
                    {
                        Board.Add(Tuple.Create((x + 1), y), bot);
                        //gegnerische Steine
                        Board.Add(Tuple.Create(x, (7 - y)), top);
                    }
                    else
                    {
                        Board.Add(Tuple.Create(x, y), bot);
                        Board.Add(Tuple.Create((x + 1), (7 - y)), top);
                    }
                }

        }

        public void run()
        {
            Generate_Board('w');
            drawing.wait(1000);
            drawing.Draw_Board(Board);

        }
    }
}