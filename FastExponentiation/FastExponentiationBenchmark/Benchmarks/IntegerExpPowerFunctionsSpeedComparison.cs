using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Binary power vs built-in power")]
	public class IntegerExpPowerFunctionsSpeedComparison : BenchmarkTemplate {
		private Int64[] exps;

		public IntegerExpPowerFunctionsSpeedComparison() {
			var random = new Random();
			bases = new double[N];
			exps = new Int64[N];
			for(int i = 0; i < N; i++) {
				bases[i] = random.NextDouble();
				exps[i] = (long)random.Next();
			}
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
			return v;
		}
	}
}
