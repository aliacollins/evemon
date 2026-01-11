; EVEMon Installer Script for Inno Setup 6.x
; Download Inno Setup from: https://jrsoftware.org/isinfo.php
;
; Build with: iscc /DMyAppVersion=5.2.0 EVEMon.iss
; Or use: build-installer.ps1

#ifndef MyAppVersion
  #define MyAppVersion "5.2.0"
#endif

#define MyAppName "EVEMon"
#define MyAppPublisher "Alia Collins"
#define MyAppURL "https://github.com/aliacollins/evemon"
#define MyAppExeName "EVEMon.exe"

; .NET 8.0 Desktop Runtime download URL (x64)
#define DotNet8DownloadUrl "https://download.visualstudio.microsoft.com/download/pr/76e5dbb2-6ae3-4629-9a84-527f8571571b/09002599b32d5d01dc3aa5ef68b5e2ae/windowsdesktop-runtime-8.0.11-win-x64.exe"
#define DotNet8InstallerFile "windowsdesktop-runtime-8.0.11-win-x64.exe"

[Setup]
AppId={{8B3D3C6F-5A7E-4B9A-9D5C-3F2A1B4C5D6E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\src\EVEMon.Common\Resources\License\gpl.txt
; Output to publish folder with name matching UpdateManager pattern
OutputDir=..\publish
OutputBaseFilename=EVEMon-install-{#MyAppVersion}
SetupIconFile=..\src\EVEMon\Properties\EVEMon.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}
; Allow non-admin install to user's local app data
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
Source: "..\publish\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
var
  DownloadPage: TDownloadWizardPage;
  DotNetRequired: Boolean;

function IsDotNet8DesktopInstalled: Boolean;
var
  ResultCode: Integer;
  TempFile: String;
  Output: AnsiString;
begin
  Result := False;
  TempFile := ExpandConstant('{tmp}\dotnet_check.txt');

  // Run dotnet --list-runtimes and save to temp file
  if Exec('cmd.exe', '/c dotnet --list-runtimes 2>nul | findstr /C:"Microsoft.WindowsDesktop.App 8." > "' + TempFile + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if LoadStringFromFile(TempFile, Output) then
    begin
      Result := Length(Output) > 0;
    end;
    DeleteFile(TempFile);
  end;
end;

function OnDownloadProgress(const Url, FileName: String; const Progress, ProgressMax: Int64): Boolean;
begin
  if Progress = ProgressMax then
    Log(Format('Successfully downloaded file to {tmp}: %s', [FileName]));
  Result := True;
end;

procedure InitializeWizard;
begin
  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), @OnDownloadProgress);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
  DotNetInstaller: String;
begin
  Result := True;

  if CurPageID = wpReady then
  begin
    // Check if .NET 8 Desktop Runtime is installed
    if not IsDotNet8DesktopInstalled then
    begin
      DotNetRequired := True;

      if MsgBox('EVEMon requires .NET 8.0 Desktop Runtime which is not installed.'#13#10#13#10 +
                'Would you like to download and install it now?'#13#10#13#10 +
                '(Size: approximately 55 MB)', mbConfirmation, MB_YESNO) = IDYES then
      begin
        DownloadPage.Clear;
        DownloadPage.Add('{#DotNet8DownloadUrl}', '{#DotNet8InstallerFile}', '');
        DownloadPage.Show;
        try
          try
            DownloadPage.Download;

            // Run the .NET installer
            DotNetInstaller := ExpandConstant('{tmp}\{#DotNet8InstallerFile}');
            if FileExists(DotNetInstaller) then
            begin
              Log('Running .NET 8 Desktop Runtime installer...');
              // /quiet for silent install, /norestart to prevent restart
              if not Exec(DotNetInstaller, '/install /quiet /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
              begin
                MsgBox('Failed to run .NET 8 installer. Error code: ' + IntToStr(ResultCode), mbError, MB_OK);
                Result := False;
              end
              else if ResultCode <> 0 then
              begin
                // ResultCode 3010 means success but restart required
                if ResultCode = 3010 then
                begin
                  MsgBox('.NET 8 Desktop Runtime installed successfully.'#13#10 +
                         'A system restart may be required after EVEMon installation.', mbInformation, MB_OK);
                end
                else
                begin
                  MsgBox('.NET 8 installer returned code: ' + IntToStr(ResultCode) + #13#10 +
                         'EVEMon may not run correctly.', mbError, MB_OK);
                end;
              end;
            end;
          except
            if DownloadPage.AbortedByUser then
              Log('Download aborted by user.')
            else
              MsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK);
            Result := False;
          end;
        finally
          DownloadPage.Hide;
        end;
      end
      else
      begin
        // User declined to download .NET
        Result := MsgBox('EVEMon will not run without .NET 8.0 Desktop Runtime.'#13#10#13#10 +
                         'You can download it later from:'#13#10 +
                         'https://dotnet.microsoft.com/download/dotnet/8.0'#13#10#13#10 +
                         'Continue installation anyway?', mbConfirmation, MB_YESNO) = IDYES;
      end;
    end;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S: String;
begin
  S := '';

  if DotNetRequired then
  begin
    S := S + 'Prerequisites to install:' + NewLine;
    S := S + Space + '.NET 8.0 Desktop Runtime' + NewLine + NewLine;
  end;

  if MemoDirInfo <> '' then
    S := S + MemoDirInfo + NewLine + NewLine;

  if MemoGroupInfo <> '' then
    S := S + MemoGroupInfo + NewLine + NewLine;

  if MemoTasksInfo <> '' then
    S := S + MemoTasksInfo + NewLine + NewLine;

  Result := S;
end;
