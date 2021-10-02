import java.util.Scanner;

class FastExponentiationJava {
    public static final int DIFFERENCE_BAR_WIDTH = 100;
    public static final int PRINT_WIDTH = 16;

    public static void main(String[] args) {
        Scanner scan = new Scanner(System.in);
        while (true) {
            double base, exponent;

            Misc.LeftPrint("Enter base:", PRINT_WIDTH);
            base = scan.nextDouble();

            Misc.LeftPrint("Enter exponent:", PRINT_WIDTH);
            exponent = scan.nextDouble();

            var realValue = Math.pow(base, exponent);
            var approximateValue = FastMath.FastPower(base, exponent);

            Misc.LeftPrint("Real result:", PRINT_WIDTH);
            Misc.Printf("%f\n", realValue);
            Misc.LeftPrint("Approximate:", PRINT_WIDTH);
            Misc.Printf("%f\n", approximateValue);

            var percentageDifference = FastMath.ToPercentage(realValue / approximateValue);
            Misc.ShowDifference(percentageDifference, PRINT_WIDTH, DIFFERENCE_BAR_WIDTH);
        }
    }
}