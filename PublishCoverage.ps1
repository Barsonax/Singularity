 param (
    [Parameter(Mandatory=$true)][string]$codegovtoken,
	[string]$coverageFolder = '.\coverage\'
 )

$coverageXml = $coverageFolder + "Singularity.Test.coverage.xml"
codecov -f $coverageXml -t $codegovtoken