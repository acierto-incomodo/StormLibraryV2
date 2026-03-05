[Setup]
AppName=StormLibrary by StormGamesStudios
AppVersion=1.0.9
DefaultDirName={userappdata}\StormGamesStudios\StormLibraryV2
DefaultGroupName=StormGamesStudios
OutputDir=C:\Users\melio\source\repos\acierto-incomodo\StormLibraryV2\output
OutputBaseFilename=StormLibraryV2_Installer
Compression=lzma
SolidCompression=yes
AppCopyright=Copyright © 2025 StormGamesStudios. All rights reserved.
VersionInfoCompany=StormGamesStudios
AppPublisher=StormGamesStudios
SetupIconFile=../logo.ico
VersionInfoVersion=1.0.9.0
CloseApplications=yes
CloseApplicationsFilter=StormLibrary.exe,StormStore.exe,installer_updater.exe
RestartIfNeededByRun=no
DisableDirPage=yes
DisableProgramGroupPage=yes

[Types]
Name: "full"; Description: "Full Installation"
Name: "normal"; Description: "Normal Installation"
Name: "custom"; Description: "Custom Installation"; Flags: iscustom

[Components]
Name: "main"; Description: "StormLibrary (Obligatorio)"; Flags: fixed; Types: full normal custom
Name: "runtime"; Description: "ASP.NET Core Runtime (Obligatorio)"; Flags: fixed; Types: full normal custom
Name: "stormstore"; Description: "StormStore (Opcional)"; Types: full custom

[Files]
; Archivos del lanzador
Source: "C:\Users\melio\source\repos\acierto-incomodo\StormLibraryV2\installer_updater.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "C:\Users\melio\source\repos\acierto-incomodo\StormLibraryV2\logo.ico"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "C:\Users\melio\source\repos\acierto-incomodo\StormLibraryV2\logo.png"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "C:\Users\melio\source\repos\acierto-incomodo\StormLibraryV2\Installer\aspnetcore-runtime-8.0.23-win-x64.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: runtime
Source: "C:\Users\melio\Documents\GitHub\StormStore\application\dist\StormStore-Setup.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: stormstore

[Icons]
; Acceso directo en el escritorio
Name: "{userdesktop}\StormLibrary"; Filename: "{app}\installer_updater.exe"; IconFilename: "{app}\logo.ico"

; Acceso directo en el menú de inicio dentro de la carpeta StormLauncher_HMCL-Edition
Name: "{commonprograms}\StormGamesStudios\StormLibrary"; Filename: "{app}\installer_updater.exe"; IconFilename: "{app}\logo.ico"
Name: "{commonprograms}\StormGamesStudios\Desinstalar StormLibrary"; Filename: "{uninstallexe}"; IconFilename: "{app}\logo.ico"

[Registry]
; Guardar ruta de instalación para poder desinstalar
Root: HKCU; Subkey: "Software\StormLibraryV2"; ValueType: string; ValueName: "Install_Dir"; ValueData: "{app}"

[UninstallDelete]
; Eliminar carpeta del appdata y acceso directo
Type: filesandordirs; Name: "{app}"

[Run]
; Instalar ASP.NET Core Runtime en silencio antes de ejecutar tu lanzador
Filename: "{app}\aspnetcore-runtime-8.0.23-win-x64.exe"; Parameters: "/quiet /norestart"; Flags: waituntilterminated; Components: runtime

; Instalar StormStore de forma opcional
Filename: "{app}\StormStore-Setup.exe"; Parameters: "/S"; Flags: waituntilterminated skipifsilent; Components: stormstore

; Ejecutar el lanzador después de la instalación
Filename: "{app}\installer_updater.exe"; Description: "Ejecutar StormLibrary"; Flags: nowait postinstall skipifsilent
Filename: "{userappdata}\StormGamesStudios\StormStore\StormStore.exe"; Description: "Ejecutar StormStore"; Flags: nowait postinstall skipifsilent; Components: stormstore

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssInstall then
  begin
    Exec('taskkill.exe', '/F /IM StormLibrary.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    Exec('taskkill.exe', '/F /IM StormLibrary.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Exec('taskkill.exe', '/F /IM installer_updater.exe', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;
