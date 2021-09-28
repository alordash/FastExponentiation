using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace FastExponentiationBenchmark {
	public abstract class BenchmarkTemplate {
		protected double minExp = -5d;
		protected double maxExp = 5d;

		protected const int N = 10000;
		protected const double minBase = -500d;
		protected const double maxBase = 500d;

		protected double[] bases;
		protected double[] exps;
		protected int index = 0;

		protected void FillValues() {
			bases = new double[N];
			exps = new double[N];
			var mulBase = maxBase - minBase;
			var mulExp = maxExp - minExp;
			for(int i = 0; i < N; i++) {
				double fraction = (double)i / (double)N;
				var b = fraction * mulBase + minBase;
				var e = fraction * mulExp + minExp;
				bases[i] = b;
				exps[i] = e;
			}
		}

		protected void NextIndex() {
			index = (index + 1) % N;
		}
	}
}
