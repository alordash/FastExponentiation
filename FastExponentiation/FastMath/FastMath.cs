﻿using System;

public static class FastMath {
	public static double BinaryPower(double b, UInt64 e) {
		double v = 1d;
		while(e > 0) {
			if((e & 1) != 0) {
				v *= b;
			}
			b *= b;
			e >>= 1;
		}
		return v;
	}

	// Formula of magic constant
	// long doubleApproximator = (long)((1L << 52) * ((1L << 10) - 1.0730088d));
	//										  manually set value - ^^^^^^^^^
	public static long doubleApproximator = 4606853616395542500L;
	public static double OldApproximatePower(double b, double e) {
		long i = BitConverter.DoubleToInt64Bits(b);
		i = (long)(FastMath.doubleApproximator + e * (i - FastMath.doubleApproximator));
		return BitConverter.Int64BitsToDouble(i);
	}

	public static double FastPowerDividing(double b, double e) {
		// To avoid undefined behaviour near key points,
		// we can hardcode results for them, but this
		// will make function slower
		if(b == 1d || e == 0d) {
			return 1d;
		}
		var eAbs = Math.Abs(e);
		var el = Math.Ceiling(eAbs);
		var basePart = OldApproximatePower(b, eAbs / el);
		var result = BinaryPower(basePart, (ulong)el);

		// Because OldApproximatePower gives inaccurate results
		// with negative exponent, we can increase precision
		// by calculating exponent of a number in positive power
		// and then dividing 1 by result of calculation
		if(e < 0d) {
			return 1d / result;
		}
		return result;
	}

	// This function is basically FastPowerDividing, except
	// it doesn't have any extra conditions. Although these
	// conditions make function slower only by 10% than FastPowerDividing 
	public static double RawFastPowerDividing(double b, double e) {
		var eAbs = Math.Abs(e);
		var el = Math.Ceiling(eAbs);
		var basePart = OldApproximatePower(b, eAbs / el);
		return BinaryPower(basePart, (ulong)el);
	}

	public static double FastPowerFractional(double b, double e) {
		// To avoid undefined behaviour near key points,
		// we can hardcode results for them, but this
		// will make function slower
		if(b == 1d || e == 0d) {
			return 1d;
		}

		double absExp = Math.Abs(e);
		ulong eIntPart = (ulong)absExp;
		double eFractPart = absExp - eIntPart;
		double result = OldApproximatePower(b, eFractPart) * BinaryPower(b, eIntPart);
		if(e < 0d) {
			return 1d / result;
		}
		return result;
	}

	// Found this realization here: https://martin.ankerl.com/2007/10/04/optimized-pow-approximation-for-java-and-c-c/
	public static double AnotherApproxPower(double a, double b) {
		int tmp = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
		int tmp2 = (int)(b * (tmp - 1072632447) + 1072632447);
		return BitConverter.Int64BitsToDouble(((long)tmp2) << 32);
	}
}