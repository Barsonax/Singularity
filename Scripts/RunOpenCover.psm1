function RunOpenCover ([Parameter(Mandatory=$true)][string]$coverageFolder,
	[Parameter(Mandatory=$true)][string]$coverageFilename,
	[Parameter(Mandatory=$true)][string]$buildOutputFolder){
	# Using the full path since appveyor already has a different version installed.
	$opencover_console = 'C:\ProgramData\chocolatey\bin\OpenCover.Console.exe' 
	
	# Clears the coverage output folder if it already exist
	Remove-Item $coverageFolder -Recurse -ErrorAction Ignore
	If (!(test-path $coverageFolder )) {
	    New-Item -ItemType Directory -Force -Path $coverageFolder | Out-Null
	}
	
	$testdlls = (Get-ChildItem -Path $buildOutputFolder -Filter '*Test.dll*').FullName # Grab all test dll's in the buildoutput folder
	$filter = '+[Singularity*]* -[Singularity*.Test]*' # This will determine what will be included in the results
	$dotnetexe = 'C:\Program Files\dotnet\dotnet.exe'
	$targetArgs = ' vstest ' + $testdlls
	
	# the coverage xml file be put in here.
	$output = $coverageFolder + $coverageFilename
	&$opencover_console -register:user -target:$dotnetexe -targetargs:$targetArgs -filter:$filter -output:$output -oldStyle -returntargetcode -hideskipped:Filter
	if ($LASTEXITCODE -ne 0) { throw "opencover exited with code: $LASTEXITCODE" }
}