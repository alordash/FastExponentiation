using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;

public static class Benchmarking {
	public static void SetUpForBenchmarking() {
		Console.WriteLine("Setting process configuration: single-core, high priority");
		var process = Process.GetCurrentProcess();
		process.ProcessorAffinity = new IntPtr(1);
		process.PriorityClass = ProcessPriorityClass.High;
		Thread.CurrentThread.Priority = ThreadPriority.Highest;
	}

	public interface IWarmUpable {
		public object Calculate(params object[] args);
	}

	public static int WarmUpMS = 2000;
	[MethodImpl(MethodImplOptions.NoOptimization)]
	public static void WarmUp(List<IWarmUpable> warmUpFunctions) {
		Console.WriteLine("Warming up for {0}ms", WarmUpMS);
		var stopWatch = new Stopwatch();

		int i = 0;
		var sum = 0d;
		var rand = new Random();
		var length = warmUpFunctions.Count;

		stopWatch.Start();
		while(stopWatch.ElapsedMilliseconds < WarmUpMS) {
			var wuf = warmUpFunctions[i];
			sum += (double)wuf.Calculate(1000.0 * rand.NextDouble(), 1000.0 * rand.NextDouble());
			sum += (double)wuf.Calculate(42d, 42d);
			i = (i + 1) % length;
		}
		stopWatch.Stop();
		Console.WriteLine("Warm up sum = {0}", sum);
	}
}

public static class Misc {
	public static void Display(string str, int width) {
		Console.Write(str.PadLeft(width));
	}

	public static double GetDouble(string msg = "") {
		if(!String.IsNullOrEmpty(msg)) {
			Console.Write(msg);
		}
		double res;
		while(!double.TryParse(Console.ReadLine(), out res)) {
			Console.WriteLine("Entered wrong double");
		}
		return res;
	}

	public static double SignedRand(Random rand) {
		return (rand.NextDouble() - 0.5d) * 2d;
	}
}