using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace FastExponentiationPrimitiveBenchmark {
	class Program {
		const int WIDTH = 20;

		const int Iterations = 500_000;
		const int Repeats = 20;

		static BenchmarkSetUp[] benchmarkSetUps = {
			new BenchmarkSetUp{ id = 0, functionName = "Built-in", benchmarkFunction =  Math.Pow },
			new BenchmarkSetUp{ id = 1, functionName = "FP dividing", benchmarkFunction =  FastMath.FastPowerDividing },
			new BenchmarkSetUp{ id = 2, functionName = "FP fractional", benchmarkFunction = FastMath.FastPowerFractional },
			new BenchmarkSetUp{ id = 3, functionName = "Binary", benchmarkIntFunction =  FastMath.BinaryPower },
			new BenchmarkSetUp{ id = 4, functionName = "Old approx", benchmarkFunction =  FastMath.OldApproximatePower },
			new BenchmarkSetUp{ id = 5, functionName = "Another approx", benchmarkFunction =  FastMath.AnotherApproxPower }
		};

		public struct TMeasureResult {
			public String functionName;
			public double totalTime;
			public double meanTime;
			public long iterationsCount;
			public double calculationResult;
		}

		public class BenchmarkSetUp : Benchmarking.IWarmUpable {
			public int id;
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

		public static TMeasureResult RunBenchmark(String functionName, Misc.PowerIntFunction benchmarkFunction, Int64 iterationsCount, double[] bases, double[] exps) {
			var calculationResult = 0.0;
			Int64[] intExps = new Int64[iterationsCount];
			for(int i = 0; i < exps.Length; i++) {
				intExps[i] = (Int64)Math.Round(exps[i]);
			}
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			for(int i = 0; i < bases.Length; i++) {
				calculationResult += benchmarkFunction(bases[i], intExps[i]);
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

		public static TMeasureResult RunBenchmark(BenchmarkSetUp benchmarkSetUp, Int64 iterationsCount, double[] bases, double[] exps) {
			return benchmarkSetUp.benchmarkFunction != null ?
				RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkFunction, iterationsCount, bases, exps)
				: RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkIntFunction, iterationsCount, bases, exps);
		}

		public static void DisplayMeasureResults(List<TMeasureResult> mrs, int baselineIndex = 0) {
			var baselineMeanTime = mrs[baselineIndex].meanTime;
			var baselineCalculationResult = mrs[baselineIndex].calculationResult;
			Misc.Display("Function", WIDTH);
			Misc.Display("Mean time", WIDTH);
			Misc.Display("Total time", WIDTH);
			Misc.Display("Ratio", WIDTH);
			Misc.Display("Sum", WIDTH);
			Misc.Display("Sum difference", WIDTH);
			Misc.Display("Iterations", WIDTH);
			Console.WriteLine("");
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
				Misc.Display(mr.calculationResult.ToString("0.00000000E+0"), WIDTH);
				var precisionError = Misc.ToPercentage(mr.calculationResult / baselineCalculationResult);
				if(precisionError > 25d) {
					Console.ForegroundColor = ConsoleColor.DarkRed;
				} else if(precisionError > 10d) {
					Console.ForegroundColor = ConsoleColor.DarkYellow;
				} else {
					Console.ForegroundColor = ConsoleColor.DarkGreen;
				}
				Misc.Display(String.Format("{0:0.00}%", precisionError), WIDTH);
				Console.ForegroundColor = ConsoleColor.White;
				Misc.Display(mr.iterationsCount.ToString(), WIDTH);
				Console.WriteLine("");
			}
		}

		static void Main(string[] args) {
			// Setting process configuration: single-core, high priority
			Benchmarking.SetUpForBenchmarking();

			// Caching benchmark functions
			var warmUpFunctions = new List<Benchmarking.IWarmUpable>();
			foreach(var benchmarkSetUp in benchmarkSetUps) {
				warmUpFunctions.Add(benchmarkSetUp);
			}
			Benchmarking.WarmUp(warmUpFunctions);

			while(true) {
				double[] bases = new double[Iterations];
				double[] exps = new double[Iterations];
				double baseMul = Misc.GetDouble("Enter base multiplicator: ");
				double expMul = Misc.GetDouble("Enter exponent multiplicator: ");

				Console.WriteLine("Generating data values");

				for(int i = 0; i < Iterations; i++) {
					bases[i] = baseMul * Math.Abs((double)i / (double)Iterations);
					exps[i] = expMul * Math.Abs((double)i / (double)Iterations);
				}
				Console.WriteLine("Done generating values, running benchmarks");
				Console.WriteLine("C#");

				var measureResults = new Dictionary<BenchmarkSetUp, TMeasureResult>();

				var rng = new Random();
				for(int i = 0; i < Repeats; i++) {
					foreach(var benchmarkSetUp in benchmarkSetUps.OrderBy(a => rng.Next())) {
						var newRes = RunBenchmark(benchmarkSetUp, Iterations, bases, exps);
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
				DisplayMeasureResults(measureResults
					.OrderBy(x => x.Key.id)
					.Select(x => x.Value)
					.Select(x => new TMeasureResult {
						functionName = x.functionName,
						totalTime = x.totalTime,
						meanTime = x.meanTime / Repeats,
						iterationsCount = x.iterationsCount,
						calculationResult = x.calculationResult
					})
					.ToList()
				);
			}
		}
	}
}
