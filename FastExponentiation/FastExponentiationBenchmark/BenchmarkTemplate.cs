using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	public abstract class BenchmarkTemplate {
		protected const int N = 10000;

		protected double[] bases;
		protected double[] exps;
		protected int index = 0;

		protected void NextIndex() {
			index = (index + 1) % N;
		}

		[IterationSetup]
		protected void ResetIndex() {
			index = 0;
		}
	}
}
