@echo off
IF (%2)==(Release) GOTO RELEASE
:DEBUG
echo DEBUG set, won't be building installers
GOTO END
:RELEASE
cd %1
echo DEBUG not set, building installers
SubWCRev.exe . ".\Properties\AssemblyInfo.template.cs" ".\Properties\AssemblyInfo.cs" -f
:END