#include <iostream>
#include <math.h>
#include <iomanip>
#include "FastMath.h"

#define DIFFERENCE_BAR_WIDTH 100

void ShowDifference(double percentageDifference) {
	int fillAmount = (int)round((double)DIFFERENCE_BAR_WIDTH * percentageDifference / 100.0);
	bool exceededLimit = fillAmount > DIFFERENCE_BAR_WIDTH;
	std::cout << "Difference: " << (exceededLimit ? "\033[31m" : "") << percentageDifference << (exceededLimit ? "\033[0m" : "") << "%\n";

	// top cap
	std::cout << char(218) << std::setfill(char(196)) << std::setw(DIFFERENCE_BAR_WIDTH + 1) << char(191) << "\n";
	// left border
	std::cout << char(179);

	if(exceededLimit) {
		fillAmount = DIFFERENCE_BAR_WIDTH;
	}
	int emptinessAmount = DIFFERENCE_BAR_WIDTH - fillAmount;

	if(exceededLimit) {
		// red color if difference > 100%
		std::cout << "\033[31m";
	}
	// difference filler
	std::cout << std::setfill('#') << std::setw(fillAmount + 4) << "\033[0m";

	// emptiness filler
	std::cout << std::setfill(' ') << std::setw(emptinessAmount + 1) << char(179) << '\n';

	// bottom cap
	std::cout << char(192) << std::setfill(char(196)) << std::setw(DIFFERENCE_BAR_WIDTH + 1) << char(217) << "\n";
}

int main() {
	while(true) {
		double base, exponent;
		std::cout << "Enter base:\n";
		std::cin >> base;
		std::cout << "Enter exponent:\n";
		std::cin >> exponent;
		double realValue = pow(base, exponent);
		double approximateValue = FastMath::FastPower(base, exponent);
		std::cout << "Real result: " << realValue << "\n";
		std::cout << "Approximate result: " << approximateValue << "\n";
		double percentageDifference = FastMath::ToPercentage(realValue / approximateValue);

		ShowDifference(percentageDifference);
	}
}