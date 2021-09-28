#include "FastMath.h"
double FastMath::BinaryPower(double b, long e) {
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

double FastMath::FastApproximatePower(double b, double e) {
	// Formula of magic constant
	// long k = (long)((1L << 52) * ((1L << 10) - 1.0730088));
	long i = *(long*)&b;
	i = (long)(doubleApproximator + e * (i - doubleApproximator));
	b = *(double*)&i;
	return b;
}

double FastMath::FastPower(double b, double e) {
	// To avoid undefined behaviour near key points,
	// we can hardcode results for them, but this
	// will make function slower
	if(b == 1.0 || e == 0.0) {
		return 1.0;
	}
	long el = (long)ceil(abs(e));
	double basePart = FastApproximatePower(b, abs(e) / el);

	// Because FastApproximatePower gives inaccurate results
	// with negative exponent, we can increase precision
	// by calculating exponent of a number in positive power
	// and then dividing 1 by result of calculation
	if(e < 0.0) {
		return 1.0 / BinaryPower(basePart, el);
	}
	return BinaryPower(basePart, el);
}

double FastMath::RawFastPower(double b, double e) {
	long el = (long)ceil(abs(e));
	double basePart = FastApproximatePower(b, e / el);
	return BinaryPower(basePart, el);
}

double FastMath::ToPercentage(double ratio) {
	return abs(ratio - 1.0) * 100.0;
}