using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SOM
{

	public partial class Form1 : Form
	{
		Neuron[,] neuroni;

		Point[] puncte;
		Graphics graphics;
		Pen p, pNegru;
		static int N = 100, nrNeuroni = 18;
		int t = 0;
		private Neuron[,] neuroniT;

		public Form1()
		{
			InitializeComponent();
			p = new Pen(Color.Blue);
			pNegru = new Pen(Color.Black);
			graphics = panel1.CreateGraphics();

			graphics.TranslateTransform(400, 400);

		}
		private void genereazaNeuronii(int n, int margin)
		{
			int distX = (panel1.Width - margin / 2) / n;
			int distY = (panel1.Height - margin / 2) / n;
			neuroni = new Neuron[n, n];
			neuroniT = new Neuron[n, n];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					neuroni[i, j] = new Neuron((i * distX + margin) - panel1.Width / 2, (j * distY + margin) - panel1.Height / 2);
					//neuroniT[i, j] = new Neuron(0, 0);
					neuroni[i, j] = new Neuron(0, 0);
				}

			}

			deseneazaNeuronii();

			deseneazaAxele();
		}

		/*private void deseneazaNeuronii(int n)
        {
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    if (neuroni[i + 1, j] != null)
                        graphics.DrawLine(p, (float)neuroni[i, j].X, (float)neuroni[i, j].Y, (float)neuroni[i + 1, j].X, (float)neuroni[i + 1, j].Y);
                    if (neuroni[i, j + 1] != null)
                        graphics.DrawLine(p, (float)neuroni[i, j].X, (float)neuroni[i, j].Y, (float)neuroni[i, j + 1].X, (float)neuroni[i, j + 1].Y);
                    if (i == n - 2)
                        graphics.DrawLine(p, (float)neuroni[n - 1, j].X, (float)neuroni[n - 1, j].Y, (float)neuroni[n - 1, j + 1].X, (float)neuroni[n - 1, j + 1].Y);
                    if (j == n - 2)
                        graphics.DrawLine(p, (float)neuroni[i, n - 1].X, (float)neuroni[i, n - 1].Y, (float)neuroni[i + 1, n - 1].X, (float)neuroni[i + 1, n - 1].Y);
                }
            }
        }*/

		private void deseneazaNeuronii()
		{
			for (int i = 0; i < nrNeuroni - 1; i++)
			{
				for (int j = 0; j < nrNeuroni - 1; j++)
				{
					graphics.DrawLine(p, (float)neuroni[i, j].X, (float)neuroni[i, j].Y, (float)neuroni[i + 1, j].X, (float)neuroni[i + 1, j].Y);
					graphics.DrawLine(p, (float)neuroni[i, j].X, (float)neuroni[i, j].Y, (float)neuroni[i, j + 1].X, (float)neuroni[i, j + 1].Y);

				}
				graphics.DrawLine(p, (float)neuroni[i, nrNeuroni - 1].X, (float)neuroni[i, nrNeuroni - 1].Y, (float)neuroni[i + 1, nrNeuroni - 1].X, (float)neuroni[i + 1, nrNeuroni - 1].Y);
				graphics.DrawLine(p, (float)neuroni[nrNeuroni - 1, i].X, (float)neuroni[nrNeuroni - 1, i].Y, (float)neuroni[nrNeuroni - 1, i + 1].X, (float)neuroni[nrNeuroni - 1, i + 1].Y);
			}
		}

		private void deseneazaAxele()
		{
			graphics.DrawLine(pNegru, panel1.Width / 2, 0, -panel1.Width / 2, 0);
			graphics.DrawLine(pNegru, 0, panel1.Height / 2, 0, -panel1.Height / 2);
		}
		private void buttonOpenFile_Click(object sender, EventArgs e)
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
					puncte = new Point[n];
					for (int i = 0; i < n; i++)
					{
						line = sr.ReadLine();

						var s = line.Split(' ');
						puncte[i] = new Point(int.Parse(s[1]), int.Parse(s[2]));

					}
				}
			}
			deseneazaPunctele();
			genereazaNeuronii(nrNeuroni, 53);
		}

		private void deseneazaPunctele()
		{
			for (int i = 0; i < puncte.Length; i++)
			{
				//graphics.DrawLine(pNegru, item.X - 1, item.Y - 1, item.X + 1, item.Y + 1);
				//graphics.DrawLine(pNegru, item.X + 1, item.Y - 1, item.X - 1, item.Y + 1);
				graphics.FillRectangle(pNegru.Brush, puncte[i].X, puncte[i].Y, 2, 2);
			}
		}
		private double distantaEuclidiana(double x1, double y1, double x2, double y2)
		{
			return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
		}


		private double distantaManhattan(int x1, int y1, int x2, int y2)
		{
			return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
		}
		private double alfa(int t)
		{
			return 0.7 * Math.Exp(-t / ((double)N));
		}
		private void buttonInvata_Click(object sender, EventArgs e)
		{
			double alf = alfa(t);
			for (t = 0; alf>0.001;t+=2)
			{
				double v = V(t);
				alf = alfa(t);
				foreach (Point punct in puncte)
				{
					double min = double.MaxValue;
					int minI = 0, minJ = 0;
					for (int i = 0; i < nrNeuroni; i++)
					{
						for (int j = 0; j < nrNeuroni; j++)
						{
							double dist = distantaEuclidiana(punct.X, punct.Y, neuroni[i, j].X, neuroni[i, j].Y);
							if (dist < min)
							{
								min = dist;
								minI = i;
								minJ = j;
							}
						}
					}

					for (int i = minI - (int)v; i <= (int)v + minI; i++)
					{
						for (int j = minJ - (int)v; j <= (int)v + minJ; j++)
						{
							if (i < 0 || j < 0 || i >= nrNeuroni || j >= nrNeuroni)
							{
								continue;
							}
							neuroni[i, j].X = neuroni[i, j].X + alf * ((double)punct.X - neuroni[i, j].X);
							neuroni[i, j].Y = neuroni[i, j].Y + alf * ((double)punct.Y - neuroni[i, j].Y);
						}
					}
				}

				System.Threading.Thread.Sleep(100);
				this.Refresh();
				deseneazaPunctele();
				deseneazaAxele();
				deseneazaNeuronii();

				label1.Text = "Vecinatate: " + (int)V(t);
				label2.Text = "Alfa: " + (decimal)alfa(t);

				Thread.Sleep(10);
			}
		}

		private double V(int t)
		{
			return 7 * Math.Exp(-(double)t / N);
		}
	}
	public class Neuron
	{
		public double X { get; set; }
		public double Y { get; set; }
		public Neuron()
		{
			X = 0.0;
			Y = 0.0;
		}
		public Neuron(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
