net use \\192.168.201.230\c$\AQHA /user:AQHA\Mohd_Iliyas_Ahmad E['zq`9s 

del /q \\192.168.201.230\aqha\Web\identity\*
for /d %%x in (\\192.168.201.230\aqha\Web\identity\*) do @rd /s /q "%%x"
"C:\Program Files\7-zip\"7z.exe x \\192.168.201.230\aqha\Web\identity.zip -o"\\192.168.201.230\aqha\Web\identity" -y

del /q \\192.168.201.230\aqha\Web\restapi\*
for /d %%x in (\\192.168.201.230\aqha\Web\restapi\*) do @rd /s /q "%%x"
"C:\Program Files\7-zip\"7z.exe x \\192.168.201.230\aqha\Web\restapi.zip -o"\\192.168.201.230\aqha\Web\restapi" -y

del /q \\192.168.201.230\aqha\Web\webapp\*
for /d %%x in (\\192.168.201.230\aqha\Web\webapp\*) do @rd /s /q "%%x"
"C:\Program Files\7-zip\"7z.exe x \\192.168.201.230\aqha\Web\webapp.zip -o"\\192.168.201.230\aqha\Web\webapp" -y

net use \\192.168.201.230\c$\AQHA /D