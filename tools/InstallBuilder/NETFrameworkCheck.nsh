Function GetDotNETVersion
  ReadRegStr $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  push $0
FunctionEnd

Var DOTNET_RETURN_CODE

Section "Microsoft .NET Framework v4.6.1"
  SectionIn RO
  
  ; search for /SKIPDOTNET on commandline and skip if found
  Push $CMDLINE
  Push "/SKIPDOTNET"
  Call StrStr
  Pop $0
  StrCmp $0 "" lbl_notSkipped
  goto lbl_Done
  
  lbl_notSkipped:
  Call GetDotNETVersion
  Pop $0
  ; On all other OS versions
  strCmp $0 "394271" lbl_Done
  ; On Windows 10 November Update systems
  strCmp $0 "394254" lbl_Done
  goto lbl_DotNetVersionNotFound

  lbl_DotNetVersionNotFound:
  MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "Microsoft .NET Framework v4.6.1 is required, and does not appear to be installed.$\nYou must \
                    install it before continuing.$\nIf you choose to continue, you will need to be connected \
                    to the internet before proceeding.$\nWould you like to continue with the installation?" /SD IDNO IDYES lbl_Confirmed IDNO lbl_Cancelled

  lbl_Cancelled:                  
  Abort "Microsoft .NET Framework v4.6.1 is required."

  lbl_Confirmed:
  nsisdl::download \
         /TIMEOUT=120000 "https://download.microsoft.com/download/E/4/1/E4173890-A24A-4936-9FC9-AF930FE3FA40/NDP461-KB3102436-x86-x64-AllOS-ENU.exe" "$PLUGINSDIR\dotnetfx.exe"
  Pop $0
  StrCmp "$0" "success" lbl_continue
  Abort "Microsoft .NET Framework v4.6.1 download failed"

  lbl_continue:
  Banner::show /NOUNLOAD "Installing .NET Framework v4.6.1..."
  nsExec::ExecToStack '"$PLUGINSDIR\dotnetfx.exe" /q /c:"install.exe /q"'
  pop $DOTNET_RETURN_CODE
  Banner::destroy
  SetRebootFlag true

  StrCmp "$DOTNET_RETURN_CODE" "" lbl_NoError
  StrCmp "$DOTNET_RETURN_CODE" "0" lbl_NoError
  StrCmp "$DOTNET_RETURN_CODE" "3010" lbl_NoError
  StrCmp "$DOTNET_RETURN_CODE" "8192" lbl_NoError
  StrCmp "$DOTNET_RETURN_CODE" "error" lbl_Error
  StrCmp "$DOTNET_RETURN_CODE" "timeout" lbl_TimeOut
  Goto lbl_Error

  lbl_TimeOut:
  Abort "The .NET Framework download timed out."

  lbl_Error:
  Abort "The .NET Framework install failed (error code $DOTNET_RETURN_CODE)."
  
  lbl_NoError:
  lbl_Done:
SectionEnd