@echo off

:INSTALL

ECHO %DATE% %TIME% Installing SupervisorPerfilado >> C:\NCRLogs\SupervisorPerfilado.txt

ECHO Decompressing SupervisorPerfilado Installation Files

IF NOT EXIST "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll.NOPERFILADO" (
  goto FAILED

)
:OOS
ECHO %DATE% %TIME%  ATM OOS >> C:\NCRLogs\SupervisorPerfilado.txt
SSTManage /LC "SupervisorPerfilado UnInstalling" 600000

:SET_NEXT_INSTALL
ECHO %DATE% %TIME%  execute runOnce_uninstall.reg >> C:\NCRLogs\SupervisorPerfilado.txt
start /wait %SYSTEMROOT%\regedit /s C:\SupervisorPerfilado\Uninstall\runOnce_uninstall.reg
GOTO END_REBOOT

:FAILED
  ECHO %DATE% %TIME% DLL FILE backup not exist - UNINSTALLATION HAS ABORTED >> C:\NCRLogs\SupervisorPerfilado.txt
  GOTO END
:END_REBOOT
ECHO %DATE% %TIME% UNINSTALLATION RESTARTING:  >> C:\NCRLogs\SupervisorPerfilado.txt
shutdown /r /f
  
:END
REM Reboot from USD
ECHO %DATE% %TIME% EXIT : %response%  >> C:\NCRLogs\SupervisorPerfilado.txt
EXIT %ERRORLEVEL%