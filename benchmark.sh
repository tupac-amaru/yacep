#!/bin/bash
strategy=Throughput 
dotnet restore
dotnet run --project ./tests/TupacAmaru.Yacep.Benchmark \
           -c Release \
           --artifacts ./results/benchmark/ \
           -j Short \
           --unrollFactor 4 \
           --invocationCount 8 \
           --warmupCount 1 \
           -i true \
           --strategy $strategy \
           --stopOnFirstError true \
           -e html \
           --iterationCount 200 \
           --filter *DictionaryBenchmark*