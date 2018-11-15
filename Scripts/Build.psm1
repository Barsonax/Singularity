function Build ([Parameter(Mandatory=$true)][string]$configuration, [Parameter(Mandatory=$true)][string]$artifactFolder){
	$gitversion = &'C:\ProgramData\chocolatey\bin\GitVersion.exe'|ConvertFrom-Json
	
	$version = $gitversion.MajorMinorPatch
	$nugetversion = $gitversion.NuGetVersion
	
	dotnet build -m -c $configuration /p:Version=$version
	if ($LASTEXITCODE -ne 0) { throw "dotnet build exited with code: $LASTEXITCODE" }

	dotnet pack -m -c $configuration -v n -o $artifactFolder --include-symbols --no-build /p:Version=$nugetversion
	if ($LASTEXITCODE -ne 0) { throw "dotnet pack exited with code: $LASTEXITCODE" }
}