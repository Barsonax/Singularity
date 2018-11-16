function PublishCoverage ([Parameter(Mandatory=$true)][string]$codegovtoken,
	[Parameter(Mandatory=$true)][string]$coverageFile){
	codecov -f $coverageFile -t $codegovtoken --required
	if ($LASTEXITCODE -ne 0) { throw "codecov exited with code: $LASTEXITCODE" }
}