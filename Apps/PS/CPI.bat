
cls
net use \\192.168.201.230\c$\AQHA /user:AQHA\Mohd_Iliyas_Ahmad E['zq`9s 

del /q \\192.168.201.230\aqha\Web\identity.zip
del C:\Work\AQHA\Deployed\*.zip

"C:\Program Files\7-Zip\"7z.exe a -tzip C:\Work\AQHA\Deployed\identity.zip C:\Work\AQHA\Deployed\testidentity.aqha.lcl\

ROBOCOPY C:\Work\AQHA\Deployed \\192.168.201.230\aqha\Web *.zip

del /q \\192.168.201.230\aqha\Web\identity\*
for /d %%x in (\\192.168.201.230\aqha\Web\identity\*) do @rd /s /q "%%x"

net use \\192.168.201.230\c$\AQHA /D