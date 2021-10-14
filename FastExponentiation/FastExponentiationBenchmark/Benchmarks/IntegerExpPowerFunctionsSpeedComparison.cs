using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Binary power vs built-in power", invocationCount: 10_000_000)]
	public class IntegerExpPowerFunctionsSpeedComparison : BenchmarkTemplate {
		protected new double minExp = 0d; // 67108864d;		// These are boundaries where built-in
		protected new double maxExp = 42d; // 134217728;	// power outperforms binary power

		private new Int64[] exps;

		public IntegerExpPowerFunctionsSpeedComparison() {
			FillValues();
		}

		protected new void FillValues() {
			bases = new double[N];
			exps = new Int64[N];
			var mulBase = maxBase - minBase;
			var mulExp = maxExp - minExp;
			for(int i = 0; i < N; i++) {
				double fraction = (double)i / (double)N;
				var b = fraction * mulBase + minBase;
				var e = fraction * mulExp + minExp;
				bases[i] = b;
				exps[i] = (Int64)e;
			}
		}

		protected new void NextIndex() {
			index = (index + 1) % N;
		}

		[Benchmark(Baseline = true)]
		public double BuiltIn() {
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
