param (
    [string]$configuration = 'z_CI_config',
	[string]$artifactFolder = $PSScriptRoot + '/Artifacts'
)

dotnet build -m -c $configuration 

$gitversion = GitVersion.exe|ConvertFrom-Json
$version = $gitversion.NuGetVersion

dotnet pack -m -c $configuration -o $artifactFolder --include-symbols --no-build /p:Version=$version