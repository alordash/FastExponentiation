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
		protected int ti = 0;
		protected int ai = 0;

		protected int NextIndex(int i) {
			return (i + 1) % N;
		}
	}
}
