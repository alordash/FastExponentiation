import java.util.Scanner;

public class PrecisionComparisonJava {
    public static final int WIDTH = 20;

    public static class ComparisonResult {
        double maxError = 0d;
        double maxErrorBase = 0d;
        double maxErrorExp = 0d;
        double averageError = 0d;
        long numbersCount = 0;

        public ComparisonResult() {
        };

        public void Display() {
            Misc.Printf("Maximal error: " + Misc.FormatError(maxError) + "%% for number " + String.valueOf(maxErrorBase)
                    + " in power " + String.valueOf(maxErrorExp) + "\n");
            Misc.Printf("Average error: " + Misc.FormatError(averageError) + "%%\n");
            Misc.Printf("Numbers processed: %d\n", numbersCount);
        }
    }

    public static ComparisonResult ComparePrecision(double minExp, double maxExp, double minBase, double maxBase,
            long maxIterationsCount) {
        var result = new ComparisonResult();

        var expStep = Math.max(0.075, Math.abs(maxExp - minExp) / maxIterationsCount);
        var baseStep = Math.max(0.01, Math.abs(maxBase - minBase) / maxIterationsCount);

        var totalDifference = 0.0;

        for (double b = minBase; b <= maxBase; b += baseStep) {
            for (double e = minExp; e <= maxExp; e += expStep) {
                if (b == 0.0) {
                    continue;
                }
                var realValue = Math.pow(b, e);
                var approximateValue = FastMath.FastPowerDividing(b, e);
                if (Misc.IsUnreal(realValue) || Misc.IsUnreal(approximateValue)) {
                    continue;
                }
                var diff = Math.abs(realValue / approximateValue);
                if (Misc.IsUnreal(diff)) {
                    continue;
                }
                var percentageDiff = Misc.ToPercentage(diff);
                if (percentageDiff >= result.maxError) {
                    result.maxErrorBase = b;
                    result.maxErrorExp = e;
                    result.maxError = percentageDiff;
                }
                totalDifference += diff;
                result.numbersCount++;
            }
        }
        result.averageError = Misc.ToPercentage(totalDifference / (result.numbersCount == 0 ? 1 : result.numbersCount));
        return result;
    }

    public static void main(String[] args) throws Exception {
        var scanner = new Scanner(System.in);

        while (true) {
            double minExp, maxExp, minBase, maxBase;
            long maxIterationsCount;
            Misc.Printf("ENTER PARAMETERS FOR PRECISION TEST\n");

            Misc.LeftPrint("Minimum exponent: ", WIDTH);
            minExp = scanner.nextDouble();

            Misc.LeftPrint("Maximum exponent: ", WIDTH);
            maxExp = scanner.nextDouble();
            if (minExp > maxExp) {
                var t = maxExp;
                maxExp = minExp;
                minExp = t;
            }

            Misc.LeftPrint("Minimum base: ", WIDTH);
            minBase = scanner.nextDouble();

            Misc.LeftPrint("Maximum base: ", WIDTH);
            maxBase = scanner.nextDouble();
            if (minBase > maxBase) {
                var t = maxBase;
                maxBase = minBase;
                minBase = t;
            }

            Misc.LeftPrint("Maximum iterations: ", WIDTH);
            maxIterationsCount = scanner.nextLong();
            Misc.Printf('\n');

            var comparisonResult = ComparePrecision(minExp, maxExp, minBase, maxBase, maxIterationsCount);
            comparisonResult.Display();
            Misc.Printf('\n');
        }
    }
}
