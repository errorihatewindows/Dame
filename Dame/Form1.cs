﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void Form1_Paint(object sender, PaintEventArgs e)   //Schachbrett zeichnen
        {

    

            Graphics l = e.Graphics;

            Pen pen = new Pen(Color.Sienna, 1);
            Brush brush = Brushes.Sienna;
               
            l.DrawRectangle(pen, 75, 75, 400, 400);
            l.FillRectangle(brush, 75, 75, 400, 400);


            pen = new Pen(Color.Wheat, 1);
            brush = Brushes.Wheat;

            
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

            l.Dispose();

        }

        public void Draw_Piece(int x, int y, char piece)      //Zeichnet Spielfiguren an gegebener Stelle 
        {
            Graphics man = this.CreateGraphics();

            //relative Koordinaten in absolute + 0,0 nach links unten transformieren
            int choor_x = 90 + (x * 50);
            int choor_y = 90 + ((7 - y) * 50);

            
            //Großbuchstabe
            if (piece < 97)
            {
                // ist es ein B?
                if (piece == 66)
                {
                    
                    //Dame Schwarz
                    Pen pen = new Pen(Color.Black, 20);
                    Brush brush = Brushes.IndianRed;
                    man.DrawEllipse(pen, choor_x, choor_y, 20, 20);
                    man.FillEllipse(brush, choor_x, choor_y, 20, 20);

                } else
                {

                    //Dame weiß
                    Pen pen = new Pen(Color.FloralWhite, 20);
                    Brush brush = Brushes.IndianRed;
                    man.DrawEllipse(pen, choor_x, choor_y, 20, 20);
                    man.FillEllipse(brush, choor_x, choor_y, 20, 20);
                }
            } else  //Kleinbuchstabe
            {
                //ist es ein b?
                if (piece == 98)
                {

                    //Man Schwarz
                    Pen pen = new Pen(Color.Black, 15);
                    Brush brush = Brushes.Black;
                    man.DrawEllipse(pen, choor_x, choor_y, 20, 20);
                    man.FillEllipse(brush, choor_x, choor_y, 20, 20);

                }
                else
                {

                    //Man weiß
                    Pen pen = new Pen(Color.FloralWhite, 15);
                    Brush brush = Brushes.FloralWhite;
                    man.DrawEllipse(pen, choor_x, choor_y, 20, 20);
                    man.FillEllipse(brush, choor_x, choor_y, 20, 20);
                }
            }




        }

        public void Draw_Board(Dictionary<Tuple<int, int>, char> Board) // Zeichnet einen kompletten Schachbrett-Zustand
        {
            foreach (KeyValuePair<Tuple<int, int>, char> kvp in Board)
                Draw_Piece(kvp.Key.Item1, kvp.Key.Item2, kvp.Value);
            
        }

        //TODO: in 2 Funktionen teilen, button click setzt eine boolean- Membervariable, 2. funktion gibt einen formatierten Zug aus
        private void Zug_bestätigt_Click(object sender, EventArgs e)
        {
            bool valid1 = false, valid2 = false, valid3 = false;

            string move = Zug.Text;
            move.ToUpper();



            //Überprüfe jeden Charackter
            for (int i = 0; i < move.Length; i++)
            {
                //Überprüfe Buchstaben
                if (i % 3 == 0)
                {
                    if (move[i] > 64 && move[i] < 73)                
                        valid1 = true;
                }
                //Überprüfe Zahl
                if (i % 3 == 1)
                {
                    if (move[i] > 47 && move[i] < 57)
                        valid2 = true;
                }
                //Überprüfe Komma
                if (i % 3 == 2)
                {
                    if (move[i] == 44)
                        valid3 = true;
                }
            }

            //Überprüfen auf korrekte Syntax der Zug-Eingabe
            if ((move.Length % 3) != 2)
                valid1 = false;


                if (valid1 && valid2 && valid3)
                { 
                    // Hier aufruf der Return Funktion      
                } else 
                    MessageBox.Show("Ungültige Syntax für einen Zug");

        }  

    }
}
