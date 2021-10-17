using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace FastExponentiationPrimitiveBenchmark {
	class Program {
		const int WIDTH = 20;
		const double TOO_BIG_SUM = 100_000_000_000d;
		const string TOO_BIG_MESSAGE = "Too big";

		public struct TMeasureResult {
			public String functionName;
			public double totalTime;
			public double meanTime;
			public long iterationsCount;
			public double calculationResult;
		}

		public class BenchmarkSetUp : Benchmarking.IWarmUpable {
			public String functionName;
			public Misc.PowerFunction benchmarkFunction;
			public Misc.PowerIntFunction benchmarkIntFunction;

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

		public static TMeasureResult RunBenchmark(String functionName, Misc.PowerFunction benchmarkFunction, Int64 iterationsCount, double[] bases, double[] exps) {
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

		public static TMeasureResult RunBenchmark(String functionName, Misc.PowerIntFunction benchmarkFunction, Int64 iterationsCount, double[] bases, Int64[] exps) {
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

		public static void DisplayMeasureResults(List<TMeasureResult> mrs, int baselineIndex = 0) {
			var baselineMeanTime = mrs[baselineIndex].meanTime;
			Misc.Display("Function", WIDTH);
			Misc.Display("Mean time", WIDTH);
			Misc.Display("Total time", WIDTH);
			Misc.Display("Ratio", WIDTH);
			Misc.Display("Iterations", WIDTH);
			Misc.Display("Sum\n", WIDTH);
			foreach(var mr in mrs) {
				Misc.Display(mr.functionName, WIDTH);
				Misc.Display(mr.meanTime.ToString("0.00") + " ns", WIDTH);
				Misc.Display(mr.totalTime.ToString() + " ns", WIDTH);
				var ratio = mr.meanTime / baselineMeanTime;
				if(ratio < 0.9d) {
					Console.ForegroundColor = ConsoleColor.DarkGreen;
				} else if(ratio > 1.1d) {
					Console.ForegroundColor = ConsoleColor.DarkRed;
				} else {
					Console.ForegroundColor = ConsoleColor.DarkYellow;
				}
				Misc.Display(ratio.ToString("0.00"), WIDTH);
				Console.ForegroundColor = ConsoleColor.White;
				Misc.Display(mr.iterationsCount.ToString(), WIDTH);
				Misc.Display(mr.calculationResult.ToString("0.00000000E+0") + "\n", WIDTH);

			}
		}

		static BenchmarkSetUp[] benchmarkSetUps = {
			new BenchmarkSetUp{ functionName = "Built-in", benchmarkFunction =  Math.Pow },
			new BenchmarkSetUp{ functionName = "Fast power", benchmarkFunction =  FastMath.FastPower },
			new BenchmarkSetUp{ functionName = "Fast power1", benchmarkFunction =  FastMath.FastPower1 },
			new BenchmarkSetUp{ functionName = "Raw fast power", benchmarkFunction = FastMath.RawFastPower },
			new BenchmarkSetUp{ functionName = "Binary", benchmarkIntFunction =  FastMath.BinaryPower },
			new BenchmarkSetUp{ functionName = "Approximate", benchmarkFunction =  FastMath.FastApproximatePower },
			new BenchmarkSetUp{ functionName = "Another approx", benchmarkFunction =  FastMath.AnotherApproximation }
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
			double[] bases = new double[n];
			double[] exps = new double[n];
			Int64[] expsInt = new Int64[n];
			double baseMul = Misc.GetDouble("Enter base multiplicator: ");
			double expMul = Misc.GetDouble("Enter exponent multiplicator: ");

			Console.WriteLine("Generating data values");

			for(int i = 0; i < n; i++) {
				bases[i] = Math.Abs(baseMul * ((0.14358 + i) / n) /*Misc.SignedRand(rand)*/);
				exps[i] = Math.Abs(expMul * ((0.254 + i) / n)/*Misc.SignedRand(rand)*/);
				expsInt[i] = (Int64)exps[i];
			}
			Console.WriteLine("Done generating values, running benchmarks");

			while(true) {
				Console.WriteLine("C#");


				var measureResults = new Dictionary<BenchmarkSetUp, TMeasureResult>();

				var rng = new Random();
				const int tries = 50;
				for(int i = 0; i < tries; i++) {
					foreach(var benchmarkSetUp in benchmarkSetUps.OrderBy(a => rng.Next())) {
						var newRes = RunBenchmark(benchmarkSetUp, n, bases, exps, expsInt);
						if(measureResults.TryGetValue(benchmarkSetUp, out TMeasureResult oldRes)) {
							newRes.totalTime += oldRes.totalTime;
							newRes.meanTime += oldRes.meanTime;
							newRes.iterationsCount += oldRes.iterationsCount;
							newRes.calculationResult += oldRes.calculationResult;
						}
						measureResults[benchmarkSetUp] = newRes;
					}
				}

				Console.WriteLine("Performance results:");
				DisplayMeasureResults(measureResults.Values.
					Select(x => new TMeasureResult() {
						functionName = x.functionName,
						totalTime = x.totalTime,
						meanTime = x.meanTime / tries,
						iterationsCount = x.iterationsCount,
						calculationResult = x.calculationResult,
					})
					.OrderBy(x => x.functionName)
					.ToList());

				Console.WriteLine("Press any key to repeat");
				Console.ReadLine();

			}
		}
	}
}
