using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "General benchmark", invocationCount: 100_000_000)]
	public class GeneralBenchmark : BenchmarkTemplate {
		private Int64[] intExps;

		public GeneralBenchmark() {
			FillValues();
		}

		protected new void FillValues() {
			bases = new double[N];
			exps = new double[N];
			intExps = new Int64[N];
			var mulBase = maxBase - minBase;
			var mulExp = maxExp - minExp;
			for(int i = 0; i < N; i++) {
				double fraction = (double)i / (double)N;
				var b = fraction * mulBase + minBase;
				var e = fraction * mulExp + minExp;
				bases[i] = b;
				exps[i] = e;
				intExps[i] = (Int64)e;
			}
		}

		[Benchmark(Baseline = true)]
		public double BuiltIn() {
			var v = Math.Pow(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double FastPowerDividing() {
			var v = FastMath.FastPowerDividing(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		//[Benchmark]
		//public double RawFastPower() {
		//	var v = FastMath.RawFastPowerDividing(bases[index], exps[index]);
		//	NextIndex();
		//	return v;
		//}

		[Benchmark]
		public double FastPowerFractional() {
			var v = FastMath.FastPowerFractional(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double BinaryPower() {
			var v = FastMath.BinaryPower(bases[index], intExps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double OldApproximation() {
			var v = FastMath.OldApproximatePower(bases[index], intExps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double AnotherApproximation() {
			var v = FastMath.AnotherApproxPower(bases[index], intExps[index]);
			NextIndex();
			return v;
		}
	}
}
