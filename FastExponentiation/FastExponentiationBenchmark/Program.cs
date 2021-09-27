using System;
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace FastExponentiationBenchmark {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Available benchmarks:");
			Console.WriteLine("1. Fast power VS built-in power");
			Console.WriteLine("2. Binary power vs built-in power (for integers exponents only)");
			Console.WriteLine("3. Approximate power vs built-in power (-1 <= exponent <= 1)");
			while(true) {
				Console.WriteLine("Enter benchmark's index to run it (1/2/3):");
				string input = Console.ReadLine();
				switch(input) {
					case "1":
						BenchmarkRunner.Run<PowerFunctionsSpeedComparison>();
						break;
					case "2":
						BenchmarkRunner.Run<IntegerExpPowerFunctionsSpeedComparison>();
						break;
					case "3":
						BenchmarkRunner.Run<ApproximatePowerFunctionsSpeedComparison>();
						break;
					default:
						Console.WriteLine("Entered wrong number");
						break;
				}
			}
		}
	}
}
