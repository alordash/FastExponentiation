public abstract class FastMath {
    public static double BinaryPower(double b, long e) {
        double v = 1;
        while (e > 0) {
            if ((e & 1) != 0) {
                v *= b;
            }
            b *= b;
            e >>= 1;
        }
        return v;
    }

    public static long doubleApproximator = 4606853616395542500L;

    public static double OldApproximatePower(double b, double e) {
        // Formula of magic constant
        // long k = (long)((1L << 52) * ((1L << 10) - 1.0730088));
		//						 manually set value - ^^^^^^^^^

        long i = Double.doubleToLongBits(b);
        i = (long) (FastMath.doubleApproximator + e * (i - FastMath.doubleApproximator));
        b = Double.longBitsToDouble(i);
        return b;
    }

    public static double FastPowerDividing(double b, double e) {
        // To avoid undefined behaviour near key points,
        // we can hardcode results for them, but this
        // will make function slower
        if (b == 1d || e == 0d) {
            return 1d;
        }
        var eAbs = Math.abs(e);
        var el = Math.ceil(eAbs);
        var basePart = OldApproximatePower(b, eAbs / el);

        // Because FastApproximatePower gives inaccurate results
        // with negative exponent, we can increase precision
        // by calculating exponent of a number in positive power
        // and then dividing 1 by result of calculation
        if (e < 0d) {
            return 1d / BinaryPower(basePart, (long) el);
        }
        return BinaryPower(basePart, (long) el);
    }

    public static double FastPowerFractional(double b, double e) {
        // To avoid undefined behaviour near key points,
        // we can hardcode results for them, but this
        // will make function slower
        if (b == 1d || e == 0d) {
            return 1d;
        }

        double absExp = Math.abs(e);
        long eIntPart = (long)absExp;
        double eFractPart = absExp - eIntPart;
        double result = OldApproximatePower(b, eFractPart) * BinaryPower(b, eIntPart);
        if(e < 0d) {
            return 1d / result;
        }
        return result;
    }

    public static double RawFastPowerDividing(double b, double e) {
        var eAbs = Math.abs(e);
        var el = Math.ceil(eAbs);
        var basePart = OldApproximatePower(b, eAbs / el);
        return BinaryPower(basePart, (long) el);
    }

    public static double AnotherApproximation(final double a, final double b) {
        final int x = (int) (Double.doubleToLongBits(a) >> 32);
        final int y = (int) (b * (x - 1072632447) + 1072632447);
        return Double.longBitsToDouble(((long) y) << 32);
    }
}