using System;
using System.Runtime.InteropServices;

public static class FastMath {
	public static double BinaryPower(double b, Int64 e) {
		double v = 1;
		while(e > 0) {
			if((e & 1) != 0) {
				v *= b;
			}
			b *= b;
			e >>= 1;
		}
		return v;
	}

	// Using union-kind struct to avoid unsafe code
	[StructLayout(LayoutKind.Explicit)]
	public struct TDoubleLongUnion {
		[FieldOffset(0)] public double d;
		[FieldOffset(0)] public long i;
	}

	// Formula of magic constant
	// long doubleApproximator = (long)((1L << 52) * ((1L << 10) - 1.0730088d));
	//										  manually set value - ^^^^^^^^^
	public static long doubleApproximator = 4606853616395542500L;
	public static double FastApproximatePower(double b, double e) {
		TDoubleLongUnion u = new TDoubleLongUnion() { d = b };
		u.i = (long)(FastMath.doubleApproximator + e * (u.i - FastMath.doubleApproximator));
		b = u.d;
		return u.d;
	}

	public static double FastPower(double b, double e) {
		// To avoid undefined behaviour near key points,
		// we can hardcode results for them, but this
		// will make function slower
		if(b == 1d || e == 0d) {
			return 1d;
		}
		var eAbs = Math.Abs(e);
		var el = Math.Ceiling(eAbs);
		var basePart = FastApproximatePower(b, eAbs / el);

		// Because FastApproximatePower gives inaccurate results
		// with negative exponent, we can increase precision
		// by calculating exponent of a number in positive power
		// and then dividing 1 by result of calculation
		if(e < 0d) {
			return 1d / BinaryPower(basePart, (long)el);
		}
		return BinaryPower(basePart, (long)el);
	}

	public static double RawFastPower(double b, double e) {
		var eAbs = Math.Abs(e);
		var el = Math.Ceiling(eAbs);
		var basePart = FastApproximatePower(b, eAbs / el);
		return BinaryPower(basePart, (long)el);
	}

	// Technical method not used in calculation
	public static double ToPercentage(double ratio) {
		if(ratio == 0d) {
			return 0d;
		}
		return Math.Abs(ratio - 1d) * 100d;
	}

	// Not my realization
	// Found it here: https://martin.ankerl.com/2007/10/04/optimized-pow-approximation-for-java-and-c-c/
	public static double AnotherApproximation(double a, double b) {
		int tmp = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
		int tmp2 = (int)(b * (tmp - 1072632447) + 1072632447);
		return BitConverter.Int64BitsToDouble(((long)tmp2) << 32);
	}

}