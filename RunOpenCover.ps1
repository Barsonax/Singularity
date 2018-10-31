param (
    [string]$configuration = 'z_CI_config',
    [string]$outputFolder = '.\coverage\'
)

$ErrorActionPreference = 'Stop'
$opencover_console = 'C:\ProgramData\chocolatey\bin\OpenCover.Console.exe'
$target = 'C:\Program Files\dotnet\dotnet.exe'
$output = $outputFolder + 'test.coverage.xml'

Remove-Item $outputFolder -Recurse -ErrorAction Ignore
If (!(test-path $outputFolder )) {
    New-Item -ItemType Directory -Force -Path $outputFolder | Out-Null
}

$buildOutput = $PSScriptRoot + '\BuildOutput\'

$project = (Get-ChildItem -Path $buildOutput -Filter '*Test.dll*').FullName
$filter = '+[Singularity*]* -[Singularity*.Test]*'

$targetArgs = ' vstest ' + $project
&$opencover_console -register:user -target:$target -targetargs:$targetArgs -filter:$filter -output:$output -oldStyle -returntargetcode -hideskipped:Filter