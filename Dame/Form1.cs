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
        private bool ENTER = false, gamestarted = false;
        private string move = "", tempmove = "";

        Bitmap b = new Bitmap(@"b.png");
        Bitmap s = new Bitmap(@"s.png");
        Bitmap BD = new Bitmap(@"BD.png");
        Bitmap WD = new Bitmap(@"WD.png");





        public Form1()
        {
            InitializeComponent();
            mcp = new MCP(this);
            DoubleBuffered = true;

        }

        //Übergibt den Move an MCP
        public string get_move(Board boarstate, int player)
        {
            bool valid = false;

            //inkorrekte Move Eingabe
            while (!valid)
            {

                move = "";
                tempmove = "";

                //Warten auf Button Eingabe
                while (!ENTER)
                    wait(100);

                ENTER = false;

                valid = check_Syntax(move); //True wenn Syntax korrekt

                if (!valid)
                {
                    Invalidate();

                    MessageBox.Show("Ungültige Syntax für einen Zug."
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + "Oder wie Google-Übersetzer sagen würde:" + "     Invalid syntax for a train."
                                    + Environment.NewLine
                                    + "Oder für unsere Ungarischen Freunde:" + "  Érvénytelen szintaxis a vonaton."
                                    + Environment.NewLine
                                    + "と 日本語: トレインの無効な構文");


                }
            }


            return move;
        }


        //Zeichnet Spielfiguren an gegebener Stelle
        public void Draw_Piece(int x, int y, char piece, Graphics l) 
        {
            //relative Koordinaten in absolute + 0,0 nach links unten transformieren
            int choor_x = 90 + (x * 50);
            int choor_y = 90 + ((7 - y) * 50);

            //Großbuchstabe
            if (piece < 97)
            {
                // ist es ein B?
                if (piece == 66)                
                    l.DrawImage(BD, choor_x - 9, choor_y - 10);   //Dame Schwarz
                //ist es ein W?
                else if (piece == 87)                    
                    l.DrawImage(WD, choor_x - 10, choor_y - 10);  //Dame weiß
            }
            //Kleinbuchstabe
            else
            {
                //ist es ein b?
                if (piece == 98)                   
                    l.DrawImage(b, choor_x - 9, choor_y - 10);    //Man Schwarz
                //ist es ein w?
                else if (piece == 119)
                    l.DrawImage(s, choor_x - 10, choor_y - 10);   //Man weiß           
            }
        }
        // Zeichnet einen kompletten Schachbrett-Zustand
        public void Draw_Board(Board Board) 
        {         
            Graphics l = this.CreateGraphics();
            
            //zeichne Rand
            Pen pen = new Pen(Color.Black, 3);
            l.DrawRectangle(pen, 74, 74, 401, 401);

            //Schachbrettmuster zeichnen
            pen = new Pen(Color.Sienna, 1);
            Brush brush = Brushes.Sienna;

            l.FillRectangle(brush, 75, 75, 400, 400);

            brush = Brushes.PeachPuff;

            for (int i = 75; i < 400; i += 100)
                for (int j = 75; j < 400; j += 100)
                    l.FillRectangle(brush, i, j, 50, 50);

            for (int i = 125; i <= 450; i += 100)
                for (int j = 125; j <= 450; j += 100)
                    l.FillRectangle(brush, i, j, 50, 50);

            //Steine aufs Brett zeichnen
            foreach (KeyValuePair<Tuple<int, int>, char> kvp in Board)
                Draw_Piece(kvp.Key.Item1, kvp.Key.Item2, kvp.Value, l);

            l.Dispose();
        }


        //Spiel initialisieren
        private void button1_Click(object sender, EventArgs e)
        {
            startgame();          
        }
        //Startet das Spiel
        private void startgame()
        {
            Zug_bestätigt.Focus();

            Tuple<string, string> Input = getInput();

            mcp.set_user(Input.Item1, Input.Item2);

            //Spieleinstellungen während des SPieles blockieren
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;

            //Zugeingabefelder sichtbar machen

            Zug.Visible = true;
            Zug_bestätigt.Visible = true;
            label17.Visible = true;

            //Spiel ausführen      
            int Winner = mcp.run(true);

            if (Winner == -1)
                MessageBox.Show("Ein Unentschieden!");
            if (Winner == 0)
                MessageBox.Show("Schwarz hat Gewonnen!");
            if (Winner == 1)
                MessageBox.Show("Weiß hat Gewonnen!");

            //Spieleinstellungen nach des SPieles wieder freigeben
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
        }

        //Lernen initialisieren
        private void button2_Click(object sender, EventArgs e)
        {
            mcp.AI();
        }
        //Startet Simulation 
        private void simulate_Click(object sender, EventArgs e)
        {
            Tuple<string, string> Input = getInput();

            int count = Convert.ToInt32(intSimulate.Text);

            if (radioButtonSpielerSchwarz.Checked || radioButtonSpielerWeiß.Checked)
                MessageBox.Show("Simulieren nicht bei eigenen Spielern möglich.");
            else
            {
                mcp.set_user(Input.Item1, Input.Item2);
                //Zeichnet die Spielfelder nicht, nur simulation
                mcp.simulate(count);
            }


        }


        //Macht aus Fled Tuple einen String
        public string TupleToString(Piece field)
        {
            string a = ((char)(field.Item1 + 'A')).ToString();
            string b = Convert.ToString((field.Item2) + 1);

            string num = a + b;

            return num;
        }
        //Macht aus String das Feld Tuple
        public Piece StringToTuple(string place)
        {
            place = place.ToUpper();

            int a = Convert.ToInt32(Convert.ToChar(place[0]) - 'A');
            int b = Convert.ToInt32(place[1]) - '1';

            Tuple<int, int> field = Tuple.Create(a, b);

            return field;
        }       
        //Returnt TRUE wenn Syntax korrekt
        public bool check_Syntax(string move)   
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
            if (((move.Length % 3) == 2) && move.Length > 4)
                count_valid++;


            //alles Korrekt
            if (count_valid == move.Length + 1)
                valid = true;

            return valid;
        }

        //LabelText funktionen für MCP
        public void labelText(string Text)
        {            
            label37.Text = "";
            label37.Text = Text;
            wait(10);
        }
        public void labelStatus(string Text)
        {
            label39.Text = "";
            label39.Text = Text;
            wait(10);
        }
        public void labelWeights(string Text)
        {
            labelweights.Text = "";
            labelweights.Text = Text;
            wait(10);
        }


        //Abfrage der RadioButton auswahl, gibt Tuple zurück
        private Tuple<string, string> getInput()
        {
            string Schwarz = "", Weiß = "";

            //Ausgewähltes Setup abfragen und laden
            if (radioButtonSpielerSchwarz.Checked)
            {
                Schwarz = "Spieler1"; //player vs player
                //Merkieren der Felder mit Maus erlaubt
                gamestarted = true;
            }

            if (radioButtonZufallSchwarz.Checked)
                Schwarz = "RAND 1";
            if (radioButtonKISchwarz.Checked)
                Schwarz = "CPU 1";

            if (radioButtonSpielerWeiß.Checked) 
            {
                Weiß = "Spieler2"; //player vs player
                //Merkieren der Felder mit Maus erlaubt
                gamestarted = true;
            }
                
            if (radioButtonZufallWeiß.Checked)
                Weiß = "RAND 2";
            if (radioButtonKIWeiß.Checked)
                Weiß = "CPU 2";

            Tuple<string, string> choose = Tuple.Create(Schwarz, Weiß);

            return choose;
        }   
        //Setzt MausEingaben zu Input Move zusammen
        private void Form1_MouseClick_1(object sender, MouseEventArgs e)
        {
            //Wenn noch nicht mit ENTER bestätigt oder Spiel gestartet
            if (!ENTER && gamestarted)
            {
                Rectangle screenRectangle = this.RectangleToScreen(this.ClientRectangle);

                //Relative Koordinaten zur oberen, linken Schachbrettkante
                int x = MousePosition.X - screenRectangle.X - 75;
                int y = MousePosition.Y - screenRectangle.Y - 75;

                //Mouse Click außerhalb des Spielfeldes
                if (x > 400 || x < 0 || y < 0 || y > 400)
                {
                    Console.WriteLine("out of border");
                }
                else
                {
                    if (tempmove == "")
                        tempmove = get_Tile(x, y);
                    else
                        tempmove = tempmove + "," + get_Tile(x, y);

                    Zug.Text = tempmove;
                }
            }
        }
        //Wenn sich TextEingabe verändert
        private void Zug_TextChanged(object sender, EventArgs e)
        {
            Graphics l = CreateGraphics();

            if (Zug.Text.Length == 0)
                Invalidate();

            if (Zug.Text.Length > 1)
            {
                if ((Zug.Text.Length % 3) == 2 || (Zug.Text.Length % 3) == 0)
                {
                    Invalidate();
                    wait(5);
                    highlight_Tiles(Zug.Text, l);
                }
            }

        }
        //Macht aus Mauszeigerposition Feldkoordinaten
        private string get_Tile(int x, int y)
        {
            int Tile_x = Convert.ToInt32(Math.Ceiling((double)(x / 50)));
            int Tile_y = Convert.ToInt32(Math.Abs(Math.Ceiling((double)(y / 50)) - 7));

            return TupleToString(Tuple.Create(Tile_x, Tile_y));
        }        
        //Markiert ausgewählte Felder
        private void highlight_Tiles(string move, Graphics l)
        {
            
            Pen pen = new Pen(Color.LightSkyBlue, 4);

            for (int i = 0; i < move.Length; i += 3)
            {
                Piece field = StringToTuple(move[i].ToString() + move[i + 1].ToString());
                
                l.DrawRectangle(pen, (field.Item1 * 50) + 75, (Math.Abs(field.Item2 - 7) * 50) + 75, 50, 50);
            }
   
        }

       
        //übergibt den finalen Move
        private void Zug_bestätigt_Click(object sender, EventArgs e)
        {
            move = Zug.Text;
            ENTER = true;
            Zug.Text = "";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("haha");
            System.Environment.Exit(0);
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        //Zeichnet das Board
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics l = e.Graphics;

            //zeichne Rand
            Pen pen = new Pen(Color.Black, 3);
            l.DrawRectangle(pen, 74, 74, 401, 401);

            //Schachbrettmuster zeichnen
            pen = new Pen(Color.Sienna, 1);
            Brush brush = Brushes.Sienna;

            l.FillRectangle(brush, 75, 75, 400, 400);

            brush = Brushes.PeachPuff;

            for (int i = 75; i < 400; i += 100)
                for (int j = 75; j < 400; j += 100)
                    l.FillRectangle(brush, i, j, 50, 50);

            for (int i = 125; i <= 450; i += 100)
                for (int j = 125; j <= 450; j += 100)
                    l.FillRectangle(brush, i, j, 50, 50);

            //Steine aufs Brett zeichnen
            foreach (KeyValuePair<Tuple<int, int>, char> kvp in mcp.Get_Board())
                Draw_Piece(kvp.Key.Item1, kvp.Key.Item2, kvp.Value, l);
        }

        //Bestätigen oder Zurücksetzen des Zuges
        private void Zug_bestätigt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Zug_bestätigt_Click(this, new EventArgs());
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Zug.Text = "";
                move = "";
                tempmove = "";
                e.Handled = true;
            }
                
        }

        //Wartet gewisse anzahl millisekunden
        public void wait(int milliseconds)
        {
            Timer timer1 = new Timer();
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
        }
    }
}
