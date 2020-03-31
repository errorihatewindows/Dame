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


        private void button1_Click(object sender, EventArgs e)
        {
            Draw_Piece(0, 0, 'S');
        }


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

            //relative Koordinaten in absolute
            int choor_x = 90 + (x * 50);
            int choor_y = 90 + (y * 50);

            
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

    }
}
