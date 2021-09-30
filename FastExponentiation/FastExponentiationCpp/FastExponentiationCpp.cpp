#include <iostream>
#include <math.h>
#include <iomanip>
#include "FastMath.h"

#define DIFFERENCE_BAR_WIDTH 100

#define _WIDTH 16
#define _SETW std::setw(_WIDTH)
#define _L std::left
#define _R std::right
#define _FILL(x) std::setfill(x)

#define _DEFAULT "\033[0m"

#define _NUM_IS_UNREAL(x) std::isnan(x) || !std::isfinite(x)
#define _FORMAT_ERROR_S(x) (_NUM_IS_UNREAL(x) ? "\033[31;7m" : (x > 25 ? "\033[31m" : (x > 10 ? "\033[33m" : "\033[32m")))
#define _FORMAT_ERROR(x) _FORMAT_ERROR_S(x) << x << _DEFAULT

void ShowDifference(double percentageDifference) {
	int fillAmount = 0;
	bool differenceIsNan = false;
	std::cout << _R;
	if(_NUM_IS_UNREAL(percentageDifference)) {
		fillAmount = DIFFERENCE_BAR_WIDTH;
		differenceIsNan = true;
	} else {
		fillAmount = (int)round((double)DIFFERENCE_BAR_WIDTH * percentageDifference / 100.0);
	}
	std::cout << "Difference: " << _FORMAT_ERROR(percentageDifference) << "%\n";

	// top cap
	std::cout << char(218) << std::setfill(char(196)) << std::setw(DIFFERENCE_BAR_WIDTH + 1) << char(191) << "\n";
	// left border
	std::cout << char(179);

	if(fillAmount > DIFFERENCE_BAR_WIDTH) {
		fillAmount = DIFFERENCE_BAR_WIDTH;
	}
	int emptinessAmount = DIFFERENCE_BAR_WIDTH - fillAmount;

	// difference filler
	std::cout << _FORMAT_ERROR_S(percentageDifference) << (differenceIsNan ? _FILL('?') : _FILL('#')) << std::setw(fillAmount + 4) << _DEFAULT;

	// emptiness filler
	std::cout << std::setfill(' ') << std::setw(emptinessAmount + 1) << char(179) << '\n';

	// bottom cap
	std::cout << char(192) << std::setfill(char(196)) << std::setw(DIFFERENCE_BAR_WIDTH + 1) << char(217) << "\n";
}

int main() {
	while(true) {
		double base, exponent;
		std::cout << _L << _SETW << _FILL(' ') << "Enter base:";
		std::cin >> base;
		std::cout << _L << _SETW << _FILL(' ') << "Enter exponent:";
		std::cin >> exponent;
		double realValue = pow(base, exponent);
		double approximateValue = FastMath::FastPower(base, exponent);
		std::cout << _L << _SETW << _FILL(' ') << "Real result: " << realValue << "\n";
		std::cout << _L << _SETW << _FILL(' ') << "Approximate: " << approximateValue << "\n";
		double percentageDifference = FastMath::ToPercentage(realValue / approximateValue);

		ShowDifference(percentageDifference);
	}
}