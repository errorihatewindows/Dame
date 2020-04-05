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
        private bool Clicked = false;
        private string move;

        public Form1()
        {
            InitializeComponent();
            mcp = new MCP(this);
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

            }
            else  //Kleinbuchstabe
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
            Graphics l = this.CreateGraphics(); 

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
        
        private void Zug_bestätigt_Click(object sender, EventArgs e)
        {            
            move = Zug.Text;
            Zug.Text = "";
            Clicked = true;
        }

        //Bestätigen der ZU Eingabe per ENTER
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Zug_bestätigt_Click(this, new EventArgs());
            }

        }

        public string get_move(Board boarstate, int player)
        {
            bool valid = false;
            label34.Text = "Spieler " + player.ToString() + " am Zug";

            //inkorrekte Move Eingabe
            while (!valid)
            {

                //Warten auf Button Eingabe
                while (!Clicked)
                    wait(100);

                Clicked = false;

                valid = check_Syntax(move); //True wenn Syntax korrekt

                if (!valid)
                {
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

        private void button1_Click(object sender, EventArgs e)
        {
            //Ausgewähltes Setup abfragen und laden
            if (radioButtonSpieler.Checked)
                mcp.set_user("Spieler 1", "Spieler 2"); //player vs player

            if (radioButtonZufall.Checked)
            {
                if (radioButtonSchwarz.Checked)
                    mcp.set_user("Spieler 1", "CPU");   //player vs CPU
                if (radioButtonWeiß.Checked)
                    mcp.set_user("CPU", "Spieler 1");   //CPU vs player
            }

            if (radioButtonKI.Checked)
            {
                MessageBox.Show("KI noch nicht verfügbar :(");
                return;
            }

           
            //Spieleinstellungen während des SPieles blockieren
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;

            //Zugeingabefelder sichtbar machen

            Zug.Visible = true;
            Zug_bestätigt.Visible = true;
            label17.Visible = true;
            
            //Status Label verbergen
            label37.Visible = false;

            //Spiel ausführen      
            int Winner = mcp.run();


            if (Winner == -1)
                MessageBox.Show("Ein Unentschieden!");
            if (Winner == 0 && radioButtonSchwarz.Checked && radioButtonZufall.Checked)
                MessageBox.Show("Schwarz, also Du, hast Gewonnen. Gratulation! Du hast besser gespielt als der Zufall :)");
            if (Winner == 0 && radioButtonSchwarz.Checked && radioButtonKI.Checked)
                MessageBox.Show("Schwarz, also Du, hast Gewonnen. Gratulation! Du hast besser gespielt als die KI :)");
            if (Winner == 1 && radioButtonWeiß.Checked && radioButtonZufall.Checked)
                MessageBox.Show("Weiß, also Du hast, Gewonnen. Gratulation! Du hast besser gespielt als der Zufall :)");
            if (Winner == 1 && radioButtonWeiß.Checked && radioButtonKI.Checked)
                MessageBox.Show("Weiß, also Du hast, Gewonnen. Gratulation! Du hast besser gespielt als die KI :)");
            if (Winner == 0 && radioButtonWeiß.Checked && radioButtonZufall.Checked)
                MessageBox.Show("Schwarz, hat Gewonnen. Pech für dich! Du bist schlechter als der Zufall :)");
            if (Winner == 0 && radioButtonWeiß.Checked && radioButtonKI.Checked)
                MessageBox.Show("Schwarz,  hat Gewonnen. Pech für dich! Du bist schlechter als die KI :)");
            if (Winner == 1 && radioButtonSchwarz.Checked && radioButtonZufall.Checked)
                MessageBox.Show("Weiß, hat Gewonnen. Pech für dich! Du bist schlechter als der Zufall :)");
            if (Winner == 1 && radioButtonSchwarz.Checked && radioButtonKI.Checked)
                MessageBox.Show("Weiß, hat Gewonnen. Pech für dich! Du bist schlechter als die KI :)");

            //Spieleinstellungen nach des SPieles wieder freigeben
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;


        }

        public string TupleToString(Tuple<int, int> field)
        {
            string a = ((char)(field.Item1 + 'A')).ToString();
            string b = Convert.ToString((field.Item2) + 1);

            string num = a + b;

            return num;
        }

        public Piece StringToTuple(string place)
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
            if (((move.Length % 3) == 2) && move.Length > 4)
                count_valid++;


            //alles Korrekt
            if (count_valid == move.Length + 1)
                valid = true;

            return valid;
        }

        private void Zug_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                Zug_bestätigt_Click(this, new EventArgs());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("haha");
            System.Environment.Exit(0);
        }

        public void labelText(string Text)
        {
            label37.Visible = true;
            label37.Text = Text;
        }



    }
}
