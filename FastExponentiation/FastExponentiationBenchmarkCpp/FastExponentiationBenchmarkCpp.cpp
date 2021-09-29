#include <chrono>
#include <iostream>
#include <iomanip>
#include <string>
#include "FastMath.h"

#define WIDTH 20
#define _SETW std::setw(WIDTH)

typedef double (*BenchmarkFunction)(double, double);

struct TMeasureResult {
	std::string functionName;
	double totalTime;
	double meanTime;
	long long iterationsCount;
	double calculationResult;
};

TMeasureResult RunBenchmark(std::string functionName, BenchmarkFunction f, long long iterationsCount, double* bases, double* exps) {
	volatile double calculationResult = 0.0;
	double* base = bases;
	double* exp = exps;
	auto start = std::chrono::high_resolution_clock::now();
	for(volatile long long i = 0; i < iterationsCount; i++) {
		calculationResult += f(*base++, *exps++);
	}
	auto finish = std::chrono::high_resolution_clock::now();
	auto nanoSeconds = std::chrono::duration_cast<std::chrono::nanoseconds>(finish - start).count();

	TMeasureResult measureResult = { functionName, nanoSeconds, nanoSeconds / (double)iterationsCount, iterationsCount, calculationResult };
	return measureResult;
}

void DisplayMeasureResult(TMeasureResult* mrs, size_t count) {
	std::cout << _SETW << "Function" << _SETW << "Mean time" << _SETW << "Total time" << _SETW << "Iterations" << _SETW << "Sum\n";
	for(size_t i = 0; i < count; i++) {
		TMeasureResult& mr = mrs[i];
		std::cout << _SETW << mr.functionName << std::setw(WIDTH - 2) << mr.meanTime << "ns" << _SETW << mr.totalTime << _SETW << mr.iterationsCount << _SETW << mr.calculationResult << "\n";
	}
}

double SignedRand() {
	return (rand() - RAND_MAX / 2.0) / (double)RAND_MAX;
}

int main() {
	srand((unsigned int)time(NULL));
	int n = 10'000'000;
	while(true) {
		double baseMul = 100002.0;
		double expMul = 12.1;
		std::cout << "Enter base multiplicator: ";
		std::cin >> baseMul;
		std::cout << "Enter exponent multiplicator: ";
		std::cin >> expMul;
		double* bases = new double[n];
		double* exps = new double[n];
		for(unsigned long long i = 0; i < n; i++) {
			bases[i] = baseMul * SignedRand();
			exps[i] = expMul * SignedRand();
		}

		auto builtInMethodTest = RunBenchmark("Built-in", pow, n, bases, exps);
		auto approximateMethodTest = RunBenchmark("Approximate", FastMath::FastPower, n, bases, exps);
		TMeasureResult* mrs = new TMeasureResult[2]{ builtInMethodTest, approximateMethodTest };
		std::cout << "Performance result:\n";
		DisplayMeasureResult(mrs, 2);
		std::cout << "Speed ";
		if(abs(builtInMethodTest.meanTime - approximateMethodTest.meanTime) <= 5.0) {
			std::cout << "\033[33m";
		} else if(builtInMethodTest.meanTime > approximateMethodTest.meanTime) {
			std::cout << "\033[32m";
		} else  {
			std::cout << "\033[31m";
		}
		std::cout << "x" << builtInMethodTest.meanTime / approximateMethodTest.meanTime << "\033[0m\n";
		delete[] mrs;
		delete[] bases;
		delete[] exps;
	}
}