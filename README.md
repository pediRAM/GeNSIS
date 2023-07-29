
- [About](#about)
- [Dependencies](#dependencies)
- [Step by Step Tutorial](#step-by-step-tutorial)
- [Application Files and Folders](#application-files-and-folders)
- [Application and Publisher](#application-and-publisher)
- [Installer](#installer)
- [Design](#design)
- [Generating the Script](#generating-the-script)
- [Compiling the Script](#compiling-the-script)
- [Testing the Installer](#testing-the-installer)
- [Saving the Project](#saving-the-project)
- [NSIS Script Code Example](#nsis-script-code-example)

# About
**GeNSIS** (an amalgamation of **Ge**nerate + **NSIS** or "Nullsoft Scriptable Install System"), is a Rapid Application Development ([RAD](https://en.wikipedia.org/wiki/Rapid_application_development)) tool that automates the writing of [NSIS](https://nsis.sourceforge.io/Main_Page) script for you. 

It enables you to **auto-generate** and build installers for your applications **fast** and **effortless** way.

Updating subsequent versions of your application becomes even more straightforward and efficient.

# Dependencies
- Windows operating system (XP, Vista, 7, 8, 10 or 11)
- .NET 6 runtime ([download .NET6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0))
- NSIS 3 ([download NSIS3+](https://nsis.sourceforge.io/Download))

# GeNSIS Installer
The **GeNSIS Installer(s)** are [here](https://github.com/pediRAM/GeNSIS/tree/main/Installers).
## GeNSIS File Structure
Upon first execution, GeNSIS will create the following folders:
- **Installers:** This is where the built installers (*.exe files) are located.
- **Projects:** All GeNSIS project files (*.gensis) should be saved here.
- **Scripts:** This contains all the generated NSIS scripts (*.nsi files).

# Step by Step Tutorial
## Application Files and Folders
After you have copied all needed files to a folder:
1. Click on the **"Add folder..."** button to open the dialog
2. Navigate to the folder containing the application installation files, then click on the **"Select folder"** button
3. This will add all the files to the project
4. The application executable will be detected automatically (you have the option to change this)
5. The license file (*.txt or *.rtf) will also be automatically detected (you can change this as well)

![Add files](source/Misc/Images/1_add_files.png)

---

## Application and Publisher
Now you can set the properties about your organisation/company, publisher etc. as follow:
1. (Optional) set/unset whether your app is an x64 app (is detected and set automatically)
2. (Optional) modify the auto-detected name of your app
3. (Optional) edit the auto-detected version of your app
4. (Optional) type an arbitary build like DEBUG, Release-Candidate, Demo ... or leave blank
5. (Optional) type the file extension which should be assigned to your app or leave blank
6. Enter the name or initials of your organization/company/...
7. Enter the name of your app's publisher
8. Enter the web-address/url of the app or company/organization homepage

![Properties about Application and Publisher](source/Misc/Images/2_app_and_publisher_data.png)

---

## Installer
In the tab **"Installer"** you can modify the installer, provided languages by your installer and installation behaviour:
1. Set/unset whether your app should be installed for user or for everyone
2. Whether the app folder should be nested in the company/organization folder or not
3. Edit the filename of installer
4. Clicking on **"Reset"** button will undo changes to **"Installer filename"**
5. You can create a static filename by clicking on the **"Static"** button
6. Clicking on **"Clear"** button will clear the **"Installer filename"** textbox
7. You can choose available languages for your installer from **"Available languages"** section
8. In the **"Added to Installer"** section you see the languages provided by your installer
9. Clicking on the **"A➝Z"** will order the available languages alphabetically
10. To order the available languages by number of native speakers, click on **"N➝1"**
11. Adds selected languages from **"Available languages"** to **"Added to Installer"** languages list
12. Adds all languages from **"Available languages"** to **"Added to Installer"** languages list
13. Removes selected languages from **"Added to Installer"** languages list
14. Resets the list of languages in **"Added to Installer"**
15. Moves selected language(s) to the top-most position
16. Moves selected language(s) one position up
17. Moves selected language(s) one postion down
18. Moves selected language(s) to last position

![Installer Properties](source/Misc/Images/3_installer_properties.png)

---

## Design
In the tab **"Design"** you can influence the appearance of your installer,
by choosing the ***Icon***, ***Wizzard Image*** and ***Header Image*** of your installer.

You can ignore this tab/step and go to the next tab/step **Script**, 
if the design or corporate-identity does not matter.

Clicking on following buttons will open the file dialog, which enables you to select the:
1. (Optional) **"Icon..."** of ***Installer*** during ***installation***
2. (Optional) **"Wizard image..."** of ***Installer*** during ***installation***
3. (Optional) **"Header image..."** of ***Installer*** during ***installation***
4. (Optional) **"Icon..."** of ***Uninstaller*** during ***deistallation***
5. (Optional) **"Wizard image..."** of ***Uninstaller*** during ***deistallation***
6. (Optional) **"Header image..."** of ***Uninstaller*** during ***deistallation***

![Design](source/Misc/Images/4_design_of_installer.png)

---

## Generating the Script
In the tab **"Script"** you generate, edit and save the NSIS script:
1. Click on the **"Generate script..."** button, which will open a dialog to save the script file
1. After choosing the path and name click on **"Save"** button of dialog

Script will now be generated, saved to the choosen path, then opened in the editor (see next images!).

![Generate NSIS script](source/Misc/Images/5_generate_script.png)

--

## Compiling the Script
Now you can, if needed, edit and modify the script, then compile it to build the installer:
1. (Optional) Click on the **"Save changes"** button, after you have made the modifications to the script
1. Now click on **"Compile..."** to compile it by ***MakeNSISW***

***MakeNSISW*** application will now open and start to build yor installer (see next section ***Test the Installer***!).
To test the built installer, just click on the button **"Test installer..."**:

![Generate Installer](source/Misc/Images/6_save_changes_and_generate_installer.png)

---

## Testing the Installer
If everything was OK, the background will become green, 
else yellow (with red marked error messages, line-number of code, where the error occured etc.).

To test the installer simply click on the button **"Test Installer"**.
In case of error(s) or warnings:
- Always read carefully the error/warning messages and try to understand what they mean
- Look carefully for typos in your NSIS script code, specially the "Copy & Paste" ones! Often one character is missed or some value after paste-action has not been changed
- Check if filepaths are OK and files exist
- In case of SYNTAX ERROR or other ERROR messages from NSIS compiler while generating the installer, read carefully the code line at given linenumber then compare it to examples in tutorials or [documentations](https://nsis.sourceforge.io/Docs/)
- If you are calling/using module(s) in your *.nsi script: have you installed the NSIS module you are calling? Is it up to date? Check the version and documentations of the module too!

![Testing Installer](source/Misc/Images/7_test_installer.png)

---

## Saving the Project
At the latest now, you should save your project by clicking on 
the **"Save as..."** in the **"Project"** submenu in **"File"** menue.
By saving the GeNSIS project, you are able to create an installer for the next version of 
your application even faster:

![Saving the Project](source/Misc/Images/8_save_gensis_project.png)

---

# NSIS Script Code Example

```nsis
;-------------------------------------------------------------------------------
; Generated by GeNSIS 0.1 Beta at 2023-07-24 22:10
; For more information visit: https://github.com/pediRAM/GeNSIS
;-------------------------------------------------------------------------------
; Copyright (C)2023 by John Doe
;
; This script creates installer for: MyApp 1.2.3
;-------------------------------------------------------------------------------

; Variables:
; Name of Application:
!define APP_NAME "MyApp"

; Filename of Application EXE file (*.exe):
!define APP_EXE_NAME "MyApp.exe"

; Version of Application:
!define APP_VERSION "1.2.3"

; Build of Application:
!define APP_BUILD "BETA"

; Architecture of Application:
!define APP_ARCH "x32"

; Machine type of Application:
!define APP_MACHINE_TYPE "I386"

; File extension associated to Application:
!define FILE_EXTENSION ".txt"

; Application Publisher (company, organisation, author):
!define APP_PUBLISHER "John Doe"

; Name or initials of the company, organisation or author:
!define COMPANY_NAME "ACME-Ltd."

; URL of the Application Website starting with 'https://' :
!define APP_URL "https://example-url.com/"

; Name of setup/installer EXE file (*.exe):
!define SETUP_EXE_NAME "Setup_${APP_NAME}_${APP_VERSION}_${APP_BUILD}_${APP_MACHINE_TYPE}_${APP_ARCH}.exe"
;-------------------------------------------------------------------------------

; Available compressions: zlib, bzip2, lzma
SetCompressor zlib

Unicode true

RequestExecutionLevel admin

; Displayed and registered name:
Name "${APP_NAME} ${APP_VERSION}"

; You can also use: "Setup_${APP_NAME}_${APP_VERSION}.exe"
OutFile "${SETUP_EXE_NAME}"
;-------------------------------------------------------------------------------

; Path of uninstallation keys in registry:
!define UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
!define UNINST_ROOT_KEY "HKLM"
;-------------------------------------------------------------------------------

; Using modern user interface for installer:
!include "MUI.nsh"

; Installer icons (*.ico):
!define MUI_ICON "C:\Program Files (x86)\NSIS\Contrib\Graphics\Icons\nsis3-install.ico"

; Uninstaller icon (*.ico):
!define MUI_UNICON "C:\Program Files (x86)\NSIS\Contrib\Graphics\Icons\nsis3-install.ico"

!define MUI_HEADERIMAGE

; Installer Header Image:
!define MUI_HEADERIMAGE_BITMAP "C:\Program Files (x86)\NSIS\Contrib\Graphics\Header\orange.bmp"
!define MUI_HEADERIMAGE_BITMAP_NOSTRETCH

; Installer Wizard Image:
!define MUI_WELCOMEFINISHPAGE_BITMAP "C:\Program Files (x86)\NSIS\Contrib\Graphics\Wizard\arrow.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH


!define MUI_ABORTWARNING
;-------------------------------------------------------------------------------

; Show welcome page:
!insertmacro MUI_PAGE_WELCOME
; License file (*.txt|*.rtf):
!insertmacro MUI_PAGE_LICENSE "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\License.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES
;-------------------------------------------------------------------------------

; Available languages (first one is the default):
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_LANGUAGE "French"
!insertmacro MUI_LANGUAGE "German"
!insertmacro MUI_LANGUAGE "Hindi"
!insertmacro MUI_LANGUAGE "Italian"
!insertmacro MUI_LANGUAGE "SimpChinese"
!insertmacro MUI_LANGUAGE "TradChinese"
!insertmacro MUI_LANGUAGE "Spanish"
!insertmacro MUI_LANGUAGE "Arabic"

; Function to show the language selection page:
Function .onInit
!insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd
;-------------------------------------------------------------------------------

; Installation folder (Programs\Company\Application):
InstallDir "$ProgramFiles\${APP_NAME}"

; Showing details while (un)installation:
ShowInstDetails show
ShowUninstDetails show
;-------------------------------------------------------------------------------

; Main Section (first component/section), which is mandatory. This means:
; user cannot unselect this component/section (if there are two or more).
Section "Required" SEC01
SetOutPath "$INSTDIR"
SetOverwrite ifnewer

; Add directories recursively (remove /r for non-recursively):
File /r "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\Manuals"
File /r "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\Tutorials"
;-------------------------------------------------------------------------------

; Add files:
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\D3DCompiler_47_cor3.dll"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\License.txt"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\MyApp.deps.json"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\MyApp.dll"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\MyApp.exe"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\MyApp.runtimeconfig.json"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\PenImc_cor3.dll"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\PresentationNative_cor3.dll"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\vcruntime140_cor3.dll"
File "H:\MyTemp\__Install Files for MyApp 1.2.3\MyApp 1.2.3\wpfgfx_cor3.dll"
File "C:\Program Files (x86)\NSIS\Contrib\Graphics\Icons\nsis3-install.ico"
;-------------------------------------------------------------------------------

; Create association of file extension to application:
WriteRegStr HKCR "${FILE_EXTENSION}" "" "${APP_NAME}"
WriteRegStr HKCR "${APP_NAME}" "" "${APP_NAME} File"
WriteRegStr HKCR "${APP_NAME}\DefaultIcon" "" "$INSTDIR\${APP_EXE_NAME},0"
WriteRegStr HKCR "${APP_NAME}\Shell\Open\Command" "" "$\"$INSTDIR\${APP_EXE_NAME}$\" $\"%1$\""
;-------------------------------------------------------------------------------

; Create shortcuts on Desktop and Programs menu.
CreateShortcut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE_NAME}" ""
CreateShortcut "$SMPROGRAMS\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE_NAME}" ""
SectionEnd
;-------------------------------------------------------------------------------

;-------------------------------------------------------------------------------

Section -Post
WriteUninstaller "$INSTDIR\uninst.exe"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "DisplayName" "$(^Name)"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "DisplayIcon" "$INSTDIR\${APP_EXE_NAME}"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "DisplayVersion" "${APP_VERSION}"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "URLInfoAbout" "${APP_URL}"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "Publisher" "${APP_PUBLISHER}"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
WriteRegStr ${UNINST_ROOT_KEY} "${UNINST_KEY}" "QuietUninstallString" '"$INSTDIR\uninst.exe" /S'
SectionEnd
;-------------------------------------------------------------------------------

; After application is sucessfully uninstalled:
Function un.onUninstSuccess
HideWindow
; You can change the message to suite your needs:
MessageBox MB_ICONINFORMATION|MB_OK "Application successfully removed."
FunctionEnd
;-------------------------------------------------------------------------------

; Before starting to uninstall:
Function un.onInit
; You can change the message to suite your needs:
MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to remove ${APP_NAME}?" IDYES +2
Abort
FunctionEnd
;-------------------------------------------------------------------------------

Section Uninstall
; Delete shortcuts on Desktop and Programs menu:
Delete "$DESKTOP\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\${APP_NAME}.lnk"
; Deleting all files:
Delete "$INSTDIR\*"
RMDir "$INSTDIR"
RMDir "$INSTDIR\.."

; Deleting registration keys:
DeleteRegKey ${UNINST_ROOT_KEY} "${UNINST_KEY}"
DeleteRegKey ${UNINST_ROOT_KEY} "${FILE_EXTENSION}"
DeleteRegKey ${UNINST_ROOT_KEY} "SOFTWARE\Microsoft\.NETFramework\v2.0.50727\AssemblyFoldersEx\${APP_NAME}"
DeleteRegKey ${UNINST_ROOT_KEY} "${APP_NAME}"

SetAutoClose true
SectionEnd

```

# Thirdparty Software and Content
GeNSIS and this documentation could not be created without following icons, tools, and libraries:
- [NSIS](http://nsis.sourceforge.io/) zlib license, copyright by [contributors](https://nsis.sourceforge.io/License)
- [Gnu Image Manipulation Program](https://www.gimp.org/) GPL-3.0 license, copyright by Spencer KIMBALL & Peter MATTIS
- [Nuvola](https://en.wikipedia.org/wiki/Nuvola) icons LGPL 2.1 license and copyright by [David VIGNONI](https://web.archive.org/web/20081026105155/http://www.icon-king.com/projects/nuvola/) the artist and creator of Nuvola icons
- [Greenshot](https://github.com/greenshot/greenshot), a very strightforward screenshot tool, GPL-3.0 license and copyright by [Robin KROM](https://github.com/Lakritzator)
- [AvalonEdit](http://avalonedit.net/) MIT license and copyright by [Daniel GRUNWALD](https://github.com/dgrunwald) ([source on Github](https://github.com/icsharpcode/AvalonEdit))
- [Visual Studio 2022 Community](https://visualstudio.microsoft.com/) licensed under [MICROSOFT VISUAL STUDIO COMMUNITY 2022](https://visualstudio.microsoft.com/license-terms/vs2022-ga-community/) copyright by Microsoft
- [Visual Studio Code](https://code.visualstudio.com/) licensed under [MICROSOFT VISUAL STUDIO CODE](https://code.visualstudio.com/license?lang=en)
- [ExtendedXmlSerializer](https://extendedxmlserializer.github.io/) MIT license and copyright by Wojciech NAGÓRSKI & Michael DEMOND
- [NLog](https://nlog-project.org/) BSD-3-Cluase license, copyright by Jarek KOWALSKI, Kim CHRISTENSEN, Julian VERDURMEN






 
