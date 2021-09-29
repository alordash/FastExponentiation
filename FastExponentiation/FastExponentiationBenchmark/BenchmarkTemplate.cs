using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace FastExponentiationBenchmark {
	public class PlatformsComparingConfig : ManualConfig {
		public PlatformsComparingConfig() {
			var x86core50 = Job.ShortRun
						.WithPlatform(BenchmarkDotNet.Environments.Platform.X86)
						.WithToolchain(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp50.WithCustomDotNetCliPath(@"C:\Program Files (x86)\dotnet\dotnet.exe")))
						.WithId("x86 .NET Core 5.0")
						.WithInvocationCount(1_000_000)
						.WithIterationCount(100)
						.WithUnrollFactor(1)
						.WithGcConcurrent(true)
						.WithStrategy(BenchmarkDotNet.Engines.RunStrategy.Throughput);
			var x64core50 = Job.ShortRun
						.WithPlatform(BenchmarkDotNet.Environments.Platform.X64)
						.WithToolchain(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp50.WithCustomDotNetCliPath(@"C:\Program Files\dotnet\dotnet.exe")))
						.WithId("x64 .NET Core 5.0")
						.WithInvocationCount(1_000_000)
						.WithIterationCount(100)
						.WithUnrollFactor(1)
						.WithGcConcurrent(true)
						.WithStrategy(BenchmarkDotNet.Engines.RunStrategy.Throughput);
			AddJob(x86core50);
			AddJob(x64core50);
		}
	}

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
