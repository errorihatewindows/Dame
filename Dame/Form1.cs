using System;
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






       private void Form1_Paint(object sender, PaintEventArgs e)
        {

            //Schachbrett zeichnen

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

        public void Draw(int x, int y, int z)
        {
            Graphics man = this.CreateGraphics();

            if (z == 1)
            {
                Pen pen = new Pen(Color.Black, 10);
                Brush brush = Brushes.Black;
            } else
            {
                Pen pen = new Pen(Color.White, 10);
                Brush brush = Brushes.White;
            }



            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

}
