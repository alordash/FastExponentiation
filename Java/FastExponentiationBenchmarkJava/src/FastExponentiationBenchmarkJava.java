import java.util.Random;
import java.util.Scanner;

public class FastExponentiationBenchmarkJava {
    public static final int WIDTH = 20;
    public static final double TOO_BIG_SUM = 100_000_000_000.0d;
    public static final String TOO_BIG_MESSAGE = "Too big";

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
            if (calculationResult > TOO_BIG_SUM) {
                Misc.RightPrint(TOO_BIG_MESSAGE + "\n", width);
            } else {
                Misc.RightPrint(String.format("%.3f\n", calculationResult), width);
            }
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
        return new MeasureResult("Fast power", duration, duration / (double) iterationsCount, iterationsCount,
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
        return new MeasureResult("Approximate", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static MeasureResult TestBinary(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        long nums[] = new long[exps.length];
        for (int i = 0; i < exps.length; i++) {
            nums[i] = (long) exps[i];
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

    public static void main(String[] args) throws Exception {
        var rand = new Random();
        rand.setSeed(System.currentTimeMillis());
        int n = 1_000_000;
        var scanner = new Scanner(System.in);
        while (true) {
            Misc.Printf("Java\n");
            double baseMul = 100002.9;
            double expMul = 12.1;

            Misc.Printf("Enter base multiplicator: ");
            baseMul = scanner.nextDouble();
            Misc.Printf("Enter exponent multiplicator: ");
            expMul = scanner.nextDouble();

            Misc.Printf("Generating data values\n");
            double[] bases = new double[n];
            double[] exps = new double[n];
            for (int i = 0; i < n; i++) {
                bases[i] = Math.abs(baseMul * Misc.SignedRand(rand));
                exps[i] = Math.abs(expMul * Misc.SignedRand(rand));
            }
            Misc.Printf("Done generating values, running benchmarks\n");

            MeasureResult[] measureResults = { TestBuiltIn(n, bases, exps),
                    TestFastPowerDividing(n, bases, exps), TestFastPowerFractional(n, bases, exps),
                    TestBinary(n, bases, exps), TestOldApproximatePower(n, bases, exps),
                    TestAnotherApproximate(n, bases, exps), };
            Misc.Printf("Performance results:\n");
            DisplayMeasureResults(measureResults, WIDTH, 0);
        }
    }
}
