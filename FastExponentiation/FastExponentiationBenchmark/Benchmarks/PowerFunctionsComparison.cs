﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	[SimpleJob(id: "Fast power vs built-in power")]
	public class PowerFunctionsSpeedComparison : BenchmarkTemplate {
		public PowerFunctionsSpeedComparison() {
			bases = new double[N];
			exps = new double[N];
			for(int i = 0; i < N; i++) {
				bases[i] = i + i / 1000d;
				exps[i] = (i- 5000) / 2000d;
			}
		}

		[Benchmark(Baseline = true)]
		public double Traditional() {
			var v = Math.Pow(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double FastPower() {
			var v = FastMath.FastPower(bases[index], exps[index]);
			NextIndex();
			return v;
		}

		[Benchmark]
		public double RawFastPower() {
			var v = FastMath.RawFastPower(bases[index], exps[index]);
			NextIndex();
			return v;
		}
	}
}