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
	//								   manually set value - ^^^^^^^^^
	union {
		double d;
		long long i;
	} u = { b };
	u.i = (long long)(doubleApproximator + e * (u.i - doubleApproximator));
	return u.d;
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

// Not my realization
// Found it here: https://martin.ankerl.com/2007/10/04/optimized-pow-approximation-for-java-and-c-c/
double FastMath::AnotherFastPower(double a, double b) {
	union {
		double d;
		int x[2];
	} u = { a };
	u.x[1] = (int)(b * (u.x[1] - 1072632447) + 1072632447);
	u.x[0] = 0;
	return u.d;
}