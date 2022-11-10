:Backup
ECHO %DATE% %TIME% Restoring components to .NOPERFILADO >> C:\NCRLogs\SupervisorPerfilado.txt
xcopy /Q /Y "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll.NOPERFILADO" "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll" >> C:\NCRLogs\SupervisorPerfilado.txt
xcopy /Q /Y "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.str.NOPERFILADO" "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.str" >> C:\NCRLogs\SupervisorPerfilado.txt
xcopy /Q /Y "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.frf.NOPERFILADO" "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.frf" >> C:\NCRLogs\SupervisorPerfilado.txt
xcopy /Q /Y "C:\Program Files\NCR APTRA\Advance NDC\config\resrvd.def.NOPERFILADO" "C:\Program Files\NCR APTRA\Advance NDC\config\resrvd.def" >> C:\NCRLogs\SupervisorPerfilado.txt
ping -n 10 127.0.0.1 > nul
DEL /Q "C:\Program Files\NCR APTRA\Advance NDC\config\SupvPwd.xml"
DEL /Q "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.dll.NOPERFILADO"
DEL /Q "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.str.NOPERFILADO"
DEL /Q "C:\Program Files\NCR APTRA\Advance NDC\Supervisor.frf.NOPERFILADO"
DEL /Q "C:\Program Files\NCR APTRA\Advance NDC\config\resrvd.def.NOPERFILADO"
ping -n 10 127.0.0.1 > nul
:END
REM Reboot from USD
ECHO %DATE% %TIME% END Uninstall Process : %response%  >> C:\NCRLogs\SupervisorPerfilado.txt
EXIT %ERRORLEVEL%