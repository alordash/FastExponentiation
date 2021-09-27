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
				bases[i] = i + i / 1000d; // Math.Pow(random.NextDouble(), 4) * 1000d;
				exps[i] = (i - N / 2) / (double)(N / 2); //random.NextDouble() * 2d - 1d;
			}
		}

		[Benchmark]
		public double Traditional() {
			var v = Math.Pow(bases[ti], exps[ti]);
			ti = NextIndex(ti);
			return v;
		}

		[Benchmark]
		public double Approximate() {
			var v = FastMath.FastApproximatePower(bases[ai], exps[ai]);
			ai = NextIndex(ai);
			return v;
		}
	}
}
