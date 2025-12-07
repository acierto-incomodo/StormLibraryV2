[Setup]
AppName=StormLibrary by StormGamesStudios
AppVersion=1.0.6
DefaultDirName={userappdata}\StormGamesStudios\StormLibraryV2
DefaultGroupName=StormGamesStudios
OutputDir=C:\Users\mapsp\source\repos\StormLibrary\output
OutputBaseFilename=StormLibraryV2_Installer
Compression=lzma
SolidCompression=yes
AppCopyright=Copyright © 2025 StormGamesStudios. All rights reserved.
VersionInfoCompany=StormGamesStudios
AppPublisher=StormGamesStudios
SetupIconFile=../logo.ico
VersionInfoVersion=1.0.6.0
CloseApplications=yes
CloseApplicationsFilter=StormLibrary.exe
DisableDirPage=yes
DisableProgramGroupPage=yes

[Files]
; Archivos del lanzador
Source: "C:\Users\mapsp\source\repos\StormLibrary\dist\installer_updater.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\mapsp\source\repos\StormLibrary\logo.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\mapsp\source\repos\StormLibrary\logo.png"; DestDir: "{app}"; Flags: ignoreversion

Source: "C:\Users\mapsp\source\repos\StormLibrary\Installer\aspnetcore-runtime-8.0.22-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall

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
Filename: "{tmp}\aspnetcore-runtime-8.0.22-win-x64.exe"; Parameters: "/quiet /norestart"; Flags: waituntilterminated

; Ejecutar el lanzador después de la instalación
Filename: "{app}\installer_updater.exe"; Description: "Ejecutar StormLibrary"; Flags: nowait postinstall skipifsilent
