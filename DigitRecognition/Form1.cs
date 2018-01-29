using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XORNN;
namespace DigitRecognition
{
	public partial class Form1 : Form
	{
		static class IDXReader
		{
			static int GetNumber(byte[] MSBfirstBytes)
			{
				return MSBfirstBytes[3] + (MSBfirstBytes[2] << 8) + (MSBfirstBytes[1] << 16) + (MSBfirstBytes[0] << 24);
			}
			public static List<Img> ReadData(string imagesPath, string labelPath)
			{
				List<Img> dataSet = new List<Img>();
					using (BinaryReader im = new BinaryReader(new FileStream(imagesPath, FileMode.Open)), label = new BinaryReader(new FileStream(labelPath, FileMode.Open)))
					{
					if (GetNumber(label.ReadBytes(4)) != 2049)
						throw new Exception("wrong labels file!");

					if (GetNumber(im.ReadBytes(4)) != 2051)
						throw new Exception("wrong images file!");
					int nrImgs = GetNumber(im.ReadBytes(4));
					int nrTags = GetNumber(label.ReadBytes(4));
					if (nrTags != nrImgs)
						throw new Exception("tag number does not match image number!");
					int row = GetNumber(im.ReadBytes(4));
					int col = GetNumber(im.ReadBytes(4));
					for (int i = 0; i < nrImgs; i++)
					{
						dataSet.Add(new Img()
						{
							Columns = col,
							Rows = row,
							Label = label.ReadByte(),
							bytes=im.ReadBytes(col*row)

						});
						
					}

				}				
				return dataSet;
			}
		}
		public class Img
		{
			public int Rows { get; set; }
			public int Columns { get; set; }
			public int Label { get; set; }
			public byte[] bytes;
			private double[] inValues;
			private double[] outValues;
			public double[] InputValues
			{
				get
				{
					if (inValues == null)
					{
						inValues = new double[bytes.Length];
						for (int i = 0; i < bytes.Length; i++)
							inValues[i] = (bytes[i] / 255.0*0.99)+0.01;
					}
					return inValues;
				}
			}
			public double[] OutputValues
			{
				get
				{
					if (outValues == null)
					{
						outValues = new double[10];
						for (int i = 0; i < outValues.Length; i++)
							outValues[i] = 0.01;
						outValues[Label] = 0.99;	
					}
					return outValues;
				}
			}
			private Image bitmap;
			public Image image {
				get
				{
					if (bitmap == null)
						return byteArrayToImage();
					return bitmap;
				}
			}
			public Img()
			{
				
			}
			private Image byteArrayToImage()
			{
				var img = new Bitmap(Columns, Rows);
				for(int i = 0; i < Columns; i++)
				{
					for(int j = 0; j < Rows; j++)
					{
						img.SetPixel(i, j, Color.FromArgb(bytes[i + j * Columns], bytes[i + j * Columns], bytes[i + j * Columns]));
					}
				}
				bitmap = img;
				return img;
			}

		}
		NeuralNetwork nn = new NeuralNetwork(784, 100, 10, 0.1);
		public Form1()
		{
			InitializeComponent();
			data = IDXReader.ReadData(Directory.GetCurrentDirectory()+"..\\..\\..\\train-images.idx3-ubyte", Directory.GetCurrentDirectory() + "..\\..\\..\\train-labels.idx1-ubyte");
			nn.InitializeWeights();
			
		}
		public List<Img> data=new List<Img>();

		int index = 0;
		Random r = new Random();
		private void button1_Click(object sender, EventArgs e)
		{
			index = r.Next(60000);
			var res = nn.FeedForward(data[index].InputValues);
			pictureBox1.Image = data[index].image;
			label1.Text = "";
			foreach (var item in res)
			{
				label1.Text += item.ToString("F4") + " ";
			}
			//nn.BackPropagation(data[index].OutputValues);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < 60000; i++)
			{
				
				nn.FeedForward(data[i].InputValues);
					
					nn.BackPropagation(data[i].OutputValues);
			
			}
			
		}
	}
}
