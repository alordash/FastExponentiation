using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastExponentiation {
	public static class FastMath {
		public static double FastPower(double b, int e) {
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
		public static double FastPower(double b, double e) {
			return FastPower(b, (int)e);
		}

		public static float FastApproximatePower(float x, float e) {
			float xhalf = 0.5f * x;
			int i = BitConverter.ToInt32(BitConverter.GetBytes(x));
			i = (int)(0x3f7a3bea + e * (i - 0x3f7a3bea));
			x = BitConverter.ToSingle(BitConverter.GetBytes(i));
			return x;
		}
	}
}
