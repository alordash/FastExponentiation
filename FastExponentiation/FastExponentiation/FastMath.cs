using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastExponentiation {
	public static class FastMath {
		public static double BinaryPower(double b, long e) {
			double v = 1;
			while(e > 0) {
				if((e & 1) == 1) {
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
			long i = BitConverter.ToInt64(BitConverter.GetBytes(b));
			i = (long)(FastMath.doubleApproximator + e * (i - FastMath.doubleApproximator));
			b = BitConverter.ToDouble(BitConverter.GetBytes(i));
			return b;
		}

		public static double FastPower(double b, double e) {
			var el = (long)Math.Ceiling(e);
			var basePart = FastApproximatePower(b, e / el);
			return BinaryPower(basePart, el);
		}
	}
}
