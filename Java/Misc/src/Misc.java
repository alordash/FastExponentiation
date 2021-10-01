import java.util.Random;

public abstract class Misc {
    public static boolean IsUnreal(double x) {
        return Double.isNaN(x) || Double.isInfinite(x);
    }

    public static String DefaultFont = "\033[0m";

    public static String FormatErrorColor(double x) {
        return IsUnreal(x) ? "\033[31;7m" : (x > 25 ? "\033[31m" : (x > 10 ? "\033[33m" : "\033[32m"));
    }

    public static String FormatError(double x) {
        return String.format("%s%f%s", 
        FormatErrorColor(x),
        x,
        DefaultFont
        );
    }

    public static void Printf(String formatString, Object... args) {
        System.out.printf(formatString, args);
    }

    public static void Printf(char c) {
        System.out.print(c);
    }

    public static void LeftPrint(String string, int width) {
        var formatString = String.format("%%%ds", -width);
        Printf(formatString, string);
    }

    public static void LeftPrint(String string, int width, char filler) {
        var formatString = String.format(String.format("%%%ds", -width), string).replace(' ', filler);
        Printf(formatString, string);
    }

    public static void RightPrint(String string, int width) {
        LeftPrint(string, -width);
    }

    public static void RightPrint(String string, int width, char filler) {
        LeftPrint(string, -width, filler);
    }

    public static void ShowDifference(double percentageDifference, int width, int differenceBarWidth) {
        int fillAmount = 0;
        boolean differenceIsNan = false;
        
        if(IsUnreal(percentageDifference)) {
            fillAmount = differenceBarWidth;
            differenceIsNan = true;
        } else {
            fillAmount = (int)Math.round((double)differenceBarWidth * percentageDifference / 100.0);
        }
        LeftPrint("Difference:", width);
        Printf("%s%%\n", Misc.FormatError(percentageDifference));
    
        // top cap
        Printf("┌");
        RightPrint("┐", differenceBarWidth + 1, '─');
        Printf('\n');
        
        // left border
        Printf('│');
    
        if(fillAmount > differenceBarWidth) {
            fillAmount = differenceBarWidth;
        }
        int emptinessAmount = differenceBarWidth - fillAmount;
    
        // difference filler
        char filler = differenceIsNan ? '?' : '#';
        var str = FormatErrorColor(percentageDifference);
        LeftPrint(str, fillAmount + str.length(), filler);
        Printf(DefaultFont);
    
        // emptiness filler
        RightPrint("│", emptinessAmount + 1);
        Printf('\n');
    
        // bottom cap
        Printf("└");
        RightPrint("┘", differenceBarWidth + 1, '─');
        Printf("\n");
    }

    // returns random double between -1 and 1
    public static double SignedRand(Random rand) {
        return (rand.nextDouble() - 0.5) * 2.0;
    }

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
    }

    public static void DisplayMeasureResult(MeasureResult[] mrs, int width) {
        RightPrint("Function", width);
        RightPrint("Mean time", width);
        RightPrint("Total time", width);
        RightPrint("Iterations", width);
        RightPrint("Sum\n", width);

        for(int i = 0; i < mrs.length; i++) {
            var mr = mrs[i];
            RightPrint(mr.functionName, width);
            RightPrint(String.valueOf(mr.meanTime) + " ns", width);
            RightPrint(String.valueOf(mr.totalTime) + " ns", width);
            RightPrint(String.valueOf(mr.iterationsCount), width);
            RightPrint(String.valueOf(mr.calculationResult), width);
            Printf('\n');
        }
    }
}