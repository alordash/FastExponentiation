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
	}
}
