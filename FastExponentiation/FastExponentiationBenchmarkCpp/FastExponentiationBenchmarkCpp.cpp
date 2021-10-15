#include <chrono>
#include <iostream>
#include <iomanip>
#include <string>
#include "FastMath.h"

#define WIDTH 20
#define _SETW std::setw(WIDTH)

#define PRECISION 2
#define _SETP(x) std::fixed << std::setprecision(2) << x

#define TOO_BIG_SUM 100'000'000'000.0
#define TOO_BIG_MESSAGE "Too big"

typedef double (*BenchmarkFunction)(double, double);
typedef double (*BenchmarkIntFunction)(double, long long);

struct TMeasureResult {
	std::string functionName;
	double totalTime;
	double meanTime;
	long long iterationsCount;
	double calculationResult;
};

TMeasureResult RunBenchmark(std::string functionName, BenchmarkFunction f, long long iterationsCount, double* bases, double* exps) {
	double calculationResult = 0.0;
	double* base = bases;
	double* exp = exps;
	double* baseEnd = base + iterationsCount;
	auto start = std::chrono::high_resolution_clock::now();
	while(base < baseEnd) {
		calculationResult += f(*base++, *exp++);
	}
	auto finish = std::chrono::high_resolution_clock::now();
	auto nanoSeconds = std::chrono::duration_cast<std::chrono::nanoseconds>(finish - start).count();

	TMeasureResult measureResult = { functionName, nanoSeconds, nanoSeconds / (double)iterationsCount, iterationsCount, calculationResult };
	return measureResult;
}

TMeasureResult RunBenchmark(std::string functionName, BenchmarkIntFunction f, long long iterationsCount, double* bases, double* exps) {
	double calculationResult = 0.0;
	double* base = bases;
	long long* intExps = new long long[iterationsCount];
	for(long long i = 0; i < iterationsCount; i++) {
		intExps[i] = (long long)exps[i];
	}
	long long* exp = intExps;
	double* baseEnd = base + iterationsCount;
	auto start = std::chrono::high_resolution_clock::now();
	while(base < baseEnd) {
		calculationResult += f(*base++, *exp++);
	}
	auto finish = std::chrono::high_resolution_clock::now();
	auto nanoSeconds = std::chrono::duration_cast<std::chrono::nanoseconds>(finish - start).count();

	TMeasureResult measureResult = { functionName, nanoSeconds, nanoSeconds / (double)iterationsCount, iterationsCount, calculationResult };
	delete[] intExps;
	return measureResult;
}

void DisplayMeasureResult(TMeasureResult* mrs, size_t count, size_t baselineIndex = 0) {
	double baselineMeanTime = mrs[baselineIndex].meanTime;
	std::cout << _SETW << "Function" << _SETW << "Mean time" << _SETW << "Total time" << _SETW << "Ratio" << _SETW << "Iterations" << _SETW << "Sum\n";
	for(size_t i = 0; i < count; i++) {
		TMeasureResult& mr = mrs[i];
		double ratio = mr.meanTime / baselineMeanTime;
		std::cout << _SETW << mr.functionName << std::setw(WIDTH - 2) << _SETP(mr.meanTime) << " ns" << _SETW << mr.totalTime;
		if(ratio < 0.9) {
			std::cout << "\033[32m";
		} else if(ratio > 1.1) {
			std::cout << "\033[31m";
		} else {
			std::cout << "\033[33m";
		}
		std::cout << _SETW << _SETP(ratio) << "\033[0m";
		std::cout << _SETW << mr.iterationsCount;
		if(mr.calculationResult > TOO_BIG_SUM) {
			std::cout << _SETW << TOO_BIG_MESSAGE << "\n";
		} else {
			std::cout << _SETW << _SETP(mr.calculationResult) << "\n";
		}
	}
}

double SignedRand() {
	return (2.0 * (rand() - RAND_MAX / 2.0)) / (double)RAND_MAX;
}

int main() {
	srand((unsigned int)time(NULL));
	int n = 1'000'000;
	while(true) {
		std::cout << "C++\n";
		double baseMul = 100002.0;
		double expMul = 12.1;
		std::cout << "Enter base multiplicator: ";
		std::cin >> baseMul;
		std::cout << "Enter exponent multiplicator: ";
		std::cin >> expMul;

		std::cout << "Generating data values\n";
		double* bases = new double[n];
		double* exps = new double[n];
		for(int i = 0; i < n; i++) {
			bases[i] = abs(baseMul * SignedRand());
			exps[i] = abs(expMul * SignedRand());
		}
		std::cout << "Done generating values, running benchmarks\n";

		TMeasureResult* mrs = new TMeasureResult[]{
			RunBenchmark("Built-in", pow, n, bases, exps),
			RunBenchmark("FP dividing", FastMath::FastPowerDividing, n, bases, exps),
			RunBenchmark("FP fractional", FastMath::FastPowerFractional, n, bases, exps),
//			RunBenchmark("Raw FP Dividing", FastMath::RawFastPowerDividing, n, bases, exps),
			RunBenchmark("Binary", FastMath::BinaryPower, n, bases, exps),
			RunBenchmark("Old approx", FastMath::OldApproximatePower, n, bases, exps),
			RunBenchmark("Another approx", FastMath::AnotherApproximatePower, n, bases, exps)
		};

		std::cout << "Performance results:\n";
		DisplayMeasureResult(mrs, 6);
		delete[] mrs;
		delete[] bases;
		delete[] exps;
	}
}