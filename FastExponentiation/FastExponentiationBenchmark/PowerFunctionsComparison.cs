using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	public class PowerFunctionsComparison {
		private const int N = 10000;

		private double[] bases;
		private double[] exps;
		private int ti = 0;
		private int ai = 0;

		public PowerFunctionsComparison() {
			var random = new Random(42);
			bases = new double[N];
			exps = new double[N];
			for(int i = 0; i < N; i++) {
				bases[i] = random.NextDouble();
				exps[i] = random.NextDouble();
			}
		}

		private int NextIndex(int i) {
			return (i + 1) % N;
		}

		[Benchmark]
		public double Traditional() {
			var v = Math.Pow(bases[ti], exps[ti]);
			ti = NextIndex(ti);
			return v;
		}

		[Benchmark]
		public double Approximate() {
			var v = FastMath.FastPower(bases[ai], exps[ai]);
			ai = NextIndex(ai);
			return v;
		}
	}
}
