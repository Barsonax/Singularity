 param (
    [string]$configuration = 'debug',
	[string]$outputFolder = '.\coverage\'
 )

$filter = '+[Singularity*]* -[Singularity.Test]*'
$target = 'C:\Program Files\dotnet\dotnet.exe'

$projectname = 'Singularity.Test'
$targetArgs = ' test .\Singularity.Test\Singularity.Test.csproj -c ' + $configuration


If(!(test-path $outputFolder ))
{
      New-Item -ItemType Directory -Force -Path $outputFolder 
}

$output = $outputFolder + $projectname + '.coverage.xml'
OpenCover.Console.exe -register:user -target:$target -targetargs:$targetArgs -filter:$filter -output:$output -oldStyle