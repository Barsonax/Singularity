$coverageFolder = '.\coverage\'
$coverageFilename = 'test.coverage.xml'
$coverageFileFullname = $coverageFolder + $coverageFilename
$buildOutputFolder = $PSScriptRoot + '\BuildOutput\' 

.\InstallChocolateyPackages.ps1 
.\Build.ps1
.\RunOpenCover.ps1 -coverageFolder:$coverageFolder -coverageFilename:$coverageFilename -buildOutputFolder:$buildOutputFolder
.\PublishCoverage.ps1 -codegovtoken:$env:codegov_token -coverageFile:$coverageFileFullname