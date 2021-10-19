using System;
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace FastExponentiationBenchmark {
	class Benchmark {
		static void Main(string[] args) {
			BenchmarkRunner.Run<GeneralBenchmark>();
		}
	}
}
