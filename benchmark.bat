dotnet build --force --configuration Release ./tests/TupacAmaru.Yacep.Benchmark/TupacAmaru.Yacep.Benchmark.csproj
dotnet benchmark ./tests/TupacAmaru.Yacep.Benchmark/bin/Release/netcoreapp2.1/TupacAmaru.Yacep.Benchmark.dll --list Tree
dotnet benchmark ./tests/TupacAmaru.Yacep.Benchmark/bin/Release/netcoreapp2.1/TupacAmaru.Yacep.Benchmark.dll ^
    -a ./results/benchmark/ ^
    --warmupCount 0 -i true --join true --strategy ColdStart  --stopOnFirstError true -e html ^
	--launchCount 1 --unrollFactor 4 --invocationCount 8 --iterationCount 10 --filter *FixedFieldBenchmark*