@echo off
set targetFramework=netcoreapp2.2
set strategy=Throughput
dotnet build --force --configuration Release -f %targetFramework% ./tests/TupacAmaru.Yacep.Benchmark/TupacAmaru.Yacep.Benchmark.csproj
dotnet benchmark ./tests/TupacAmaru.Yacep.Benchmark/bin/Release/%targetFramework%/TupacAmaru.Yacep.Benchmark.dll --list Tree
dotnet benchmark ./tests/TupacAmaru.Yacep.Benchmark/bin/Release/%targetFramework%/TupacAmaru.Yacep.Benchmark.dll ^
    -a ./results/benchmark/ -j Dry --unrollFactor 2 --invocationCount 8 -r %targetFramework%^
    --warmupCount 1 -i false --strategy %strategy% --stopOnFirstError true -e html ^
	--iterationCount 50 --filter *FixedFieldBenchmark*