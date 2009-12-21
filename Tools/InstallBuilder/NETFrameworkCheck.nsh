Function GetDotNETVersion
  Push $0
  Push $1
 
  System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i) i .r1"
  StrCmp $1 0 +2
    StrCpy $0 "not found"
 
  Pop $1
  Exch $0
FunctionEnd

Function VersionCompare
	!define VersionCompare `!insertmacro VersionCompareCall`
 
	!macro VersionCompareCall _VER1 _VER2 _RESULT
		Push `${_VER1}`
		Push `${_VER2}`
		Call VersionCompare
		Pop ${_RESULT}
	!macroend
 
	Exch $1
	Exch
	Exch $0
	Exch
	Push $2
	Push $3
	Push $4
	Push $5
	Push $6
	Push $7
 
	begin:
	StrCpy $2 -1
	IntOp $2 $2 + 1
	StrCpy $3 $0 1 $2
	StrCmp $3 '' +2
	StrCmp $3 '.' 0 -3
	StrCpy $4 $0 $2
	IntOp $2 $2 + 1
	StrCpy $0 $0 '' $2
 
	StrCpy $2 -1
	IntOp $2 $2 + 1
	StrCpy $3 $1 1 $2
	StrCmp $3 '' +2
	StrCmp $3 '.' 0 -3
	StrCpy $5 $1 $2
	IntOp $2 $2 + 1
	StrCpy $1 $1 '' $2
 
	StrCmp $4$5 '' equal
 
	StrCpy $6 -1
	IntOp $6 $6 + 1
	StrCpy $3 $4 1 $6
	StrCmp $3 '0' -2
	StrCmp $3 '' 0 +2
	StrCpy $4 0
 
	StrCpy $7 -1
	IntOp $7 $7 + 1
	StrCpy $3 $5 1 $7
	StrCmp $3 '0' -2
	StrCmp $3 '' 0 +2
	StrCpy $5 0
 
	StrCmp $4 0 0 +2
	StrCmp $5 0 begin newer2
	StrCmp $5 0 newer1
	IntCmp $6 $7 0 newer1 newer2
 
	StrCpy $4 '1$4'
	StrCpy $5 '1$5'
	IntCmp $4 $5 begin newer2 newer1
 
	equal:
	StrCpy $0 0
	goto end
	newer1:
	StrCpy $0 1
	goto end
	newer2:
	StrCpy $0 2
 
	end:
	Pop $7
	Pop $6
	Pop $5
	Pop $4
	Pop $3
	Pop $2
	Pop $1
	Exch $0
FunctionEnd

Var DOTNET_RETURN_CODE

Section "Microsoft .NET Framework v2.0"
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
  ${If} $0 == "not found"
     goto lbl_StartInstallNotFound
  ${EndIf}
  StrCpy $0 $0 "" 1
  ${VersionCompare} $0 "2.0" $1
  ${If} $1 == 2
     goto lbl_StartInstallTooLow
  ${EndIf}
  goto lbl_Done

  lbl_StartInstallNotFound:
  MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "Microsoft .NET Framework 2.0 is required, and you do not have any version installed.$\nYou must \
                    install it before continuing.$\nIf you choose to continue, you will need to be connected \
                    to the internet before proceeding.$\nWould you like to continue with the installation?" /SD IDNO IDYES lbl_Confirmed IDNO lbl_Cancelled

  lbl_StartInstallTooLow:
  Call GetDotNETVersion
  Pop $0
  MessageBox MB_ICONEXCLAMATION|MB_YESNO|MB_DEFBUTTON2 "Microsoft .NET Framework 2.0 is required (you only have $0).$\nYou must \
                    install it before continuing.$\nIf you choose to continue, you will need to be connected \
                    to the internet before proceeding.$\nWould you like to continue with the installation?" /SD IDNO IDYES lbl_Confirmed IDNO lbl_Cancelled

  lbl_Cancelled:                  
  Abort "Microsoft .NET Framework 2.0 is required."

  lbl_Confirmed:
  nsisdl::download \
         /TIMEOUT=30000 "http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe" "$PLUGINSDIR\dotnetfx.exe"
  Pop $0
  StrCmp "$0" "success" lbl_continue
  Abort ".NET Framework v2.0 download failed: $0"

  lbl_continue:
  Banner::show /NOUNLOAD "Installing Microsoft .NET Framework 2.0..."
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


