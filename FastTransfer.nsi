; Fast Transfer NSIS Installer Script
; Requires NSIS 3.0 or later

;--------------------------------
; Includes

!include "MUI2.nsh"
!include "x64.nsh"
!include "FileFunc.nsh"

;--------------------------------
; General Settings

Name "FastTransfer"
OutFile "FastTransferSetup.exe"
Unicode True

; Default installation folder
InstallDir "$PROGRAMFILES64\FastTransfer"

; Get installation folder from registry if available
InstallDirRegKey HKLM "Software\FastTransfer" "InstallDir"

; Request application privileges
RequestExecutionLevel admin

; Compress installer
SetCompressor /SOLID lzma

; Version Information
VIProductVersion "2.0.0.0"
VIAddVersionKey "ProductName" "FastTransfer"
VIAddVersionKey "CompanyName" "FastTransfer"
VIAddVersionKey "FileDescription" "FastTransfer Installer"
VIAddVersionKey "FileVersion" "2.0.0.0"
VIAddVersionKey "ProductVersion" "2.0.0.0"
VIAddVersionKey "LegalCopyright" "© 2026 FastTransfer"

;--------------------------------
; Interface Settings

!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\nsis3-metro.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\nsis3-metro.bmp"

;--------------------------------
; Pages

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "LICENSE.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
; Languages

!insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Installer Sections

Section "Fast Transfer" SecMain
  SectionIn RO  ; Read-only section (always installed)
  
  SetOutPath "$INSTDIR"
  
  ; Copy application files
  File "FastTransfer\bin\Release\net8.0-windows\FastTransfer.exe"
  File "FastTransfer\bin\Release\net8.0-windows\FastTransfer.dll"
  File "FastTransfer\bin\Release\net8.0-windows\FastTransfer.runtimeconfig.json"
  File "FastTransfer\bin\Release\net8.0-windows\FastTransfer.deps.json"
  
  ; Copy all DLL dependencies
  File /nonfatal "FastTransfer\bin\Release\net8.0-windows\*.dll"
  
  ; Register context menu for files
  WriteRegStr HKCR "*\shell\FastTransfer" "" "FastTransfer"
  WriteRegStr HKCR "*\shell\FastTransfer" "Icon" "$INSTDIR\FastTransfer.exe"
  WriteRegStr HKCR "*\shell\FastTransfer\command" "" '"$INSTDIR\FastTransfer.exe" "%1"'
  
  ; Register context menu for directories
  WriteRegStr HKCR "Directory\shell\FastTransfer" "" "FastTransfer"
  WriteRegStr HKCR "Directory\shell\FastTransfer" "Icon" "$INSTDIR\FastTransfer.exe"
  WriteRegStr HKCR "Directory\shell\FastTransfer\command" "" '"$INSTDIR\FastTransfer.exe" "%1"'
  
  ; Register context menu for directory background (right-click in empty space)
  WriteRegStr HKCR "Directory\Background\shell\FastTransfer" "" "FastTransfer"
  WriteRegStr HKCR "Directory\Background\shell\FastTransfer" "Icon" "$INSTDIR\FastTransfer.exe"
  WriteRegStr HKCR "Directory\Background\shell\FastTransfer\command" "" '"$INSTDIR\FastTransfer.exe" "%V"'
  
  ; Refresh shell to apply changes
  System::Call 'shell32.dll::SHChangeNotify(i, i, i, i) v (0x08000000, 0, 0, 0)'
  
  ; Store installation folder
  WriteRegStr HKLM "Software\FastTransfer" "InstallDir" "$INSTDIR"
  
  ; Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
  ; Add uninstall information to Add/Remove Programs
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "DisplayName" "FastTransfer"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "QuietUninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "InstallLocation" "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "DisplayIcon" "$INSTDIR\FastTransfer.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "Publisher" "Fast Transfer"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "DisplayVersion" "2.0.0"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "NoRepair" 1
  
  ; Calculate and store installation size
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer" "EstimatedSize" "$0"
  
SectionEnd

;--------------------------------
; Uninstaller Section

Section "Uninstall"
  
  ; Remove context menu entries
  DeleteRegKey HKCR "*\shell\FastTransfer"
  DeleteRegKey HKCR "Directory\shell\FastTransfer"
  DeleteRegKey HKCR "Directory\Background\shell\FastTransfer"
  
  ; Refresh shell to apply changes
  System::Call 'shell32.dll::SHChangeNotify(i, i, i, i) v (0x08000000, 0, 0, 0)'
  
  ; Remove files
  Delete "$INSTDIR\FastTransfer.exe"
  Delete "$INSTDIR\FastTransfer.dll"
  Delete "$INSTDIR\FastTransfer.runtimeconfig.json"
  Delete "$INSTDIR\FastTransfer.deps.json"
  Delete "$INSTDIR\*.dll"
  Delete "$INSTDIR\Uninstall.exe"
  
  ; Remove directories
  RMDir "$INSTDIR"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FastTransfer"
  DeleteRegKey HKLM "Software\FastTransfer"
  
SectionEnd

;--------------------------------
; Installer Functions

Function .onInit
  
  ; Check if running on 64-bit Windows
  ${If} ${RunningX64}
    ; 64-bit Windows
    SetRegView 64
  ${Else}
    MessageBox MB_OK|MB_ICONSTOP "Fast Transfer requires 64-bit Windows."
    Abort
  ${EndIf}
  
  ; Check if .NET 8 Desktop Runtime is installed
  ReadRegStr $0 HKLM "SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost" "Version"
  ${If} $0 == ""
    MessageBox MB_YESNO|MB_ICONQUESTION ".NET 8 Desktop Runtime is required but not detected. Do you want to continue anyway?$\n$\n(The application will not work without .NET 8 Desktop Runtime)" IDYES +2
    Abort
  ${EndIf}
  
  ; Check if already installed
  ReadRegStr $0 HKLM "Software\FastTransfer" "InstallDir"
  ${If} $0 != ""
    MessageBox MB_YESNO|MB_ICONQUESTION "FastTransfer is already installed in:$\n$0$\n$\nDo you want to reinstall?" IDYES +2
    Abort
  ${EndIf}
  
FunctionEnd

Function .onInstSuccess
  MessageBox MB_OK "FastTransfer has been successfully installed!$\n$\nYou can now right-click on any file or folder and select 'Fast Transfer'."
FunctionEnd

;--------------------------------
; Uninstaller Functions

Function un.onInit
  
  ${If} ${RunningX64}
    SetRegView 64
  ${EndIf}
  
  MessageBox MB_YESNO|MB_ICONQUESTION "Are you sure you want to uninstall FastTransfer?" IDYES +2
  Abort
  
FunctionEnd

Function un.onUninstSuccess
  MessageBox MB_OK "FastTransfer has been successfully uninstalled."
FunctionEnd
