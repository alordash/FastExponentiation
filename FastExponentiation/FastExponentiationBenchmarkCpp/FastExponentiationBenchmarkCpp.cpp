#include <chrono>
#include <iostream>
#include <iomanip>
#include <string>
#include <functional>
#include <algorithm>    
#include <array>        
#include <random>       
#include "FastMath.h"

using namespace std;

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


typedef std::function<TMeasureResult()> BenchmarkSetUpFn;



TMeasureResult RunBenchmark(std::string functionName, BenchmarkFunction f, long long iterationsCount, double* bases, double* exps) {
	double calculationResult = 0.0;
	double* base = bases;
	double* exp = exps;
	double* baseEnd = base + iterationsCount;
	auto start = std::chrono::high_resolution_clock::now();
	while (base < baseEnd) {
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
	for (long long i = 0; i < iterationsCount; i++) {
		intExps[i] = (long long)exps[i];
	}
	long long* exp = intExps;
	double* baseEnd = base + iterationsCount;
	auto start = std::chrono::high_resolution_clock::now();
	while (base < baseEnd) {
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
	for (size_t i = 0; i < count; i++) {
		TMeasureResult& mr = mrs[i];
		double ratio = mr.meanTime / baselineMeanTime;
		std::cout << _SETW << mr.functionName << std::setw(WIDTH - 2) << _SETP(mr.meanTime) << " ns" << _SETW << mr.totalTime;
		if (ratio < 0.9) {
			std::cout << "\033[32m";
		} else if (ratio > 1.1) {
			std::cout << "\033[31m";
		} else {
			std::cout << "\033[33m";
		}
		std::cout << _SETW << _SETP(ratio) << "\033[0m";
		std::cout << _SETW << mr.iterationsCount;
		if (mr.calculationResult > TOO_BIG_SUM) {
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
	double baseMul = 100002.0;
	double expMul = 12.1;
	std::cout << "Enter base multiplicator: ";
	std::cin >> baseMul;
	std::cout << "Enter exponent multiplicator: ";
	std::cin >> expMul;

	std::cout << "Generating data values\n";
	double* bases = new double[n];
	double* exps = new double[n];
	for (int i = 0; i < n; i++) {
		bases[i] = abs(baseMul * ((0.14358 + i) / n) /*SignedRand()*/);
		exps[i] = abs(expMul * ((0.254 + i) / n)/*SignedRand()*/);
	}
	std::cout << "Done generating values, running benchmarks\n";

	std::array<BenchmarkSetUpFn, 6> benchmarkSetUps{
		[&]() -> TMeasureResult { return RunBenchmark("Built-in", pow, n, bases, exps); },
		[&]() -> TMeasureResult { return RunBenchmark("Fast power", FastMath::FastPower, n, bases, exps); },
		[&]() -> TMeasureResult { return RunBenchmark("Raw fast power", FastMath::RawFastPower, n, bases, exps); },
		[&]() -> TMeasureResult { return RunBenchmark("Binary", FastMath::BinaryPower, n, bases, exps); },
		[&]() -> TMeasureResult { return RunBenchmark("Approximate", FastMath::FastApproximatePower, n, bases, exps); },
		[&]() -> TMeasureResult { return RunBenchmark("Another approx", FastMath::AnotherApproximation, n, bases, exps); },
	};


	while (true) {
		std::cout << "C++\n";

		const int tryes = 50;
		for (int i = 0; i < tryes; i++) {
			unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();

			shuffle(benchmarkSetUps.begin(), benchmarkSetUps.end(), std::default_random_engine(seed));

			for (auto benchmarkSetUp : benchmarkSetUps) {
				auto newRes = benchmarkSetUp();
			}
			//for (var benchmarkSetUp in benchmarkSetUps.OrderBy(a = > rng.Next())) {
			//	var newRes = RunBenchmark(benchmarkSetUp, n, bases, exps, expsInt);
			//	if (measureResults.TryGetValue(benchmarkSetUp, out TMeasureResult oldRes)) {
			//		newRes.totalTime += oldRes.totalTime;
			//		newRes.meanTime += oldRes.meanTime;
			//		newRes.iterationsCount += oldRes.iterationsCount;
			//		newRes.calculationResult += oldRes.calculationResult;
			//	}
			//	measureResults[benchmarkSetUp] = newRes;
			//}
		}

		TMeasureResult* mrs = new TMeasureResult[]{
			RunBenchmark("Built-in", pow, n, bases, exps),
			RunBenchmark("Fast power", FastMath::FastPower, n, bases, exps),
			RunBenchmark("Raw fast power", FastMath::RawFastPower, n, bases, exps),
			RunBenchmark("Binary", FastMath::BinaryPower, n, bases, exps),
			RunBenchmark("Approximate", FastMath::FastApproximatePower, n, bases, exps),
			RunBenchmark("Another approx", FastMath::AnotherApproximation, n, bases, exps)
		};

		std::cout << "Performance results:\n";
		DisplayMeasureResult(mrs, 6);
		delete[] mrs;

		std::cout << "Press any key to repeat";
		std::cin;
	}
	delete[] bases;
	delete[] exps;
}