using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace FastExponentiationPrimitiveBenchmark {
	class Program {
		const int WIDTH = 20;

		public struct TMeasureResult {
			public String functionName;
			public double totalTime;
			public double meanTime;
			public long iterationsCount;
			public double calculationResult;
		}

		public delegate double BenchmarkFunction(double b, double e);
		public delegate double BenchmarkIntFunction(double b, Int64 e);

		public class BenchmarkSetUp : Benchmarking.IWarmUpable {
			public String functionName;
			public BenchmarkFunction benchmarkFunction;
			public BenchmarkIntFunction benchmarkIntFunction;

			public object Calculate(params object[] args) {
				var a1 = args[0] as IConvertible;
				if(a1 == null || a1 is not double) {
					return 0d;
				}
				var a2 = args[1] as IConvertible;
				if(a2 == null || a2 is not double) {
					return 0d;
				}
				var b = a1.ToDouble(null);
				var e = a2.ToDouble(null);

				if(benchmarkFunction is null) {
					return benchmarkIntFunction(b, (Int64)e);
				}
				return benchmarkFunction(b, e);
			}
		}

		public static TMeasureResult RunBenchmark(String functionName, BenchmarkFunction benchmarkFunction, Int64 iterationsCount, double[] bases, double[] exps) {
			var calculationResult = 0.0;
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			for(int i = 0; i < bases.Length; i++) {
				calculationResult += benchmarkFunction(bases[i], exps[i]);
			}
			stopWatch.Stop();
			var elapsed = stopWatch.ElapsedMilliseconds * 1_000_000d;

			return new TMeasureResult {
				functionName = functionName,
				totalTime = elapsed,
				meanTime = elapsed / (double)iterationsCount,
				iterationsCount = iterationsCount,
				calculationResult = calculationResult
			};
		}

		public static TMeasureResult RunBenchmark(String functionName, BenchmarkIntFunction benchmarkFunction, Int64 iterationsCount, double[] bases, Int64[] exps) {
			var calculationResult = 0.0;
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			for(int i = 0; i < bases.Length; i++) {
				calculationResult += benchmarkFunction(bases[i], exps[i]);
			}
			stopWatch.Stop();
			var elapsed = stopWatch.ElapsedMilliseconds * 1_000_000d;

			return new TMeasureResult {
				functionName = functionName,
				totalTime = elapsed,
				meanTime = elapsed / (double)iterationsCount,
				iterationsCount = iterationsCount,
				calculationResult = calculationResult
			};
		}

		public static TMeasureResult RunBenchmark(BenchmarkSetUp benchmarkSetUp, Int64 iterationsCount, double[] bases, double[] exps, Int64[] expsInt) {
			return benchmarkSetUp.benchmarkFunction != null ?
				RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkFunction, iterationsCount, bases, exps)
				: RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkIntFunction, iterationsCount, bases, expsInt);
		}

		public static void DisplayMeasureResults(List<TMeasureResult> mrs) {
			const int sumWidth = 10;
			Misc.Display("Function", WIDTH);
			Misc.Display("Mean time", WIDTH);
			Misc.Display("Total time", WIDTH);
			Misc.Display("Iterations", WIDTH);
			Misc.Display("Sum\n", WIDTH + sumWidth);
			foreach(var mr in mrs) {
				Misc.Display(mr.functionName, WIDTH);
				Misc.Display(mr.meanTime.ToString() + "ns", WIDTH);
				Misc.Display(mr.totalTime.ToString() + "ns", WIDTH);
				Misc.Display(mr.iterationsCount.ToString(), WIDTH);
				Misc.Display(mr.calculationResult.ToString() + "\n", WIDTH + sumWidth);
			}
		}

		static BenchmarkSetUp[] benchmarkSetUps = {
			new BenchmarkSetUp{ functionName = "Built-in", benchmarkFunction =  Math.Pow },
			new BenchmarkSetUp{ functionName = "Fast power", benchmarkFunction =  FastMath.FastPower },
			new BenchmarkSetUp{ functionName = "Approximate", benchmarkFunction =  FastMath.FastApproximatePower },
			new BenchmarkSetUp{ functionName = "Binary power", benchmarkIntFunction =  FastMath.BinaryPower },
			new BenchmarkSetUp{ functionName = "Raw fast power", benchmarkFunction = FastMath.RawFastPower }
		};

		static void Main(string[] args) {
			// Setting process configuration: single-core, high priority
			Benchmarking.SetUpForBenchmarking();

			// Caching benchmark functions
			var warmUpFunctions = new List<Benchmarking.IWarmUpable>();
			foreach(var benchmarkSetUp in benchmarkSetUps) {
				warmUpFunctions.Add(benchmarkSetUp);
			}
			Benchmarking.WarmUp(warmUpFunctions);

			var rand = new Random();
			int n = 1_000_000;
			while(true) {
				double baseMul = Misc.GetDouble("Enter base multiplicator: ");
				double expMul = Misc.GetDouble("Enter exponent multiplicator: ");

				Console.WriteLine("Generating data values");
				double[] bases = new double[n];
				double[] exps = new double[n];
				Int64[] expsInt = new Int64[n];
				for(int i = 0; i < n; i++) {
					bases[i] = Math.Abs(baseMul * Misc.SignedRand(rand));
					exps[i] = Math.Abs(expMul * Misc.SignedRand(rand));
					expsInt[i] = (Int64)exps[i];
				}

				List<TMeasureResult> measureResults = new List<TMeasureResult>();
				Console.WriteLine("Done generating values, running benchmarks");

				foreach(var benchmarkSetUp in benchmarkSetUps) {
					measureResults.Add(RunBenchmark(benchmarkSetUp, n, bases, exps, expsInt));
				}

				Console.WriteLine("Performance results:");
				DisplayMeasureResults(measureResults);

				//Console.Write("Speed ");
				//if(Math.Abs(builtInMethodTest.meanTime - fastPowerMethodTest.meanTime) <= 5.0) {
				//	Console.ForegroundColor = ConsoleColor.DarkYellow;
				//} else if(builtInMethodTest.meanTime > fastPowerMethodTest.meanTime) {
				//	Console.ForegroundColor = ConsoleColor.DarkGreen;
				//} else {
				//	Console.ForegroundColor = ConsoleColor.DarkRed;
				//}
				//Console.WriteLine(String.Format("x{0}", builtInMethodTest.meanTime / fastPowerMethodTest.meanTime));
				//Console.ForegroundColor = ConsoleColor.White;
			}
		}
	}
}
