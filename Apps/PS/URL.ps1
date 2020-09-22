$textfile = "c:\temp\url.txt"
 
ForEach ($url in Get-Content $textfile) {
        Start-Process "chrome.exe" $url
    
}