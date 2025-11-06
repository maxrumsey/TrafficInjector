dotnet-coverage collect -f cobertura -o coverage.cobertura dotnet test
reportgenerator -reports:coverage.cobertura -targetdir:.\report -assemblyfilters:+TrafficInjector.Plugin

Invoke-Item .\report\index.html