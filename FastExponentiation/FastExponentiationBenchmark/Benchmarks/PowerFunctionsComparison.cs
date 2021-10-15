using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

// ————————————————
// —	UNUSED    —
// ————————————————

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Fast power vs built-in power", invocationCount: 10_000_000)]
	public class PowerFunctionsSpeedComparison : BenchmarkTemplate {
		public PowerFunctionsSpeedComparison() {
			FillValues();
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

		[Benchmark]
		public double RawFastPowerDividing() {
			var v = FastMath.RawFastPowerDividing(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double FastPowerFractional() {
			var v = FastMath.FastPowerFractional(bases[index], exps[index]);
			NextIndex();
			return v;
		}
	}
}
