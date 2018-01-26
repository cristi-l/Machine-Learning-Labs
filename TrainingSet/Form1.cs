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

namespace TrainingSet
{
    public partial class Form1 : Form
    {
        static int pointCount = 750,zoneCount=10;
        Pen pen,linePen;
        Graphics graphics;
        Random random;
        Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.GreenYellow, Color.Purple, Color.Pink, Color.Brown, Color.Yellow, Color.Turquoise,Color.Maroon };
        Point[] zone = new Point[zoneCount];
        Point[] points = new Point[pointCount*zoneCount];
        int[] sigmaX = new int[zoneCount], sigmaY = new int[zoneCount];
        public Form1()
        {
            InitializeComponent();
            pen = new Pen(Color.Black, 1);
            graphics = panel1.CreateGraphics();
            random = new Random();
            for(int i=0;i<zone.Length;i++)
            {
                zone[i] = new Point(random.Next(-360, 360), random.Next(-360, 360));
            }
            for (int i = 0; i < sigmaX.Length; i++)
            {
                sigmaX[i] = sigmaY[i] = 35;
            }
            linePen = new Pen(Color.Black);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            graphics.DrawLine(linePen, panel1.Width / 2, 0, panel1.Width / 2, panel1.Height);
            graphics.DrawLine(linePen, 0, panel1.Height / 2, panel1.Width, panel1.Height / 2);
            graphics.TranslateTransform(400, 400);
            for (int i = 0; i < zone.Length; i++)
            {
                draw_point(zone[i],graphics,Color.Magenta);
            }
            for (int i = 0; i < sigmaX.Length; i++)
            {
                draw_reactangle(zone[i],sigmaX[i],sigmaY[i],graphics);
            }
            
            
        }

        private void draw_reactangle(Point center, int dx, int dy,Graphics g)
        {
            g.DrawRectangle(pen, center.X - dx / 2, center.Y - dy / 2, dx, dy);
        }
        private double G(double x, double m, double sigma)
        {
            return Math.Exp(-((Math.Pow(m - x, 2)) / (2*sigma*sigma)));
        }
        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            int i = random.Next(zoneCount);
            double x, y;
            double p, g;
            using (var f = new System.IO.StreamWriter("../../../out10grupe.txt"))
            {
                f.WriteLine(points.Length);
            for (int c = 0; c < points.Length; c++)
            {
                i = random.Next(zoneCount);
                do
                {
                    x = random.Next(-400,400);
                    g = G(x: x, m: zone[i].X, sigma: sigmaX[i]/2);

                    p = random.Next(0, 10000) / 10000.0;
                   // g = random.NextDouble();
                } while (g <=p);//0.65);
                do
                {
                    y = random.Next(-400, 400);
                    g = G(x: y, m: zone[i].Y, sigma: sigmaY[i]/2);

                    p = random.Next(0,10000)/10000.0;
                    //g = random.NextDouble();
                } while (g <= p);//0.65);
                points[c] = new Point((int)x,(int)y);
                draw_point(points[c], graphics,colors[i]);
                f.WriteLine(i+" "+points[c].X + " " + points[c].Y);
            }
            
               
                    
                
            }
        }
        private void draw_point(Point p, Graphics g,Color c)
        {
            draw_point(p.X, p.Y, g,c);
        }
        private void draw_point(int x, int y, Graphics g,Color c)
        {
            pen.Color = c;
            g.DrawLine(pen,  x-1, y-1, x+1, y+1);
            g.DrawLine(pen,  x+1, y-1, x-1, y+1);
        }

        
    }
}
