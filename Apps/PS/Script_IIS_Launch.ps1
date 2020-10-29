# define path to store password and input password 
$path = "C:\Secure"
# get the encrypted password from the path
$encpwd = Get-Content $path\filename.bin
# convert file to secure string 
$passwd = ConvertTo-SecureString $encpwd
# define needed credential 
$cred = new-object System.Management.Automation.PSCredential 'XILP-IND-3128\Administrator',$passwd
# go to IIS launch InetMgr.exe as user with privileges to launch the program with no user input required
Set-Location C:\Windows\system32\inetsrv
Start-Process PowerShell -Cred $cred -ArgumentList .\InetMgr.exe