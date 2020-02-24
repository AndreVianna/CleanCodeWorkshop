@echo off
echo [93mProcessing DbContext...[0m
echo [93mReverse engineering existing database...[0m
dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=XPenCDB;Integrated Security=True;" Microsoft.EntityFrameworkCore.SqlServer -c XPenCDbContext --context-dir . -o Schema --force
if NOT %errorlevel% == 0 goto :Error
goto :End
:Error
echo [91mError executing command! Execution aborted.[0m
:End
pause

