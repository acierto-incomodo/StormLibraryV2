[Setup]
AppName=StormLibrary by StormGamesStudios
AppVersion=1.0.8
DefaultDirName={userappdata}\StormGamesStudios\StormLibraryV2
DefaultGroupName=StormGamesStudios
OutputDir=C:\Users\mapsp\Documents\GitHub\StormLibraryV2\output
OutputBaseFilename=StormLibraryV2_Installer
Compression=lzma
SolidCompression=yes
AppCopyright=Copyright © 2025 StormGamesStudios. All rights reserved.
VersionInfoCompany=StormGamesStudios
AppPublisher=StormGamesStudios
SetupIconFile=../logo.ico
VersionInfoVersion=1.0.8.0
CloseApplications=yes
CloseApplicationsFilter=StormLibrary.exe
DisableDirPage=yes
DisableProgramGroupPage=yes

[Components]
Name: "main"; Description: "StormLibrary (Obligatorio)"; Flags: fixed
Name: "runtime"; Description: "ASP.NET Core Runtime (Obligatorio)"; Flags: fixed
Name: "stormstore"; Description: "StormStore (Opcional)"

[Files]
; Archivos del lanzador
Source: "C:\Users\mapsp\source\repos\StormLibrary\installer_updater.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "C:\Users\mapsp\source\repos\StormLibrary\logo.ico"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "C:\Users\mapsp\source\repos\StormLibrary\logo.png"; DestDir: "{app}"; Flags: ignoreversion; Components: main
Source: "C:\Users\mapsp\Documents\GitHub\StormLibraryV2\Installer\aspnetcore-runtime-8.0.23-win-x64.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: runtime
Source: "C:\Users\mapsp\Documents\GitHub\StormLibraryV2\Installer\StormStore-Setup-1.1.4.exe"; DestDir: "{app}"; Flags: ignoreversion; Components: stormstore

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
Filename: "{app}\StormStore-Setup-1.1.4.exe"; Flags: waituntilterminated skipifsilent; Components: stormstore

; Ejecutar el lanzador después de la instalación
Filename: "{app}\installer_updater.exe"; Description: "Ejecutar StormLibrary"; Flags: nowait postinstall skipifsilent
