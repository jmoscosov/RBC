@echo off

:INSTALL

ECHO %DATE% %TIME% Installing ByPassRBC >> C:\NCRLogs\SupervisorPerfilado.txt

ECHO Decompressing SupervisorPerfilado Installation Files
rem start /wait ..\1.vol\RBCMessage.exe

IF exist "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll.NOPERFILADO" (
  goto FAILED

)
:OOS
ECHO %DATE% %TIME%  ATM OOS >> C:\NCRLogs\SupervisorPerfilado.txt
SSTManage /LC "SupervisorPerfilado Installing" 600000

:Backup
ECHO %DATE% %TIME% Backup components to .NOPERFILADO >> C:\NCRLogs\ByPassRBC.txt
COPY /y "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll" "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll.NOPERFILADO"
COPY /y "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.str" "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.str.NOPERFILADO"
COPY /y "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.frf" "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.frf.NOPERFILADO"
COPY /y "C:\Program Files\NCR APTRA\Advance NDC\config\resrvd.def" "C:\Program Files\NCR APTRA\Advance NDC\resrvd.def.NOPERFILADO"

:SET_NEXT_INSTALL
ECHO %DATE% %TIME%  execute runOnce_install.reg >> C:\NCRLogs\SupervisorPerfilado.txt
start /wait %SYSTEMROOT%\regedit /s C:\SupervisorPerfilado\install\runOnce_install.reg
GOTO END_REBOOT

:FAILED
  ECHO %DATE% %TIME% DLL FILE EXIST - INSTALLATION HAS ABORTED >> C:\NCRLogs\SupervisorPerfilado.txt
  GOTO END
:END_REBOOT
ECHO %DATE% %TIME% INSTALLATION SUCCESS : %response%  >> C:\NCRLogs\SupervisorPerfilado.txt
shutdown /r /f
  
:END
REM Reboot from USD
ECHO %DATE% %TIME% ERROR EXIT : %response%  >> C:\NCRLogs\SupervisorPerfilado.txt
EXIT %ERRORLEVEL%