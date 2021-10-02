#include "FastMath.h"

double FastMath::BinaryPower(double b, long long e) {
	double v = 1.0;
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
	// long long k = (long long)((1L << 52) * ((1L << 10) - 1.0730088));
	long long i = *(long long*)&b;
	i = (long long)(doubleApproximator + e * (i - doubleApproximator));
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

	double eAbs = fabs(e);
	double el = ceil(eAbs);
	double basePart = FastApproximatePower(b, eAbs / el);

	// Because FastApproximatePower gives inaccurate results
	// with negative exponent, we can increase precision
	// by calculating exponent of a number in positive power
	// and then dividing 1 by result of calculation
	if(e < 0.0) {
		return 1.0 / BinaryPower(basePart, (long long)el);
	}
	return BinaryPower(basePart, (long long)el);
}

double FastMath::RawFastPower(double b, double e) {
	double eAbs = fabs(e);
	double el = ceil(eAbs);
	double basePart = FastApproximatePower(b, eAbs / el);
	return BinaryPower(basePart, (long long)el);
}

double FastMath::ToPercentage(double ratio) {
	if(ratio == 0.0) {
		return 0.0;
	}
	return fabs(ratio - 1.0) * 100.0;
}