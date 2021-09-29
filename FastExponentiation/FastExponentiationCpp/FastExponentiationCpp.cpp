#include <iostream>
#include <math.h>
#include <iomanip>
#include "FastMath.h"

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
		std::cout << "Difference: " << FastMath::ToPercentage(realValue / approximateValue) << "%\n";
		std::cout << std::setfill(char(196)) << std::setw(50) << "\n";
	}
}