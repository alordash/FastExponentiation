using System;
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace FastExponentiationBenchmark {
	class Program {
		static void Main(string[] args) {
			var summary = BenchmarkRunner.Run<PowerFunctionsComparison>();
			Console.WriteLine("Benchmark done");
			while(true) {
				int count;
				Console.WriteLine("Enter count:");
				while(!int.TryParse(Console.ReadLine(), out count)) {
					Console.WriteLine("Entered wrong count");
				}

				double Exp;
				Console.WriteLine("Enter exponent:");
				while(!double.TryParse(Console.ReadLine(), out Exp)) {
					Console.WriteLine("Entered wrong exponent");
				}

				var random = new Random();
				var bases = new double[count];
				for(int i = 0; i < count; i++) {
					bases[i] = random.NextDouble();
				}

				var realValues = new double[count];
				var approximateValues = new double[count];
				var stopWatch = new Stopwatch();
				stopWatch.Start();
				for(int i = 0; i < count; i++) {
					realValues[i] = Math.Pow(bases[i], Exp);
				}
				stopWatch.Stop();
				Console.WriteLine(String.Format("Traditional method took {0}ms for raising {1} numbers to power {2}", stopWatch.ElapsedMilliseconds, count, Exp));
				stopWatch.Reset();

				stopWatch.Start();
				for(int i = 0; i < count; i++) {
					approximateValues[i] = FastMath.FastPower(bases[i], Exp);
				}
				stopWatch.Stop();
				Console.WriteLine(String.Format("Approximate method took {0}ms for raising {1} numbers to power {2}", stopWatch.ElapsedMilliseconds, count, Exp));
			}
		}
	}
}
