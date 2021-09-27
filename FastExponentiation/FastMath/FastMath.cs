﻿using System;

public static class FastMath {
	public static double BinaryPower(double b, Int64 e) {
		double v = 1;
		while(e > 0) {
			if((e & 1) != 0) {
				v *= b;
			}
			e >>= 1;
			b *= b;
		}
		return v;
	}

	public static long doubleApproximator = 4606853616395542500L;
	public static double FastApproximatePower(double b, double e) {
		//			long k = (long)((1L << 52) * ((1L << 10) - 1.0730088));
		unsafe {
			long i = *(long*)&b;
			i = (long)(FastMath.doubleApproximator + e * (i - FastMath.doubleApproximator));
			b = *(double*)&i;
		}
		return b;
	}

	public static double FastPower(double b, double e) {
		var el = (long)Math.Ceiling(Math.Abs(e));
		var basePart = FastApproximatePower(b, e / el);
		return BinaryPower(basePart, el);
	}
}