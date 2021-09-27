using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Approximate power vs built-in power")]
	public class ApproximatePowerFunctionsSpeedComparison : BenchmarkTemplate {
		public ApproximatePowerFunctionsSpeedComparison() {
			var random = new Random();
			bases = new double[N];
			exps = new double[N];
			for(int i = 0; i < N; i++) {
				bases[i] = Math.Pow(random.NextDouble(), 4) * 1000d;
				exps[i] = random.NextDouble();
			}
		}

		[Benchmark]
		public double Approximate() {
			var v = FastMath.FastApproximatePower(bases[ai], exps[ai]);
			ai = NextIndex(ai);
			return v;
		}
	}
}
