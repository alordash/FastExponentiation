#pragma once
#include <math.h>

static class FastMath {
public:
	static double BinaryPower(double b, long long e);

	static const long long doubleApproximator = 4606853616395542500L;
	static double FastApproximatePower(double b, double e);

	static double FastPower(double b, double e);

	static double RawFastPower(double b, double e);

	static double ToPercentage(double ratio);

	// Not my realization
	// Found it here: https://martin.ankerl.com/2007/10/04/optimized-pow-approximation-for-java-and-c-c/
	static double AnotherApproximation(double a, double b);
};

