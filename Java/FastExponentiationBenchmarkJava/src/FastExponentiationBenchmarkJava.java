import java.util.Collections;
import java.util.Scanner;
import java.util.Map;
import java.util.HashMap;
import java.util.Arrays;
import java.util.List;

public class FastExponentiationBenchmarkJava {
    public static final int WIDTH = 20;

    static final int Iterations = 500_000;
    static final int Repeats = 20;

    public static class MeasureResult {
        String functionName;
        double totalTime;
        double meanTime;
        long iterationsCount;
        double calculationResult;

        public MeasureResult(String functionName, double totalTime, double meanTime, long iterationsCount,
                double calculationResult) {
            this.functionName = functionName;
            this.totalTime = totalTime;
            this.meanTime = meanTime;
            this.iterationsCount = iterationsCount;
            this.calculationResult = calculationResult;
        }

        public void Display(int width, double baselineMeanTime) {
            Misc.RightPrint(functionName, width);
            Misc.RightPrint(String.format("%.2f", meanTime) + " ns", width);
            Misc.RightPrint(String.valueOf(totalTime) + " ns", width);
            var ratio = meanTime / baselineMeanTime;
            if (ratio < 0.9d) {
                Misc.Printf("\033[32m");
            } else if (ratio > 1.1d) {
                Misc.Printf("\033[31m");
            } else {
                Misc.Printf("\033[33m");
            }
            Misc.RightPrint(String.format("%.2f", ratio), width);
            Misc.Printf("\033[0m");
            Misc.RightPrint(String.valueOf(iterationsCount), width);
            Misc.RightPrint(String.format("%1.8e", calculationResult), width);
            Misc.Printf('\n');
        }
    }

    public static void DisplayMeasureResults(MeasureResult[] mrs, int width, int baselineIndex) {
        var baselineMeanTime = mrs[baselineIndex].meanTime;
        Misc.RightPrint("Function", width);
        Misc.RightPrint("Mean time", width);
        Misc.RightPrint("Total time", width);
        Misc.RightPrint("Ratio", width);
        Misc.RightPrint("Iterations", width);
        Misc.RightPrint("Sum\n", width);

        for (int i = 0; i < mrs.length; i++) {
            mrs[i].Display(width, baselineMeanTime);
        }
    }

    public static MeasureResult TestBuiltIn(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += Math.pow(bases[i], exps[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new MeasureResult("Built-in", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult TestFastPowerDividing(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += FastMath.FastPowerDividing(bases[i], exps[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new MeasureResult("FP dividing", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult TestFastPowerFractional(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += FastMath.FastPowerFractional(bases[i], exps[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new MeasureResult("FP fractional", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult TestBinary(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        long nums[] = new long[exps.length];
        for (int i = 0; i < exps.length; i++) {
            nums[i] = (long) Math.round(exps[i]);
        }
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += FastMath.BinaryPower(bases[i], nums[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new MeasureResult("Binary", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult TestOldApproximatePower(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += FastMath.OldApproximatePower(bases[i], exps[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new MeasureResult("Old approx", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult TestAnotherApproximate(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        long[] nums = new long[exps.length];
        for (int i = 0; i < exps.length; i++) {
            nums[i] = (long) exps[i];
        }
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += FastMath.AnotherApproximation(bases[i], nums[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new MeasureResult("Another approx", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult RunBenchmark(int benchmarkId, int iterationsCount, double[] bases,
            double[] exps) {
        switch (benchmarkId) {
            case 0:
                return TestBuiltIn(iterationsCount, bases, exps);
            case 1:
                return TestFastPowerDividing(iterationsCount, bases, exps);
            case 2:
                return TestFastPowerFractional(iterationsCount, bases, exps);
            case 3:
                return TestBinary(iterationsCount, bases, exps);
            case 4:
                return TestOldApproximatePower(iterationsCount, bases, exps);
            case 5:
                return TestAnotherApproximate(iterationsCount, bases, exps);
            default:
                break;
        }
        return null;
    }

    public static List<Integer> BenchmarksOrder = Arrays.asList(new Integer[] {0, 1, 2, 3, 4, 5});

    public static void main(String[] args) throws Exception {
        var scanner = new Scanner(System.in);

        while (true) {
            Misc.Printf("Java\n");

            Misc.Printf("Enter base multiplicator: ");
            double baseMul = scanner.nextDouble();
            Misc.Printf("Enter exponent multiplicator: ");
            double expMul = scanner.nextDouble();

            Misc.Printf("Generating data values\n");
            double[] bases = new double[Iterations];
            double[] exps = new double[Iterations];
            for (int i = 0; i < Iterations; i++) {
                bases[i] = baseMul * Math.abs((double)i / (double)Iterations);
                exps[i] = expMul * Math.abs((double)i / (double)Iterations);
            }
            Misc.Printf("Done generating values, running benchmarks\n");

            Collections.shuffle(BenchmarksOrder);
            Map<Integer, MeasureResult> measureResults = new HashMap<Integer, MeasureResult>();

            for(int i = 0; i < Repeats; i++) {
                for (var benchmarkId : BenchmarksOrder) {
                    var newRes = RunBenchmark(benchmarkId, Iterations, bases, exps);
                    var oldRes = measureResults.get(benchmarkId);
                    if(oldRes != null) {
                        newRes.totalTime += oldRes.totalTime;
                        newRes.meanTime += oldRes.meanTime;
                        newRes.iterationsCount += oldRes.iterationsCount;
                        newRes.calculationResult += oldRes.calculationResult;
                    }
                    measureResults.put(benchmarkId, newRes);
                }
            }

            Misc.Printf("Performance results:\n");
            var mrs = measureResults.values().toArray(new MeasureResult[measureResults.size()]);
            for(int i = 0; i < mrs.length; i++) {
                mrs[i].meanTime /= Repeats;
            }
            DisplayMeasureResults(mrs, WIDTH, 0);
        }
    }
}
