import java.util.Random;
import java.util.Scanner;

public class FastExponentiationBenchmarkJava {
    public static Misc.MeasureResult TestTraditionalFunction(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += Math.pow(bases[i], exps[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new Misc.MeasureResult("Traditional", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static Misc.MeasureResult TestApproximateFunction(int iterationsCount, double[] bases, double[] exps) {
        var calculationResult = 0.0;
        long[] nums = new long[exps.length];
        for (int i = 0; i < exps.length; i++) {
            nums[i] = (long) exps[i];
        }
        var start = System.nanoTime();
        for (int i = 0; i < iterationsCount; i++) {
            calculationResult += FastMath.BinaryPower(bases[i], nums[i]);
        }
        var end = System.nanoTime();
        var duration = end - start;
        return new Misc.MeasureResult("Binary", duration, duration / (double) iterationsCount, iterationsCount,
                calculationResult);
    }

    public static void main(String[] args) throws Exception {
        var rand = new Random();
        rand.setSeed(System.currentTimeMillis());
        int n = 100_000_000;
        var scanner = new Scanner(System.in);
        while (true) {
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
                bases[i] = baseMul * Misc.SignedRand(rand);
                var exp = expMul * Misc.SignedRand(rand);
                exp = exp >= 0.0 ? exp : -exp;
                exps[i] = exp;
            }
            Misc.Printf("Done generating values, running benchmarks\n");

            var builtInFunctionTest = TestTraditionalFunction(n, bases, exps);
            var approximateFunctionTest = TestApproximateFunction(n, bases, exps);

            Misc.MeasureResult[] measureResults = { builtInFunctionTest, approximateFunctionTest };
            Misc.Printf("Performance results:\n");
            Misc.DisplayMeasureResult(measureResults, 20);
            Misc.Printf("Speed ");
            if (Math.abs(builtInFunctionTest.meanTime - approximateFunctionTest.meanTime) <= 5.0) {
                Misc.Printf("\033[33m");
            } else if (builtInFunctionTest.meanTime > approximateFunctionTest.meanTime) {
                Misc.Printf("\033[32m");
            } else {
                Misc.Printf("\033[31m");
            }
            Misc.Printf("x" + String.valueOf(builtInFunctionTest.meanTime / approximateFunctionTest.meanTime)
                    + "\033[0m\n");
        }
    }
}
