@echo off
echo [93mCleaning obj folders...[0m
for /f "tokens=*" %%g in ('dir /b /ad /s obj') do (
	@echo "Removing %%g"
	rmdir /s /q "%%g"
)
echo [93mCleaning bin folders...[0m
for /f "tokens=*" %%g in ('dir /b /ad /s bin') do (
	@echo "Removing %%g"
	rmdir /s /q "%%g"
)

echo [93mCleaning Test Results...[0m
rmdir /s /q .\TestResults
for /f "tokens=*" %%g in ('dir /b /ad /s TestResults') do (
	@echo "Removing %%g"
	rmdir /s /q "%%g"
)

pause
