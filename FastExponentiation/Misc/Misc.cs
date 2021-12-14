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
		Console.WriteLine("Warm up sum = {0} (ignore this)", sum);
	}
}

public static class Misc {
	public delegate double PowerFunction(double b, double e);
	public delegate double PowerIntFunction(double b, UInt64 e);
	public class PowerFunctionModel : IEquatable<PowerFunctionModel> {
		public PowerFunction Function;
		public string Name;

		public bool Equals(PowerFunctionModel powerFunctionObject) {
			return this.Name == powerFunctionObject.Name;
		}
	}

	public static List<PowerFunctionModel> PowerFunctionModels = new List<PowerFunctionModel>() {
		new PowerFunctionModel { Function = Math.Pow, Name = "Built-in power" },
		new PowerFunctionModel { Function = FastMath.FastPowerDividing, Name = "Fast power dividing" },
		new PowerFunctionModel { Function = FastMath.FastPowerFractional, Name = "Fast power fractional" },
		new PowerFunctionModel { Function = FastMath.OldApproximatePower, Name = "Old power approximation"},
		new PowerFunctionModel { Function = FastMath.AnotherApproxPower, Name = "Another power approximation"}
	};

	public static int BaselinePowerFunctionIndex = Misc.PowerFunctionModels.IndexOf(new Misc.PowerFunctionModel { Name = "Built-in power" });

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

	public static string GetErrorColor(double percentageError) {
		if(percentageError > 25d) {
			return "#d52c2c";
		} else if(percentageError > 10) {
			return "#cccc3c";
		}
		return "#4ebd4e";
	}

	public static string FormatDouble(double value) {
		return value.ToString("0.0000");
	}

	public static string FormatError(double error) {
		return String.Format("{0:0.00}", error);
	}

	public static double ToPercentage(double ratio) {
		if(ratio == 0d) {
			return 0d;
		}
		return Math.Abs(ratio - 1d) * 100d;
	}
}