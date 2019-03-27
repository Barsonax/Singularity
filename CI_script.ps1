# This will make sure the CI will fail if the something in this script fails (such as the unit tests).
$ErrorActionPreference = 'Stop' 
Set-StrictMode -Version Latest

$modulesFolder = $PSScriptRoot + '\Scripts\'

foreach ($module in Get-Childitem $modulesFolder -Name -Filter "*.psm1")
{
    Import-Module $modulesFolder\$module
}

$coverageFolder = $PSScriptRoot + '\coverage\'
$buildOutputFolder = $PSScriptRoot + '\BuildOutput\' 
$artifactFolder = $PSScriptRoot + '\Artifacts\'
$coverageFilename = 'test.coverage.xml'
$coverageFileFullname = $coverageFolder + $coverageFilename
$configuration = 'z_CI_config'


InstallChocolateyPackages
Build $configuration $artifactFolder
dotnet test
#RunOpenCover $coverageFolder $coverageFilename $buildOutputFolder
#PublishCoverage -codegovtoken:$env:codegov_token -coverageFile:$coverageFileFullname