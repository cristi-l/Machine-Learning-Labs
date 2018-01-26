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

namespace K_Means
{
    public partial class Form1 : Form
    {
        int formulaDistanta = 1;
        Pen pen, linePen;
        Graphics graphics;
        Random random;
        Punct[] puncte;
        Punct[] centroizi;
        Color[] colors = { Color.Red, Color.Blue, Color.Lime, Color.Purple, Color.Magenta, Color.Yellow, Color.Turquoise, Color.Navy, Color.Orange };
        public Form1()
        {
            InitializeComponent();
            pen = new Pen(Color.Black, 1);
            graphics = panel1.CreateGraphics();
            graphics.TranslateTransform(400, 400);
            random = new Random(144);
            linePen = new Pen(Color.Black);
            centroizi = new Punct[random.Next(2, 9)];
            for (int i = 0; i < centroizi.Length; i++)
            {
                centroizi[i] = new Punct();
                centroizi[i].x = random.Next(-350, 350);
                centroizi[i].y = random.Next(-350, 350);
                centroizi[i].zona = i;
                centroizi[i].c = colors[i];
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            deseneazaAxele();
            deseneazaCentroizii(centroizi);

        }

        private void deseneazaAxele()
        {
            graphics.DrawLine(linePen, panel1.Width / 2, 0, -panel1.Width / 2, 0);
            graphics.DrawLine(linePen, 0, panel1.Height / 2, 0, -panel1.Height / 2);
        }

        private void draw_reactangle(Point center, int dx, int dy, Graphics g)
        {
            g.DrawRectangle(pen, center.X - dx / 2, center.Y - dy / 2, dx, dy);
        }
        private double G(double x, double m, double sigma)
        {
            return Math.Exp(-((Math.Pow(m - x, 2)) / (2 * sigma * sigma)));
        }
        private void draw_point(int x, int y, Graphics g, Color c)
        {

        }

        private void openFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            using (var sr = new System.IO.StreamReader(openFileDialog1.FileName))
            {
                String line = sr.ReadLine();
                int n = 0;
                if (int.TryParse(line, out n))
                {
                    puncte = new Punct[n];
                    for (int i = 0; i < n; i++)
                    {
                        line = sr.ReadLine();
                        puncte[i] = new Punct();
                        var s = line.Split(' ');
                        puncte[i].c = Color.Black;
                        puncte[i].x = int.Parse(s[1]);
                        puncte[i].y = int.Parse(s[2]);
                    }
                }
            }
            deseneazaPunctele(puncte);
        }
        private void deseneazaCentroizii(Punct[] puncte)
        {
            foreach (var punct in puncte)
            {
                pen.Color = punct.c;
                graphics.FillEllipse(pen.Brush, punct.x + 6, punct.y - 6, 12, 12);
                pen.Color = Color.Black;
                graphics.DrawEllipse(pen, punct.x + 6, punct.y - 6, 12, 12);
            }
        }
        private void stergeCentroizii(Punct[] puncte)
        {
            foreach (var punct in puncte)
            {
                pen.Color = panel1.BackColor;
                graphics.FillEllipse(pen.Brush, punct.x + 6, punct.y - 6, 12, 12);
                
            }
        }
        private void deseneazaPunctele(Punct[] puncte)
        {
            foreach (var item in puncte)
            {
                pen.Color = item.c;
                graphics.DrawLine(pen, item.x - 1, item.y - 1, item.x + 1, item.y + 1);
                graphics.DrawLine(pen, item.x + 1, item.y - 1, item.x - 1, item.y + 1);
            }
        }
        private double distantaEuclidiana(Punct a, Punct b)
        {
            return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }
        private double distantaEuclidiana(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
        private double distantaManhattan(Punct a, Punct b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }
        private double distantaManhattan(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
        private double distantaCosinus(Punct a, Punct b)
        {
            double x1 = a.x / 400.0, y1 = a.y / 400.0, x2 = b.x / 400.0, y2 = b.y / 400.0;
            return (x1 * x2 + y1 * y2) / (double)((x1 * x1 + y1 * y1) * (x2 * x2 + y2 * y2));
        }
        private double distantaCosinus(int X1, int Y1, int X2, int Y2)
        {
            double x1 = X1 / 400.0,y1=Y1/400.0,x2=X2/400.0,y2=Y2/400.0;
            return (x1 * x2 + y1 * y2) / (double)((x1 * x1 + y1 * y1) * (x2 * x2 + y2 * y2));
        }

        private double assign()
        {
            double distanta = 0;
            foreach (var punct in puncte)
            {
                double d, min = int.MaxValue;
                foreach (var centroid in centroizi)
                {
                    switch (formulaDistanta)
                    {
                        case 1:d = distantaEuclidiana(punct, centroid);
                            break;
                        case 2: d = distantaManhattan(punct, centroid);
                            break;
                        default:
                            d = distantaCosinus(punct, centroid);
                            break;
                    }
                    
                    
                    
                    if (d < min)
                    {
                        min = d;
                        punct.c = centroid.c;
                        punct.zona = centroid.zona;
                    }
                }
                distanta += min;
            }
            return distanta;
        }
        private void update()
        {
            foreach (var centroid in centroizi)
            {
                int x = 0, count = 0, y = 0;
                foreach (var punct in puncte)
                    if (punct.zona == centroid.zona)
                    {
                        x += punct.x;
                        y += punct.y;
                        count++;
                    }
                if (count != 0)
                {
                    centroid.x = x / count;
                    centroid.y = y / count;
                }
            }
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            double convergenta = 0, convergentaAnterioara;

            do
            {
                convergentaAnterioara = convergenta;
                stergeCentroizii(centroizi);
                deseneazaAxele();
                convergenta = assign();
                update();
                deseneazaCentroizii(centroizi);
                deseneazaPunctele(puncte);
                deseneazaCentroizii(centroizi);
            } while (convergenta != convergentaAnterioara);//(Math.Abs(convergenta - convergentaAnterioara)<5);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Color c = Color.Black;
            for (int i = -400; i < 400; i+=5)
            {
                for (int j = -400; j < 400; j+=5)
                {
                    double d, min = int.MaxValue;
                    foreach (var centroid in centroizi)
                    {
                        switch (formulaDistanta)
                        {
                            case 1: d = distantaEuclidiana(i, j, centroid.x, centroid.y);
                                break;
                            case 2: d = distantaManhattan(i, j, centroid.x, centroid.y);
                                break;
                            default:
                                d = distantaCosinus(i, j, centroid.x, centroid.y);
                                break;
                        }
                        
                        
                        
                        
                        if (d < min)
                        {
                            min = d;
                            c = centroid.c;
                        }
                    }
                    pen.Color = c;
                    graphics.FillRectangle(pen.Brush, i, j, 2, 2);
                }
            }
        }
    }
}

