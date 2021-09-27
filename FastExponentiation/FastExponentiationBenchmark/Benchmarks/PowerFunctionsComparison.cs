using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Fast power vs built-in power")]
	public class PowerFunctionsSpeedComparison : BenchmarkTemplate {
		public PowerFunctionsSpeedComparison() {
			var random = new Random();
			bases = new double[N];
			exps = new double[N];
			for(int i = 0; i < N; i++) {
				bases[i] = Math.Pow(random.NextDouble(), 4) * 1000d;
				exps[i] = Math.Pow(random.NextDouble(), 4) * 1000d;
			}
		}

		[Benchmark]
		public double FastPower() {
			var v = FastMath.FastPower(bases[ai], exps[ai]);
			ai = NextIndex(ai);
			return v;
		}
	}
}
