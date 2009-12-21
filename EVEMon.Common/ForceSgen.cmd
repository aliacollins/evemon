@ECHO off
REM v6 SDK Installed
SET SgenPath=C:\Program Files\Microsoft SDKs\Windows\v6.0a\bin\
ECHO Testing v6.0 SDK...
IF EXIST "%SgenPath%" GOTO RUN

REM v6.1 SDK Installed
SET SgenPath=C:\Program Files\Microsoft SDKs\Windows\v6.1\Bin\
ECHO Testing v6.1 SDK...
IF EXIST "%SgenPath%" GOTO RUN

REM v6.1 SDK Installed on D: (For BattleClinic)
SET SgenPath=D:\Program Files\Microsoft SDKs\Windows\v6.1\Bin\
ECHO Testing v6.1 SDK installed on D:...
IF EXIST "%SgenPath%" GOTO RUN

REM v7.0 SDK Installed
SET SgenPath=C:\Program Files\Microsoft SDKs\Windows\v7.0\Bin\
ECHO Testing v7.0 SDK...
IF EXIST "%SgenPath%" GOTO RUN

REM v7.0 SDK Installed on D: (For BattleClinic)
SET SgenPath=D:\Program Files\Microsoft SDKs\Windows\v7.0\Bin\
ECHO Testing v7.0 SDK installed on D:...
IF EXIST "%SgenPath%" GOTO RUN

ECHO SGen not found, check the SDK is installed.
EXIT /B 1
:RUN

ECHO Found SGen output follows:
"%SgenPath%sgen.exe" %1 /force