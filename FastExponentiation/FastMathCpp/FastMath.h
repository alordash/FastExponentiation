#pragma once
#include <math.h>

static class FastMath {
public:
	static double BinaryPower(double b, long long e);

	// Formula of magic constant
	// long long doubleApproximator = (long long)((1L << 52) * ((1L << 10) - 1.0730088));
	//													manually set value - ^^^^^^^^^
	static const long long doubleApproximator = 4606853616395542500L;
	static double OldApproximatePower(double b, double e);

	static double FastPowerDividing(double b, double e);
	static double FastPowerFractional(double b, double e);

	static double RawFastPowerDividing(double b, double e);

	static double ToPercentage(double ratio);

	// Not my realization
	// Found it here: https://martin.ankerl.com/2007/10/04/optimized-pow-approximation-for-java-and-c-c/
	static double AnotherApproximatePower(double a, double b);
};

