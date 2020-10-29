# Enter encrypted password once and store to file; define variables
new-item c:\Secure -itemtype directory  
$path = "C:\Secure"
$passwd = Read-Host "Administrator" -AsSecureString
$encpwd = ConvertFrom-SecureString $passwd
$encpwd > $path\filename.bin