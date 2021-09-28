using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Binary power vs built-in power")]
	public class IntegerExpPowerFunctionsSpeedComparison : BenchmarkTemplate {
		private new Int64[] exps;

		public IntegerExpPowerFunctionsSpeedComparison() {
			bases = new double[N];
			exps = new Int64[N];
			for(int i = 0; i < N; i++) {
				bases[i] = i + i / 1000d;
				exps[i] = i;
			}
		}

		[Benchmark(Baseline = true)]
		public double Traditional() {
			var v = Math.Pow(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double Binary() {
			var v = FastMath.BinaryPower(bases[index], exps[index]);
			NextIndex();
			return v;
		}
	}
}
