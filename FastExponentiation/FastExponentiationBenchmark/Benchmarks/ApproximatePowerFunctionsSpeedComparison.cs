using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace FastExponentiationBenchmark {

//	[Config(typeof(PlatformsComparingConfig))]
	[SimpleJob(id: "Approximate power vs built-in power", invocationCount: 1_000_000)]
	public class ApproximatePowerFunctionsSpeedComparison : BenchmarkTemplate {

		public ApproximatePowerFunctionsSpeedComparison() {
			minExp = -1d;
			maxExp = 1d;
			FillValues();
		}

		[Benchmark(Baseline = true)]
		public double Traditional() {
			var v = Math.Pow(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double Approximate() {
			var v = FastMath.FastApproximatePower(bases[index], exps[index]);
			NextIndex();
			return v;
		}
	}
}
