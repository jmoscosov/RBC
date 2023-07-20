@echo off
::Fix Promote txRequest restart
:: JM185384
ECHO %DATE% %TIME% Executing clearfolderutility >> C:\NCRLogs\ClearFolderUtility.LOG
SSTManage /LC "clearfolderutility" 700000
IF %ERRORLEVEL% NEQ 0 (
    IF %ERRORLEVEL% EQU 1 ECHO %DATE% %TIME% Error : ATM ALREADY OOS  >> C:\NCRLogs\ClearFolderUtility.LOG
	IF %ERRORLEVEL% EQU 2 ECHO %DATE% %TIME% Error : ATM COULD NOT BE OOS  >> C:\NCRLogs\ClearFolderUtility.LOG
	GOTO ROLLBACK
)
ECHO %DATE% %TIME% info : ATM OOS OK  >> C:\NCRLogs\ClearFolderUtility.LOG
call clearfolderutility.exe "C:\Program Files (x86)\NCR APTRA\Advance NDC\Screens"
ECHO %DATE% %TIME% info : Running clearfolderutility   >> C:\NCRLogs\ClearFolderUtility.LOG
IF %ERRORLEVEL% NEQ 0 (
	IF %ERRORLEVEL% EQU 1 ECHO %DATE% %TIME% Error : Execute program without args  >> C:\NCRLogs\ClearFolderUtility.LOG
	IF %ERRORLEVEL% EQU 2 ECHO %DATE% %TIME% Error : Folder not found  >> C:\NCRLogs\ClearFolderUtility.LOG
	IF %ERRORLEVEL% EQU 100 ECHO %DATE% %TIME% Error : AccessViolationException  >> C:\NCRLogs\ClearFolderUtility.LOG
	IF %ERRORLEVEL% EQU 101 ECHO %DATE% %TIME% Error : UnauthorizedAccessException  >> C:\NCRLogs\ClearFolderUtility.LOG
	GOTO ROLLBACK
)
ECHO %DATE% %TIME% info : Successful file removal process   >> C:\NCRLogs\ClearFolderUtility.LOG
GOTO ROLLBACK

:ROLLBACK
ECHO %DATE% %TIME% info : Rollback Entry   >> C:\NCRLogs\ClearFolderUtility.LOG
SSTManage /RC "clearfolderutility" 
IF %ERRORLEVEL% EQU 0 ECHO %DATE% %TIME% Info : ATM IN SERVICE  >> C:\NCRLogs\ClearFolderUtility.LOG
GOTO END

:END
ECHO %DATE% %TIME% info : ExitCode : %ERRORLEVEL% >> C:\NCRLogs\ClearFolderUtility.LOG
EXIT %ERRORLEVEL%
