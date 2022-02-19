using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace practica
{
	public partial class Form1 : Form
	{
		readonly View3D view = new View3D();

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
			Size = new Size(1200, 600);
			pictureBox1.Width = 980;
			pictureBox1.Height = Height - 48;
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
			view.Display();
			pictureBox1.Refresh();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			view.view_x0 -= 50;
			Redraw();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			view.ClearScene();
			Redraw();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			view.C0 = Convert.ToDouble(textBox1.Text);

			try
			{
				Redraw();
			}
			catch
			{
				MessageBox.Show("Коэффициент преломления принял недопустимое значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
			}
		}


		private void button1_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex == 0)
			{
				view.AddShip();
				Redraw();
			}
			if (comboBox1.SelectedIndex == 1)
			{
				view.AddBall();
				Redraw();
			}
			if (comboBox1.SelectedIndex == 2)
			{
				view.AddCloud();
				Redraw();
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			view.view_x0 += 50;
			Redraw();
		}

		private void button6_Click(object sender, EventArgs e)
		{
			File.WriteAllText(@"log.txt", string.Empty);
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}
	}
}
