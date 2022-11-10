@echo off

:INSTALL

ECHO %DATE% %TIME% Decompressing Supervisor components >> C:\NCRLogs\SupervisorPerfilado.txt

ECHO Decompressing build Installation Files
start /wait C:\SupervisorPerfilado\Install\build.exe
  
:END
REM Reboot from USD
ECHO %DATE% %TIME% installation completed >> C:\NCRLogs\SupervisorPerfilado.txt
EXIT %ERRORLEVEL%