using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace practica
{
    public partial class Form1 : Form
    {
        View3D view = new View3D();

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            UInt64[] B = view.ColorBuffer();
            int h = pictureBox1.Height;
            int w = pictureBox1.Width;
            var bmp = new Bitmap(w, h);

            for (int i = 0; i < h; ++i)
            {
                int Y = h - 1 - i;

                for (int X = 0; X < w; ++X)
                {
                    var a = B[Y * w + X];
                    bmp.SetPixel(X, i, Graphicx.IntToColor(a));
                }
            }

            pictureBox1.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            view.SetViewPort(pictureBox1.Width, pictureBox1.Height);
            Redraw();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            view.SetViewPort(pictureBox1.Width, pictureBox1.Height);
            Redraw();
        }

        private void Redraw()
        {
            view.Clear(Color.Green);
        }
    }
}
