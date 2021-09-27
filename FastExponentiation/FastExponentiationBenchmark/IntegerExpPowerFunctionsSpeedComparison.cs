using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Binary power vs built-in power")]
	public class IntegerExpPowerFunctionsSpeedComparison {
		private const int N = 10000;

		private double[] bases;
		private Int64[] exps;
		private int ti = 0;
		private int ai = 0;

		public IntegerExpPowerFunctionsSpeedComparison() {
			var random = new Random();
			bases = new double[N];
			exps = new Int64[N];
			for(int i = 0; i < N; i++) {
				bases[i] = random.NextDouble();
				exps[i] = (long)random.Next();
			}
		}

		private int NextIndex(int i) {
			if(i % 1000 == 0) {
				Console.WriteLine(String.Format("Base: {0}, exp: {1}", bases[i], exps[i]));
			}
			return (i + 1) % N;
		}

		[Benchmark]
		public double Traditional() {
			var v = Math.Pow(bases[ti], exps[ti]);
			ti = NextIndex(ti);
			return v;
		}

		[Benchmark]
		public double Binary() {
			var v = FastMath.BinaryPower(bases[ai], exps[ai]);
			ai = NextIndex(ai);
			Console.WriteLine(String.Format("ai = {0}", ai));
			return v;
		}
	}
}
