$findString = 'https://ng.aqha.com'
$replaceString = 'https://ngdev.aqha.com'

Get-ChildItem D:\Wk\OD\AQHA\Extnl\WebSite\Views *.*htm* -recurse |
    Foreach-Object {
        $c = ($_ | Get-Content) 
        $c = $c -replace $findString, $replaceString
        [IO.File]::WriteAllText($_.FullName, ($c -join "`r`n"))
    }