dotnet-coverage collect -f xml -o coverage.xml dotnet test
reportgenerator -reports:coverage.xml -targetdir:.\report -assemblyfilters:-TrafficInjector.Tests.dll

Invoke-Item .\report\index.html