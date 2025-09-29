
Set-StrictMode -Version 3

foreach($item in Get-Item ..\samples\*.fsx) {
	if ($item.Name -notmatch '^(?:Far|StockAndCandlestick|VanillaLine)') {
		task $item.Name {
			Start-Far "fs:exec file=..\samples\$($task.Name)" -Exit 500, 30000
		}
	}
}
