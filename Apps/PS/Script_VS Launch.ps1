# define path to store password and input password 
$path = "C:\Secure"
# get the encrypted password from the path
$encpwd = Get-Content $path\filename.bin
# convert file to secure string 
$passwd = ConvertTo-SecureString $encpwd
# define needed credential 
$cred = new-object System.Management.Automation.PSCredential 'XCLP-TU104-2775\Administrator',$passwd
# go to VS launch devenv.exe as user with privileges to launch the program with no user input required
Set-Location "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE"
Start-Process PowerShell -Cred $cred -ArgumentList .\devenv.exe