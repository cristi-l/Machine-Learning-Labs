using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XORNN
{
	class Program
	{
		static void Main(string[] args)
		{
			NeuralNetwork nn = new NeuralNetwork(2, 2, 1,1);
			nn.InitializeWeights();
			List<double[]> trainigSet = new List<double[]>();
			List<double[]> desiredResults = new List<double[]>();
			Random r = new Random(1);
			/*for (int i = 0; i < 1000; i++)
			{
				trainigSet.Add(new double[] { (r.Next()%2 ), r.Next()%2  });
				desiredResults.Add(new double[] { (int)(Math.Round(trainigSet[i][0])) ^ (int)(Math.Round(trainigSet[i][1])) });
			}
			*/
			/*trainigSet.Add(new double[] { 0.01, 0.01 });
			desiredResults.Add(new double[] { 0.01 });
			trainigSet.Add(new double[] { 0.99, 0.01 });
			desiredResults.Add(new double[] { 0.99 });
			trainigSet.Add(new double[] { 0.01, 0.99 });
			desiredResults.Add(new double[] { 0.99 });
			trainigSet.Add(new double[] { 0.99, 0.99 });
			desiredResults.Add(new double[] { 0.01 });*/

			trainigSet.Add(new double[] { 0, 0 });
			desiredResults.Add(new double[] { 0 });
			trainigSet.Add(new double[] { 1, 0 });
			desiredResults.Add(new double[] { 1 });
			trainigSet.Add(new double[] { 0, 1 });
			desiredResults.Add(new double[] { 1 });
			trainigSet.Add(new double[] { 1, 1 });
			desiredResults.Add(new double[] { 0 });
			double rez = 10,er=1;
			while(er>0.01)
			{
				double sumError = 0;
				for (int i = 0; i < trainigSet.Count; i++)
				{
					
					rez = nn.FeedForward(trainigSet[i],desiredResults[i])[0];
					Console.WriteLine(trainigSet[i][0] + " XOR " + trainigSet[i][1] + " = " + rez);
					Console.WriteLine("Eroarea dupa fw este: " + nn.E);
					
					nn.BackPropagation(desiredResults[i]);
					nn.FeedForward(trainigSet[i], desiredResults[i]);
					er = nn.E;
					Console.WriteLine("Eroarea dupa bp este: " + er);
					sumError += er;
				}
				Console.WriteLine("Eroarea totala pe pas: " + sumError);
			}

			
			Console.WriteLine(" 1 xor 0 = " + nn.FeedForward(new double[] { 1, 0 })[0]);
			Console.WriteLine(" 1 xor 1 = " + nn.FeedForward(new double[] { 1, 1 })[0]);
			Console.WriteLine(" 0 xor 1 = " + nn.FeedForward(new double[] { 0, 1 })[0]);
			Console.WriteLine(" 0 xor 0 = " + nn.FeedForward(new double[] { 0, 0 })[0]);
			Console.WriteLine(" 7 xor 0 = " + nn.FeedForward(new double[] { 7, 0 })[0]);
			Console.WriteLine(" 7 xor -7 = " + nn.FeedForward(new double[] { 7, -7 })[0]);
			Console.WriteLine(" 5 xor 5 = " + nn.FeedForward(new double[] { 5, 5 })[0]);

			Console.ReadLine();
		}

	}
	public static class ActivationFunction
	{
		public static double sigmoid(double x)
		{
			return 1.0 / (1.0 + Math.Exp(-x));
		}
		/// <summary>
		/// Sigmoid derivate.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static double Sigmoid(double s)
		{
			return s * (1.0 - s);
		}
	}
	public class NeuralNetwork
	{
		double[,] W12 { get; set; }
		double[,] W23 { get; set; }
		double[] bias2 { get; set; }
		double[] bias3 { get; set; }
		double[] Out1 { get; set; }
		double[] Out2 { get; set; }
		double[] Out3 { get; set; }
		public double E { get; set; }
		public double[] Error { get; set; }
		public double learningRate { get; set; }

		public int N1 { get; set; }
		public int N2 { get; set; }

		public int N3 { get; set; }

		Random random = new Random(1);

		public NeuralNetwork(int inputCount, int hiddenCount, int outputCount, double learningRate)
		{
			this.learningRate = learningRate;
			N1 = inputCount;
			N2 = hiddenCount;
			N3 = outputCount;
			W12 = new double[N2, N1];
			W23 = new double[N3, N2];
			bias2 = new double[N2];
			bias3 = new double[N3];
			Out1 = new double[N1];
			Out2 = new double[N2];
			Out3 = new double[N3];
			Error = new double[N3];
		}
		public void InitializeWeights()
		{
			for (int i = 0; i < N2; i++)
				for (int j = 0; j < N1; j++)
					W12[i, j] = random.NextDouble()/1000.0;
			for (int i = 0; i < N3; i++)
				for (int j = 0; j < N2; j++)
					W23[i, j] = random.NextDouble() / 1000.0;
			for (int i = 0; i < N2; i++)
				bias2[i] = random.NextDouble() / 1000.0;
			for (int i = 0; i < N3; i++)
				bias3[i] = random.NextDouble() / 1000.0;
		}
		public double[] FeedForward(double[] input, double[] desiredOut = null)
		{
			if (input.Length != N1)
				throw new ArgumentException("Input array size in not equal to the first layer count.");
			for (int i = 0; i < N1; i++)
			{
				Out1[i] = input[i];
			}
			for (int h = 0; h < N2; h++)
			{
				double sum = 0;
				for (int i = 0; i < N1; i++)
				{
					sum += W12[h, i] * input[i] + bias2[h];
				}
				Out2[h] = ActivationFunction.sigmoid(sum);
			}
			for (int o = 0; o < N3; o++)
			{
				double sum = 0;
				for (int h = 0; h < N2; h++)
				{
					sum += W23[o, h] * Out2[h] + bias3[o];
				}
				Out3[o] = ActivationFunction.sigmoid(sum);

			}
			E = 0;
			if (desiredOut != null)
				for (int o = 0; o < N3; o++)
				{
					E += Math.Pow(Out3[o] - desiredOut[o],2);
				}
			 
			return  Out3;
		}

		public double BackPropagation(double[] desiredOut)
		{
			if (desiredOut.Length != N3)
				throw new ArgumentException("Input array size in not equal to the first layer count.");

			
			for (int h = 0; h < N2; h++)
			{
				double deltaW = 0;
				for (int o = 0; o < N3; o++)
				{
					deltaW += (Out3[o] - desiredOut[o]) * ActivationFunction.Sigmoid(Out3[o]) * W23[o,h] * ActivationFunction.Sigmoid(Out2[h]);
				}
				bias2[h] += -learningRate *2 * deltaW;
				deltaW = 0;
				for (int i = 0; i < N1; i++)
				{
					deltaW = 0;
					for (int o = 0; o < N3; o++)
					{
						deltaW += (Out3[o] - desiredOut[o]) * ActivationFunction.Sigmoid(Out3[o]) * W23[o,h] * ActivationFunction.Sigmoid(Out2[h]) * Out1[i];
					}
					W12[h,i] += -learningRate  *2* deltaW;
				}
			}
			for (int o = 0; o < N3; o++)
			{
				bias3[o] += -learningRate * 2 * (Out3[0] - desiredOut[o]) * ActivationFunction.Sigmoid(Out3[o]);
				for (int h = 0; h < N2; h++)
					W23[o, h] += -learningRate * 2 * (Out3[o] - desiredOut[o]) * ActivationFunction.Sigmoid(Out3[o]) * Out2[h];
			}
			E = 0;
			for (int o = 0; o < N3; o++)
			{
				E += Math.Pow(Out3[o] - desiredOut[o],2);
			}
			return E;
		}
		private double BackPropagationOffline()
		{

			for (int o = 0; o < N3; o++)
			{
				bias3[o] += -learningRate * 2 * (Error[0]) * ActivationFunction.Sigmoid(Out3[o]);
				for (int h = 0; h < N2; h++)
					W23[h, o] += -learningRate * 2 * (Error[0]) * ActivationFunction.Sigmoid(Out3[o]) * Out2[h];
			}
			for (int h = 0; h < N2; h++)
			{
				double deltaW = 0;
				for (int o = 0; o < N3; o++)
				{
					deltaW += (Error[0]) * ActivationFunction.Sigmoid(Out3[o]) * W23[h, o] * ActivationFunction.Sigmoid(Out2[h]);
				}
				bias2[h] += -learningRate * 2 * deltaW;
				deltaW = 0;
				for (int i = 0; i < N1; i++)
				{
					deltaW = 0;
					for (int o = 0; o < N3; o++)
					{
						deltaW += (Error[0]) * ActivationFunction.Sigmoid(Out3[o]) * W23[h, o] * ActivationFunction.Sigmoid(Out2[h]) * Out1[i];
					}
					W12[i, h] += -learningRate * 2 * deltaW;
				}
			}
			E = 0;
			for (int o = 0; o < N3; o++)
			{
				E += Math.Pow(Error[0], 2);
			}
			return E;
		}
	}
}

