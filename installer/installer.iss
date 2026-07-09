; Inno Setup script template for FlaUIRecorder
; Build the app first: dotnet build src\FlaUIRecorder\FlaUIRecorder.csproj -c Release
; Then compile this script with Inno Setup Compiler.

#define MyAppName "FlaUI Recorder"
#define MyAppVersion "1.0"
#define MyAppPublisher "FlaUIRecorder"
#define MyAppExeName "FlaUIRecorder.exe"

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=..\output
OutputBaseFilename=FlaUIRecorder-Setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Files]
Source: "..\src\FlaUIRecorder\bin\Release\net461\win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
