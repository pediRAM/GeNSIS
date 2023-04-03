/***************************************************************************************
* GeNSIS - a free and open source NSIS installer script generator tool.                *
* Copyright (C) 2023 Pedram Ganjeh Hadidi                                              *
*                                                                                      *
* This file is part of GeNSIS.                                                         *
*                                                                                      *
* GeNSIS is free software: you can redistribute it and/or modify it under the terms    *
* of the GNU General Public License as published by the Free Software Foundation,      *
* either version 3 of the License, or any later version.                               *
*                                                                                      *
* GeNSIS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;  *
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     *
* PURPOSE. See the GNU General Public License for more details.                        *
*                                                                                      *
* You should have received a copy of the GNU General Public License along with GeNSIS. *
* If not, see <https://www.gnu.org/licenses/>.                                         *
****************************************************************************************/


namespace GeNSIS.Core.TextGenerators
{
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class NsisGenerator : ITextGenerator
    {
        #region Constants
        /// <summary>
        /// Filesize limitation for zlib usage (when greater lzma will be used).
        /// </summary>
        public const int ZLIB_SIZE_LIMIT = 16 * 1024 * 1024;

        /// <summary>
        /// Number of * characters in the stripline.
        /// </summary>
        public const int STRIPLINE_LENGTH = 80;
        #endregion Constants

        #region Variables
        private StringBuilder sb = new StringBuilder();
        private IAppData d = null;
        private TextGeneratorOptions o = null;
        #endregion Variables

        #region Methods
        public string Generate(IAppData data, TextGeneratorOptions opt)
        {
            sb.Clear();
            d = data;
            o = opt;

            var totalSize = data.GetFiles().Sum(x => new FileInfo(x).Length);
            var totalDirSize = data.GetDirectories().Sum(x => new DirectoryInfo(x).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));
            bool isOver16Mb = (totalDirSize + totalDirSize) > (ZLIB_SIZE_LIMIT);
            AddCommentHeader();

            AddComment("Variables:");
            AddComment("Name of Application:");
            AddDefine("APP_NAME", d.AppName);
            Add();

            AddComment("Filename of Application EXE file (*.exe):");
            AddDefine("APP_EXE_NAME", Path.GetFileName(d.ExeName));
            Add();

            AddComment("Version of Application:");
            AddDefine("APP_VERSION", d.AppVersion);
            Add();

            AddComment("Application Publisher (company, organisation, author):");
            AddDefine("APP_PUBLISHER", d.Publisher);
            Add();

            AddComment("Name or initials of the company, organisation or author:");
            AddDefine("COMPANY_NAME", d.Company);
            Add();

            AddComment("URL of the Application Website starting with 'https://' :");
            AddDefine("APP_URL", d.Url);
            Add();

            AddComment("Name of setup/installer EXE file (*.exe):");
            AddDefine("SETUP_EXE_NAME", d.InstallerFileName);
            AddComment("Instead of hardcoded name above, you can use the reusable one below (comment above and uncomment below line):");
            AddComment($"!define SETUP_EXE_NAME \"Setup_${{APP_NAME}}_${{APP_VERSION}}.exe\"");
            AddStripline();
            Add();

            AddComment("Available compressions: zlib, bzip2, lzma");
            if (isOver16Mb)
                Add("SetCompressor lzma");
            else
                Add("SetCompressor zlib");

            Add("Unicode true");

            if (!d.DoInstallPerUser)
                Add("RequestExecutionLevel admin");

            AddComment("Displayed and registered name:");
            Add("Name \"${APP_NAME} ${APP_VERSION}\"");
            
            AddComment("You can also use: \"Setup_${APP_NAME}_${APP_VERSION}.exe\"");
            Add("OutFile \"${SETUP_EXE_NAME}\"");
            AddStripline();
            Add();

            AddComment("Do not change this values!");
            AddDefine("UNINST_KEY", "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\${APP_NAME}");

            // Installation mode:
            //  - Per User
            //  - Shared (for all users)
            if(d.DoInstallPerUser)
                AddDefine("UNINST_ROOT_KEY", "HKCU");
            else
                AddDefine("UNINST_ROOT_KEY", "HKLM");
            AddStripline();
            Add();

            // Modern GUI
            AddComment("Using modern user interface for installer:");
            Add("!include \"MUI.nsh\"");
            Add();

            // Installer Icon
            AddComment("Installer icons (*.ico):");
            if (string.IsNullOrWhiteSpace(d.InstallerIcon))
                AddDefine("MUI_ICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-install.ico");
            else
                AddDefine("MUI_ICON", d.InstallerIcon);
            Add();

            // Uninstaller Icon
            AddComment("Uninstaller icon (*.ico):");
            if (string.IsNullOrWhiteSpace(d.InstallerIcon))
                AddDefine("MUI_UNICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-uninstall.ico");
            else
                AddDefine("MUI_UNICON", d.InstallerIcon);

            Add();

            // Installer/Uninstaller Header Images?
            if (!string.IsNullOrWhiteSpace(d.InstallerHeaderImage) || !string.IsNullOrWhiteSpace(d.UninstallerHeaderImage))
            {
                AddDefine("MUI_HEADERIMAGE");
                Add();
            }

            // Installer Header Image
            if (!string.IsNullOrWhiteSpace(d.InstallerHeaderImage))
            {
                AddComment("Installer Header Image:");
                AddDefine("MUI_HEADERIMAGE_BITMAP", d.InstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_BITMAP_NOSTRETCH");
                Add();
            }

            // Uninstaller Header Image
            if (!string.IsNullOrWhiteSpace(d.UninstallerHeaderImage))
            {
                AddComment("Uninstaller Header Image:");
                AddDefine("MUI_HEADERIMAGE_UNBITMAP", d.UninstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH");
                Add();
            }

            // Installer Wizard Image (left side)
            if (!string.IsNullOrWhiteSpace(d.InstallerWizardImage))
            {
                AddComment("Installer Wizard Image:");
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP", d.InstallerWizardImage);
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
                Add();
            }

            // Uninstaller Wizard Image (left side)
            if (!string.IsNullOrWhiteSpace(d.UninstallerWizardImage))
            {
                AddComment("Uninstaller Wizard Image:");
                AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP", d.InstallerWizardImage);
                AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
                Add();
            }
            AddStripline();
            Add();

            Add("!define MUI_ABORTWARNING");
            Add();

            AddComment("Show welcome page:");
            AddInsertMacro("MUI_PAGE_WELCOME");
            Add();

            if (!string.IsNullOrWhiteSpace(d.License))
            {
                AddComment("License file (*.txt|*.rtf):");
                AddInsertMacro("MUI_PAGE_LICENSE", d.License);
                Add();
            }

            AddInsertMacro("MUI_PAGE_DIRECTORY");
            AddInsertMacro("MUI_PAGE_INSTFILES");
            AddInsertMacro("MUI_PAGE_FINISH");
            AddInsertMacro("MUI_UNPAGE_INSTFILES");
            AddStripline();
            Add();

            // Packed Translations for installer:
            AddComment("Available languages (first one is the default):");
            if (o.Languages == null || o.Languages.IsEmpty())
            {
                AddInsertMacro("MUI_LANGUAGE", "English");
            }
            else
            {
                foreach (var lang in o.Languages)
                {
                    AddInsertMacro("MUI_LANGUAGE", lang.Name);
                }
            }
            Add();
            AddComment("Function to show the language selection page:");
            Add("Function .onInit");
            AddInsertMacro("MUI_LANGDLL_DISPLAY");
            Add("FunctionEnd");
            AddStripline();
            Add();

            AddComment("Installation folder (Programs\\Company\\Application):");
            if (d.DoInstallPerUser)
                Add("InstallDir \"$LocalAppData\\Programs\\${COMPANY_NAME}\\${APP_NAME}\"");
            else
                Add("InstallDir \"$ProgramFiles\\${COMPANY_NAME}\\${APP_NAME}\"");


            AddComment("Showing details while (un)installation:");
            Add("ShowInstDetails show");
            Add("ShowUninstDetails show");
            AddStripline();

            Add("Section \"MainSection\" SEC01");
            Add("SetOutPath \"$INSTDIR\"");
            Add("SetOverwrite ifnewer");

            if (data.GetDirectories().Any())
            {
                AddComment("Add directories recursively (remove /r for non-recursively):");
                foreach (var s in data.GetDirectories())
                    Add($"File /r \"{s}\"");
                AddStripline();
            }

            AddComment("Add files:");
            foreach (var s in d.GetFiles())
                Add($"File \"{s}\"");
            AddStripline();

            AddComment("Create shortcuts on Desktop and Programs menu.");
            Add($"CreateShortcut \"$DESKTOP\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add($"CreateShortcut \"$SMPROGRAMS\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add("SectionEnd");
            AddStripline();


            Add("Section -Post");
            Add("WriteUninstaller \"$INSTDIR\\uninst.exe\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"DisplayName\" \"$(^Name)\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"DisplayIcon\" \"$INSTDIR\\${APP_EXE_NAME}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"DisplayVersion\" \"${APP_VERSION}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"URLInfoAbout\" \"${APP_URL}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"Publisher\" \"${APP_PUBLISHER}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"UninstallString\" \"$INSTDIR\\uninst.exe\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"QuietUninstallString\" '\"$INSTDIR\\uninst.exe\" /S'");
            Add("SectionEnd");
            AddStripline();

            AddComment("After application is sucessfully uninstalled:");
            Add("Function un.onUninstSuccess");
            Add("HideWindow");
            AddComment("You can change the message to suite your needs:");
            Add("MessageBox MB_ICONINFORMATION|MB_OK \"Application successfully removed.\"");
            Add("FunctionEnd");
            AddStripline();

            AddComment("Before starting to uninstall:");
            Add("Function un.onInit");
            AddComment("You can change the message to suite your needs:");
            Add("MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 \"Are you sure you want to remove ${APP_NAME}?\" IDYES +2");
            Add("Abort");
            Add("FunctionEnd");
            AddStripline();


            Add("Section Uninstall");
            AddComment("Delete shortcuts on Desktop and Programs menu:");
            Add("Delete \"$DESKTOP\\${APP_NAME}.lnk\"");
            Add("Delete \"$SMPROGRAMS\\${APP_NAME}.lnk\"");


            if (data.GetDirectories().Any())
            {
                AddComment("Deleteing Subfolders:");
                foreach (var s in data.GetDirectories())
                    Add($"RMDir /r \"$INSTDIR\\{Path.GetFileName(s)}\"");
                AddStripline();
            }

            AddComment("Deleting all files:");
            Add("Delete \"$INSTDIR\\*\"");
            Add("RMDir \"$INSTDIR\"");
            Add("RMDir \"$INSTDIR\\..\"");
            Add("DeleteRegKey ${UNINST_ROOT_KEY} \"${UNINST_KEY}\"");
            Add("DeleteRegKey ${UNINST_ROOT_KEY} \"SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\${COMPANY_NAME}\\${APP_NAME}\"");
            Add("SetAutoClose true");
            Add("SectionEnd");
            Add();

            return sb.ToString();
        }

        private void AddCommentHeader()
        {
            AddStripline();
            AddComment($"Generated by GeNSIS { AsmConst.VERSION} at {DateTime.Now:yyyy-MM-dd HH:mm}");
            AddComment($"Copyright (C){DateTime.Now.Year} by {d.Publisher}");
            AddEmptyLine();
            AddComment($"This script creates installer for: {d.AppName} {d.AppVersion}");
            AddStripline();
        }

        private void Add() => sb.AppendLine();

        private void Add(string s) => sb.AppendLine(s);

        private void AddComment(string pCommentLine) 
            => sb.AppendLine($"; {pCommentLine}");

        private void AddCommentBlock(IEnumerable<string> pCommentLines)
        {
            foreach (var s in pCommentLines) 
                sb.AppendLine($"; {s}");
        }

        private void AddEmptyLine() => sb.AppendLine(";");

        private void AddStripline(int pPadRight = STRIPLINE_LENGTH) 
            => sb.AppendLine(";".PadRight(pPadRight, '-'));

        private void AddDefine(string pVarName, string pValue) 
            => sb.AppendLine($"!define {pVarName} \"{pValue}\"");

        private void AddDefine(string pVarName)
            => sb.AppendLine($"!define {pVarName}");

        private void AddLog(string pValue)
            => sb.AppendLine($"!echo \"{pValue}\"");

        private void AddInsertMacro(string pMacro, string pValue)
            => sb.AppendLine($"!insertmacro {pMacro} \"{pValue}\"");

        private void AddInsertMacro(string pMacro)
            => sb.AppendLine($"!insertmacro {pMacro}");
        #endregion Methods
    }
}
