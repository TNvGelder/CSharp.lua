$log = "test\full_$((Get-Date).ToString('yyyyMMdd_HHmmss')).log"
$start = Get-Date
"=== Test run started $start ===" | Out-File -FilePath $log -Encoding ascii
cmd /c "test\test.bat" 2>&1 | Tee-Object -FilePath $log -Append
$end = Get-Date
"=== Test run finished $end ===" | Out-File -FilePath $log -Encoding ascii -Append
