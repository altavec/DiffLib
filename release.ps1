$Version=$([DateTime]::Now.ToString('yyyy.MM.dd.Hmm'))
dotnet pack --output .nupkg -property:VersionPrefix=$Version -property:ContinuousIntegrationBuild=true
dotnet nuget push .nupkg/DiffLib.$Version.nupkg --source https://nuget.pkg.github.com/altavec/index.json