param (
    [string]$configuration = 'z_CI_config',
	[string]$artifactFolder = $PSScriptRoot + '/Artifacts'
)

$gitversion = GitVersion.exe|ConvertFrom-Json

$version = $gitversion.MajorMinorPatch
$nugetversion = $gitversion.NuGetVersion

dotnet build -m -c $configuration /p:Version=$version
dotnet pack -m -c $configuration -o $artifactFolder --include-symbols --no-build /p:Version=$nugetversion