using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using K_Means;
using XORNN;

namespace Backpropagation
{
	public partial class Form1 : Form
	{
		Pen pen, linePen;
		Graphics graphics;
		Random random;
		Punct[] puncte;
		Color[] colors = { Color.Red, Color.Blue, Color.Lime, Color.Purple, Color.Magenta, Color.Yellow, Color.Turquoise, Color.Navy, Color.Orange, Color.Maroon,Color.AliceBlue,Color.AntiqueWhite };
		NeuralNetwork nn;
		public Form1()
		{
			InitializeComponent();
			pen = new Pen(Color.Black, 1);
			graphics = panel1.CreateGraphics();
			graphics.TranslateTransform(400, 400);
			random = new Random(1);
			linePen = new Pen(Color.Black);
			nn = new NeuralNetwork(2, 8, 10, 0.002);
			nn.InitializeWeights();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

			deseneazaAxele();


		}

		private void deseneazaAxele()
		{
			graphics.DrawLine(linePen, panel1.Width / 2, 0, -panel1.Width / 2, 0);
			graphics.DrawLine(linePen, 0, panel1.Height / 2, 0, -panel1.Height / 2);
		}


		private void openFile_Click(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog();
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			using (var sr = new System.IO.StreamReader(openFileDialog1.FileName))
			{
				string line = sr.ReadLine();
				int n = 0;
				if (int.TryParse(line, out n))
				{
					puncte = new Punct[n];
					for (int i = 0; i < n; i++)
					{
						line = sr.ReadLine();
						puncte[i] = new Punct();
						var s = line.Split(' ');
						puncte[i].zona = int.Parse(s[0]);
						puncte[i].c = Color.Black;
						puncte[i].x = int.Parse(s[1]);
						puncte[i].y = int.Parse(s[2]);
					}
				}
			}
			deseneazaPunctele(puncte);
		}


		private void deseneazaPunctele(Punct[] puncte)
		{
			foreach (var item in puncte)
			{
				pen.Color = item.c;
				graphics.FillRectangle(pen.Brush, item.x - 1, item.y - 1, 2, 2);
			}
		}


		private void buttonGenerate_Click(object sender, EventArgs e)
		{
			double error = 0;
			for (int t = 0; t < puncte.Length; t++)
			{

				Punct punct = puncte[t];			
				double[] result = nn.FeedForward(new double[] { punct.x / 400.0, punct.y / 400.0 });
				double[] target = new double[result.Length];
				double max = double.MinValue;
				int maxPos = 0;
				for (int i = 0; i < result.Length; i++)
				{
					if (result[i] > max)
					{
						maxPos = i;
						max = result[i];
					}
					target[i] = 0;
				}
				if (punct.zona != maxPos)
				{
					target[punct.zona] = 1;
					nn.BackPropagation(target);
					error++;
				}
				
				punct.c = colors[maxPos];

			}

			deseneazaPunctele(puncte);
			if (error < 0.02 * puncte.Length)
				timer1.Stop();


		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			buttonGenerate_Click(sender, e);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Color c = Color.Black;
			deseneazaPunctele(puncte);
			for (int i = -400; i < 400; i += 5)
			{
				for (int j = -400; j < 400; j += 5)
				{
					double[] result = nn.FeedForward(new double[] { i / 400.0, j / 400.0 });
					double max = double.MinValue;
					int maxPos = 0;
					for (int k = 0; k < result.Length; k++)
					{
						if (result[k] > max)
						{
							maxPos = k;
							max = result[k];
						}
					}
					pen.Color =  colors[maxPos];
					graphics.FillRectangle(pen.Brush, i, j, 3, 3);
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (timer1.Enabled)
			{
				timer1.Stop();
			}
			else
			{
				timer1.Interval = 300;
				timer1.Start();
			}
		}
	}
}

