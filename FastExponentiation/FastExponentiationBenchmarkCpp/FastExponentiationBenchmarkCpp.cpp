#include <chrono>
#include <iostream>
#include <iomanip>
#include <string>
#include <array>
#include <random>
#include <map>
#include "FastMath.h"

using namespace std;

const int Iterations = 500'000;
const int Repeats = 20;

#define WIDTH 20
#define _SETWX(x) std::setw(x)
#define _SETW std::setw(WIDTH)

#define PRECISION 2
#define _SETP(x) std::fixed << std::setprecision(PRECISION) << x

#define _DEFAULT "\033[0m"

#define _NUM_IS_UNREAL(x) std::isnan(x) || !std::isfinite(x)
#define _FORMAT_ERROR_S(x) (_NUM_IS_UNREAL(x) ? "\033[31;7m" : (x > 25 ? "\033[31m" : (x > 10 ? "\033[33m" : "\033[32m")))
#define _FORMAT_ERROR(x) _FORMAT_ERROR_S(x) << x << _DEFAULT

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
	int id;
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
		intExps[i] = (long long)round(exps[i]);
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


TMeasureResult RunBenchmark(BenchmarkSetUp benchmarkSetUp, long long iterationsCount, double* bases, double* exps) {
	return benchmarkSetUp.benchmarkFunction != nullptr ?
		RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkFunction, iterationsCount, bases, exps)
		: RunBenchmark(benchmarkSetUp.functionName, benchmarkSetUp.benchmarkIntFunction, iterationsCount, bases, exps);
}

void DisplayMeasureResult(TMeasureResult* mrs, size_t count, size_t baselineIndex = 0) {
	double baselineMeanTime = mrs[baselineIndex].meanTime;
	double baselineCalculationResult = mrs[baselineIndex].calculationResult;
	std::cout << _SETW << "Function" << _SETW << "Mean time" << _SETW << "Total time" << _SETW << "Ratio"<< _SETW << "Sum" << _SETW << "Sum difference" << _SETW << "Iterations" << "\n";
	for(size_t i = 0; i < count; i++) {
		TMeasureResult& mr = mrs[i];
		double ratio = mr.meanTime / baselineMeanTime;
		std::cout << _SETW << mr.functionName << _SETWX(WIDTH - 3) << _SETP(mr.meanTime) << " ns" << _SETWX(WIDTH - 3) << mr.totalTime << " ns";
		if(ratio < 0.9) {
			std::cout << "\033[32m";
		} else if(ratio > 1.1) {
			std::cout << "\033[31m";
		} else {
			std::cout << "\033[33m";
		}
		std::cout << _SETW << _SETP(ratio) << "\033[0m";
		std::cout << _SETW << std::scientific << std::setprecision(8) << mr.calculationResult;
		double precisionError = FastMath::ToPercentage(mr.calculationResult / baselineCalculationResult);
		std::cout << _FORMAT_ERROR_S(precisionError) << _SETWX(WIDTH - 1) << std::right << _SETP(precisionError) << "%" << _DEFAULT;
		std::cout << _SETW << mr.iterationsCount;
		std::cout << "\n";
	}
}

double SignedRand() {
	return (2.0 * (rand() - RAND_MAX / 2.0)) / (double)RAND_MAX;
}

int main() {
	srand((unsigned int)time(NULL));
	double baseMul = 0;
	double expMul = 0;

	std::array<BenchmarkSetUp, 6> benchmarkSetUps{ {
		{ 0, "Built-in", pow, nullptr },
		{ 1, "FP dividing", FastMath::FastPowerDividing, nullptr },
		{ 2, "FP fractional", FastMath::FastPowerFractional, nullptr },
		{ 3, "Binary", nullptr, FastMath::BinaryPower },
		{ 4, "Old approx", FastMath::OldApproximatePower, nullptr },
		{ 5, "Another approx", FastMath::AnotherApproximatePower, nullptr }
	} };

	while(true) {
		std::cout << "Enter base multiplicator: ";
		std::cin >> baseMul;
		std::cout << "Enter exponent multiplicator: ";
		std::cin >> expMul;

		std::cout << "Generating data values\n";
		double* bases = new double[Iterations];
		double* exps = new double[Iterations];
		for(int i = 0; i < Iterations; i++) {
			bases[i] = baseMul * abs((double)i / (double)Iterations);
			exps[i] = expMul * abs((double)i / (double)Iterations);
		}
		std::cout << "Done generating values, running benchmarks\n";
		std::cout << "C++\n";

		std::map<int, TMeasureResult> dictMeasureResults;

		for(int i = 0; i < Repeats; i++) {
			auto seed = std::chrono::system_clock::now().time_since_epoch().count();
			shuffle(benchmarkSetUps.begin(), benchmarkSetUps.end(), std::default_random_engine(seed));

			for(auto benchmarkSetUp : benchmarkSetUps) {
				auto newRes = RunBenchmark(benchmarkSetUp, Iterations, bases, exps);

				auto it = dictMeasureResults.find(benchmarkSetUp.id);
				if(it != dictMeasureResults.end()) {
					TMeasureResult oldRes = it->second;
					newRes.totalTime += oldRes.totalTime;
					newRes.meanTime += oldRes.meanTime;
					newRes.iterationsCount += oldRes.iterationsCount;
					newRes.calculationResult += oldRes.calculationResult;
				}
				dictMeasureResults[benchmarkSetUp.id] = newRes;
			}
		}

		std::vector<TMeasureResult> measureResults;
		measureResults.reserve(dictMeasureResults.size());
		for(auto elem : dictMeasureResults) {
			elem.second.meanTime = elem.second.meanTime / Repeats;
			measureResults.push_back(elem.second);
		}

		std::cout << "Performance results:\n";
		DisplayMeasureResult(measureResults.data(), measureResults.size());
		std::cout << "\n";

		delete[] bases;
		delete[] exps;
	}
}