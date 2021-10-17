#include <chrono>
#include <iostream>
#include <iomanip>
#include <string>
#include <functional>
#include <algorithm>    
#include <array>        
#include <random>       
#include <ctime>
#include <map>
#include "FastMath.h"

using namespace std;

#define WIDTH 20
#define _SETW std::setw(WIDTH)

#define PRECISION 2
#define _SETP(x) std::fixed << std::setprecision(2) << x

typedef double (*BenchmarkFunction)(double, double);
typedef double (*BenchmarkIntFunction)(double, long long);

struct TMeasureResult {
	std::string functionName;
	double totalTime;
	double meanTime;
	long long iterationsCount;
	double calculationResult;
};

struct BenchmarkSetUp {
	std::string functionName;
	BenchmarkFunction benchmarkFunction = nullptr;
	BenchmarkIntFunction benchmarkIntFunction = nullptr;
};

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


TMeasureResult RunBenchmark(BenchmarkSetUp benchmarkSetUp, long long iterationsCount, double* bases, double* exps) {
	return benchmarkSetUp.benchmarkFunction != nullptr ?
		RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkFunction, iterationsCount, bases, exps)
		: RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkIntFunction, iterationsCount, bases, exps);
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
		std::cout << _SETW << std::scientific << std::setprecision(8) << mr.calculationResult << "\n";
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


	std::array<BenchmarkSetUp, 6> benchmarkSetUps{ {
		{ "Built-in", pow, nullptr },
		{ "Fast power", FastMath::FastPower, nullptr },
		////{ "Fast power1", FastMath::FastPower1, nullptr },
		{ "Raw fast power", FastMath::RawFastPower, nullptr },
		{ "Binary", nullptr, FastMath::BinaryPower },
		{ "Approximate",  FastMath::FastApproximatePower, nullptr },
		{ "Another approx",  FastMath::AnotherApproximation, nullptr }
	} };


	while (true) {
		std::cout << "C++\n";

		std::map<std::string, TMeasureResult> dictMeasureResults;

		const int tries = 50;
		for (int i = 0; i < tries; i++) {
			auto seed = std::chrono::system_clock::now().time_since_epoch().count();
			shuffle(benchmarkSetUps.begin(), benchmarkSetUps.end(), std::default_random_engine(seed));

			for (auto benchmarkSetUp : benchmarkSetUps) {
				auto newRes = RunBenchmark(benchmarkSetUp, n, bases, exps);

				auto it = dictMeasureResults.find(benchmarkSetUp.functionName);
				if (it != dictMeasureResults.end()) {
					TMeasureResult oldRes = it->second;
					newRes.totalTime += oldRes.totalTime;
					newRes.meanTime += oldRes.meanTime;
					newRes.iterationsCount += oldRes.iterationsCount;
					newRes.calculationResult += oldRes.calculationResult;
				}
				dictMeasureResults[benchmarkSetUp.functionName] = newRes;
			}
		}

		std::vector<TMeasureResult> measureResults;
		measureResults.reserve(dictMeasureResults.size());
		for (auto elem : dictMeasureResults) {
			elem.second.meanTime = elem.second.meanTime / tries;
			measureResults.push_back(elem.second);
		}

		std::cout << "Performance results:\n";
		DisplayMeasureResult(measureResults.data(), measureResults.size());

		std::cout << "Press Enter to Continue\n";
		cin.ignore(numeric_limits<streamsize>::max(), '\n');
	}
	delete[] bases;
	delete[] exps;
}