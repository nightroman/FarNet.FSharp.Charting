
Set-StrictMode -Version 3
$Testing = @{Test = 500; Timeout = 30000}

foreach($item in Get-Item ..\samples\*.fsx) {
	if ($item.Name -notmatch '^(?:Far|StockAndCandlestick|VanillaLine)') {
		task $item.Name {
			Start-Far @Testing "fs:exec file=..\samples\$($task.Name)"
		}
	}
}
