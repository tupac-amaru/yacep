#!/bin/bash

targetFramework=netcoreapp2.2
strategy=Throughput 
dotnet build --force --configuration Release -f $targetFramework ./tests/TupacAmaru.Yacep.Benchmark/TupacAmaru.Yacep.Benchmark.csproj
dotnet benchmark ./tests/TupacAmaru.Yacep.Benchmark/bin/Release/$targetFramework/TupacAmaru.Yacep.Benchmark.dll --list Tree
dotnet benchmark ./tests/TupacAmaru.Yacep.Benchmark/bin/Release/$targetFramework/TupacAmaru.Yacep.Benchmark.dll \
    -a ./results/benchmark/ -j Short --unrollFactor 4 --invocationCount 8 -r $targetFramework\
    --warmupCount 1 -i true --strategy $strategy --stopOnFirstError true -e html \
	--iterationCount 200 --filter *FixedFieldBenchmark*