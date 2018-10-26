 param (
    [Parameter(Mandatory=$true)][string]$codegovtoken,
	[string]$coverageFolder = '.\coverage\'
 )

$coverageXml = $coverageFolder + "test.coverage.xml"
codecov -f $coverageXml -t $codegovtoken