using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Piece = System.Tuple<int, int>;
using Board = System.Collections.Generic.Dictionary<System.Tuple<int, int>, char>;


namespace Dame
{
    public partial class Form1 : Form
    {
        MCP mcp;
        CPU cpu;
        private bool Clicked = false;
        private string move;


        public Form1()
        {
            InitializeComponent();
            mcp = new MCP(this);
            cpu = new CPU(this);


            
        }

        private void Form1_Shown(object sender, EventArgs e) //Zeichnet Grundzustand
        {
            Draw_Board(mcp.Get_Board());
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Draw_Board(mcp.Get_Board());
        }


        //Wartet gewisse anzahl millisekunden
        public void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }   //End of wait

        public void Draw_Piece(int x, int y, char piece)      //Zeichnet Spielfiguren an gegebener Stelle 
        {
            Graphics man = this.CreateGraphics();

            //relative Koordinaten in absolute + 0,0 nach links unten transformieren
            int choor_x = 90 + (x * 50);
            int choor_y = 90 + ((7 - y) * 50);

            Bitmap b = new Bitmap(@"..\..\b.png");
            Bitmap s = new Bitmap(@"..\..\s.png");
            Bitmap BD = new Bitmap(@"..\..\BD.png");
            Bitmap WD = new Bitmap(@"..\..\WD.png");

            //Großbuchstabe
            if (piece < 97)
            {
                // ist es ein B?
                if (piece == 66)
                {

                    //Dame Schwarz
                    Pen pen = new Pen(Color.Black, 20);
                    Brush brush = Brushes.IndianRed;
                    man.DrawImage(BD, choor_x - 9, choor_y - 10);

                }
                else if (piece == 87)
                {

                    //Dame weiß
                    Pen pen = new Pen(Color.FloralWhite, 20);
                    Brush brush = Brushes.IndianRed;
                    man.DrawImage(WD, choor_x - 10, choor_y - 10);

                }

            } else  //Kleinbuchstabe
            {
                //ist es ein b?
                if (piece == 98)
                {                    
                    
                    //Man Schwarz
                    Pen pen = new Pen(Color.Black, 15);
                    Brush brush = Brushes.Black;
                    man.DrawImage(b, choor_x - 9, choor_y - 10);

                }
                else if (piece == 119)
                {

                    //Man weiß
                    Pen pen = new Pen(Color.FloralWhite, 15);
                    Brush brush = Brushes.FloralWhite;
                    man.DrawImage(s, choor_x - 10, choor_y - 10);

                }
            }
        }

        public void Draw_Board(Dictionary<Tuple<int, int>, char> Board) // Zeichnet einen kompletten Schachbrett-Zustand
        {

            //Schachbrettmuster zeichnen
            Graphics l = this.CreateGraphics(); ;

            Pen pen = new Pen(Color.Sienna, 1);
            Brush brush = Brushes.Sienna;

            l.DrawRectangle(pen, 75, 75, 400, 400);
            l.FillRectangle(brush, 75, 75, 400, 400);


            pen = new Pen(Color.PeachPuff, 1);
            brush = Brushes.PeachPuff;


            for (int i = 75; i < 400; i = i + 100)
                for (int j = 75; j < 400; j = j + 100)
                {
                    l.DrawRectangle(pen, i, j, 50, 50);
                    l.FillRectangle(brush, i, j, 50, 50);
                }

            for (int i = 125; i <= 450; i = i + 100)
                for (int j = 125; j <= 450; j = j + 100)
                {
                    l.DrawRectangle(pen, i, j, 50, 50);
                    l.FillRectangle(brush, i, j, 50, 50);
                }

            //Steine aufs Brett zeichnen
            foreach (KeyValuePair<Tuple<int, int>, char> kvp in Board)
                Draw_Piece(kvp.Key.Item1, kvp.Key.Item2, kvp.Value);

            l.Dispose();

        }       

        //TODO: in 2 Funktionen teilen, button click setzt eine boolean- Membervariable, 2. funktion gibt einen formatierten Zug aus            
        private void Zug_bestätigt_Click(object sender, EventArgs e)
        {
            move = Zug.Text;
            Clicked = true;
            get_move();
        }

        public string get_move()
        {
            bool valid = false;

            //inkorrekte Move Eingabe
            while (!valid)
            {

                //Warten auf Button Eingabe
                while (!Clicked)
                    wait(100);

                Clicked = false;
             
                valid = check_Syntax(move); //True wenn Syntax korrekt

                if (!valid)                            
                    MessageBox.Show("Ungültige Syntax für einen Zug");
            }

            return move;           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mcp.run();            
        }

        public string TupleToString(Tuple<int, int> field)
        {
            string a = (Convert.ToChar(field.Item1 + 'A')).ToString();
            string b = (Convert.ToInt32(field.Item2) + 1).ToString();

            string num = a + b;

            return num;
        }

        public Tuple<int, int> StringToTuple(string place)
        {
            place = place.ToUpper();
            
            int a = Convert.ToInt32(Convert.ToChar(place[0]) - 'A');
            int b = Convert.ToInt32(place[1]) - '1';
            
            Tuple<int, int> field = Tuple.Create(a, b);

            return field;
        }

        public bool check_Syntax(string move)   //Returnt TRUE wenn Syntax korrekt
        {
            bool valid = false;
            move = move.ToUpper();

            //Anzahl der valid Chars
            int count_valid = 0;

            //Überprüfe jeden Charackter
            for (int i = 0; i < move.Length; i++)
            {

                //Überprüfe Buchstaben
                if (i % 3 == 0)
                {
                    if (move[i] > 64 && move[i] < 73)
                        count_valid++;
                }
                //Überprüfe Zahl
                if (i % 3 == 1)
                {
                    if (move[i] > 48 && move[i] < 57)
                        count_valid++;
                }
                //Überprüfe Komma
                if (i % 3 == 2)
                {
                    if (move[i] == 44)
                        count_valid++;
                }
            }

            //Überprüfen auf korrekte Länge der Zug-Eingabe
            if ((move.Length % 3) != 2)
                count_valid++;


            //alles Korrekt
            if (count_valid == move.Length)
                valid = true;

            return valid;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cpu.get_move(mcp.Get_Board(), 0);
        }

    }
}
