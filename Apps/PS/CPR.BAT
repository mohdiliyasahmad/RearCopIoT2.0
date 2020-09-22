cls
del C:\Work\AQHA\Deployed\*.zip

"C:\Program Files\7-Zip\"7z.exe a -tzip C:\Work\AQHA\Deployed\restapi.zip C:\Work\AQHA\Deployed\testrestapi.aqha.lcl\

net use \\192.168.203.15\C$\EquusDeployment\SecurityMatrix /user:AQHA\Mohd_Iliyas_Ahmad E['zq`9s 

del /q \\192.168.203.15\C$\EquusDeployment\SecurityMatrix\*.zip

ROBOCOPY C:\Work\AQHA\Deployed \\192.168.203.15\C$\EquusDeployment\SecurityMatrix *.zip

net use \\192.168.203.15\C$\EquusDeployment\SecurityMatrix /D