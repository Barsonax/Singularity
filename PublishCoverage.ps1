 param (
    [Parameter(Mandatory=$true)]
	[string]$codegovtoken,
	[Parameter(Mandatory=$true)]
	[string]$coverageFile
 )

codecov -f $coverageFile -t $codegovtoken