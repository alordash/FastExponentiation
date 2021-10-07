#include <chrono>
#include <iostream>
#include <iomanip>
#include <string>
#include "FastMath.h"

#define WIDTH 20
#define _SETW std::setw(WIDTH)

typedef double (*BenchmarkFunction)(double, double);
typedef double (*BenchmarkIntFunction)(double, long long);

struct TMeasureResult {
	std::string functionName;
	double totalTime;
	double meanTime;
	long long iterationsCount;
	double calculationResult;
};

double fastPow(double a, double b) {
	union {
		double d;
		int x[2];
	} u = { a };
	u.x[1] = (int)(b * (u.x[1] - 1072632447) + 1072632447);
	u.x[0] = 0;
	return u.d;
}

TMeasureResult RunBenchmark(std::string functionName, BenchmarkFunction f, long long iterationsCount, double* bases, double* exps) {
	volatile double calculationResult = 0.0;
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
	volatile double calculationResult = 0.0;
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
		std::cout << _SETW << mr.functionName << std::setw(WIDTH - 2) << mr.meanTime << "ns" << _SETW << mr.totalTime;
		if(ratio < 0.9) {
			std::cout << "\033[32m";
		} else if(ratio > 1.1) {
			std::cout << "\033[31m";
		} else {
			std::cout << "\033[33m";
		}
		std::cout << _SETW << std::fixed << std::setprecision(2) << ratio << "\033[0m";
		std::cout << _SETW << mr.iterationsCount << _SETW << mr.calculationResult << "\n";
	}
}

double SignedRand() {
	return (rand() - RAND_MAX / 2.0) / (double)RAND_MAX;
}

int main() {
	srand((unsigned int)time(NULL));
	int n = 1'000'000;
	while(true) {
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
			bases[i] = baseMul * SignedRand();
			exps[i] = abs(expMul * SignedRand());
		}
		std::cout << "Done generating values, running benchmarks\n";

		TMeasureResult* mrs = new TMeasureResult[]{
			RunBenchmark("Built-in", pow, n, bases, exps),
			RunBenchmark("Fast power", FastMath::FastPower, n, bases, exps),
			RunBenchmark("Approximate", FastMath::FastApproximatePower, n, bases, exps),
			RunBenchmark("Binary", FastMath::BinaryPower, n, bases, exps),
			RunBenchmark("Raw fast power", FastMath::RawFastPower, n, bases, exps),
			RunBenchmark("New", fastPow, n, bases, exps)
		};

		std::cout << "Benchmark results:\n";
		DisplayMeasureResult(mrs, 6);
		delete[] mrs;
		delete[] bases;
		delete[] exps;
	}
}