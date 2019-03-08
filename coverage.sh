#!/bin/bash

dotnet test ./tests/TupacAmaru.Yacep.Test/TupacAmaru.Yacep.Test.csproj \
    /p:CollectCoverage=true \
    /p:Exclude=\"[xunit.*]*,[TupacAmaru.Yacep.Test*]*\" \
    /p:CoverletOutputFormat=\"lcov\"