#include <iostream>
#include <iomanip>
#include <math.h>
#include "FastMath.h"

#define _WIDTH 20
#define _SETW std::setw(_WIDTH)
#define _L std::left

#define _DEFAULT "\033[0m"

#define _NUM_IS_UNREAL(x) std::isnan(x) || !std::isfinite(x)
#define _FORMAT_ERROR_S(x) (_NUM_IS_UNREAL(x) ? "\033[31;7m" : (x > 25 ? "\033[31m" : (x > 10 ? "\033[33m" : "\033[32m")))
#define _FORMAT_ERROR(x) _FORMAT_ERROR_S(x) << x << _DEFAULT

#define _BOLD(x) "\033[1m" << x << "\033[0m"
#define _HIGHLIGHT_S "\033[7;1m"
#define _HIGHLIGHT(x) _HIGHLIGHT_S << x << _DEFAULT

struct TComparisonResult {
	double maxError = 0.0;
	double maxErrorBase = 0.0;
	double maxErrorExp = 0.0;
	double averageError = 0.0;
	long long numbersCount = 0;
};

TComparisonResult ComparePrecision(double minExp, double maxExp, double minBase, double maxBase, int maxIterationsCount) {
	TComparisonResult result;

	double expStep = std::max(0.073, abs(maxExp - minExp) / maxIterationsCount);
	double baseStep = std::max(0.01, abs(maxBase - minBase) / maxIterationsCount);

	double totalDifference = 0.0;

	for(double b = minBase; b <= maxBase; b += baseStep) {
		for(double e = minExp; e <= maxExp; e += expStep) {
			if(b == 0.0) {
				continue;
			}
			double realValue = pow(b, e);
			double approximateValue = FastMath::FastPower(b, e);
			if(_NUM_IS_UNREAL(realValue) || _NUM_IS_UNREAL(approximateValue)) {
				continue;
			}
			double diff = abs(realValue / approximateValue);
			if(_NUM_IS_UNREAL(diff) || _NUM_IS_UNREAL(realValue) || _NUM_IS_UNREAL(approximateValue)) {
				continue;
			}
			double percentageDiff = FastMath::ToPercentage(diff);
			if(percentageDiff >= result.maxError) {
				result.maxErrorBase = b;
				result.maxErrorExp = e;
				result.maxError = percentageDiff;
			}
			totalDifference += diff;
			result.numbersCount++;
		}
	}
	result.averageError = FastMath::ToPercentage(totalDifference / (result.numbersCount == 0 ? 1 : result.numbersCount));

	return result;
}

void ShowComparisonResult(TComparisonResult cr) {
	std::cout << "Maximal error: " << _FORMAT_ERROR(cr.maxError) << "% for number " << _BOLD(cr.maxErrorBase) << " in power " << _BOLD(cr.maxErrorExp) << "\n";
	std::cout << "Average error: " << _FORMAT_ERROR(cr.averageError) << "%\n";
	std::cout << "Numbers processed: " << cr.numbersCount << "\n";
}

int main() {
	while(true) {
		double minExp, maxExp, minBase, maxBase;
		long long maxIterationsCount;
		std::cout << _HIGHLIGHT_S << "ENTER PARAMETERS FOR PRECISION TEST" << _DEFAULT << "\n";
		std::cout << _SETW << _L << "Minimum exponent: " << _HIGHLIGHT_S;
		std::cin >> minExp;
		std::cout << _DEFAULT;

		std::cout << _SETW << _L << "Maximum exponent: " << _HIGHLIGHT_S;
		std::cin >> maxExp;
		std::cout << _DEFAULT;
		if(minExp > maxExp) {
			double t = maxExp;
			maxExp = minExp;
			minExp = t;
		}

		std::cout << _SETW << _L << "Minimum base: " << _HIGHLIGHT_S;
		std::cin >> minBase;
		std::cout << _DEFAULT;

		std::cout << _SETW << _L << "Maximum base: " << _HIGHLIGHT_S;
		std::cin >> maxBase;
		std::cout << _DEFAULT;
		if(minBase > maxBase) {
			double t = maxBase;
			maxBase = minBase;
			minBase = t;
		}

		std::cout << _SETW << _L << "Maximum iterations: " << _HIGHLIGHT_S;
		std::cin >> maxIterationsCount;
		std::cout << _DEFAULT << "\n";

		TComparisonResult comparisonResult = ComparePrecision(minExp, maxExp, minBase, maxBase, maxIterationsCount);
		ShowComparisonResult(comparisonResult);
		std::cout << "\n";
	}
	return 0;
}