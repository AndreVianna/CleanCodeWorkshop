@echo off
echo [93mProcessing Initial repository...[0m
echo [93mDropping existing database...[0m
dotnet ef database drop --force --context XPenC.Database.DataContext
if NOT %errorlevel% == 0 goto :Error
echo [93mRemove current Initial migration...[0m
dotnet ef migrations remove --context XPenC.Database.DataContext
if NOT %errorlevel% == 0 goto :Error
echo [93mCreating new Initial migration...[0m
dotnet ef migrations add Initial --context XPenC.Database.DataContext
if NOT %errorlevel% == 0 goto :Error
echo [93mUpdating database...[0m
dotnet ef database update --context XPenC.Database.DataContext
if NOT %errorlevel% == 0 goto :Error
echo [92mCommand executed with success.[0m
goto :End
:Error
echo [91mError executing command! Execution aborted.[0m
:End
pause