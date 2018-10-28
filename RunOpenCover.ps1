 param (
    [string]$configuration = 'debug',
	[string]$outputFolder = '.\coverage\'
 )

$ErrorActionPreference='Stop'
$opencover_console = 'C:\ProgramData\chocolatey\bin\OpenCover.Console.exe'
$target = 'C:\Program Files\dotnet\dotnet.exe'
$output = $outputFolder + 'test.coverage.xml'

Remove-Item $outputFolder -Recurse -ErrorAction Ignore
If(!(test-path $outputFolder ))
{
      New-Item -ItemType Directory -Force -Path $outputFolder 
}

$project = "Singularity.Test\Singularity.Test.csproj"
$filter = '+[Singularity*]* -[Singularity*.Test]*'
$targetArgs = ' test .\Tests\' + $project + ' -c ' + $configuration

$coverOutput = &$opencover_console -register:user -target:$target -targetargs:$targetArgs -filter:$filter -output:$output -oldStyle -returntargetcode

$project = "Singularity.Duality.Test\Singularity.Duality.Test.csproj"
$targetArgs = ' test .\Tests\' + $project + ' -c ' + $configuration
	
$coverOutput = &$opencover_console -register:user -target:$target -targetargs:$targetArgs -filter:$filter -output:$output -oldStyle -mergeoutput -mergebyhash -returntargetcode