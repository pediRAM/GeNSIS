﻿/***************************************************************************************
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
    using GeNSIS.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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
        private IAppData m_AppData = null;
        private TextGeneratorOptions m_Options = null;
        private int ln = 0;
        #endregion Variables


        #region Methods

        public bool IsCompanyDirEnabled()
            => (m_AppData.DoCreateCompanyDir && string.IsNullOrWhiteSpace(m_AppData.Company));


        #region NSIS Code Creation

        public string Generate(IAppData pAppData, TextGeneratorOptions pOptions)
        {
            ln = 1;
            sb.Clear();
            m_AppData = pAppData;
            m_Options = pOptions;

            var totalFilesSizeMainSection = pAppData.GetFiles().Where(x => x.FileSystemType == Enums.EFileSystemType.File).Sum(x => new FileInfo(x.Path).Length);
            var totalDirsSizeMainSection  = pAppData.GetFiles().Where(x => x.FileSystemType == Enums.EFileSystemType.Directory).Sum(x => new DirectoryInfo(x.Path).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));
           
            var totalDirSize = pAppData.GetSections().Sum(x => new DirectoryInfo(x.SourcePath).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));
            bool isOver16Mb = (totalDirSize + totalDirSize) > (ZLIB_SIZE_LIMIT);

            AddCommentHeader();
            Add();

            AddVariableDeclarations();
            AddStripline();
            Add();

            AddCompressionSelection(isOver16Mb);
            Add();

            AddCharacterEncoding();
            Add();

            AddInstallerExecutionAsAdmin();
            Add();

            AddAppName();
            Add();

            AddInstallerExeFilename();
            AddStripline();
            Add();

            AddUninstallPathKey();

            AddInstallationPerUserOrAllUsers();
            AddStripline();
            Add();

            AddModernGuiWithIconAndImages();
            AddStripline();
            Add();

            AddPages();
            AddStripline();
            Add();

            AddLanguages();
            AddStripline();
            Add();

            AddInstallationDir();
            Add();

            AddShowUnInstallationDetails();
            AddStripline();
            Add();

            AddMainSection();
            AddStripline();
            Add();
            AddSections();
            //Add();
            //AddShortCutSection();
            AddStripline();
            Add();

            AddPostSection();
            AddStripline();
            Add();

            AddUninstallMessage();
            AddStripline();
            Add();

            AddAskUserBeforeUninstall();
            AddStripline();
            Add();

            AddUninstallSection();
            Add();

            return sb.ToString();
        }
        private void AddCommentHeader()
        {
            AddStripline();
            AddComment($"Generated by GeNSIS {AsmConst.FULL_VERSION} at {DateTime.Now:yyyy-MM-dd HH:mm}");
            AddComment("For more information visit: https://github.com/pediRAM/GeNSIS");
            AddStripline();
            AddComment($"Copyright (C){DateTime.Now.Year} by {m_AppData.Publisher}");
            AddEmptyComment();
            AddComment($"This script creates installer for: {m_AppData.AppName} {m_AppData.AppVersion}");
            AddStripline();
        }

        private void AddVariableDeclarations()
        {
            AddComment("Variables:");
            AddComment("Name of Application:");
            AddDefine("APP_NAME", m_AppData.AppName);
            Add();

            AddComment("Filename of Application EXE file (*.exe):");
            AddDefine("APP_EXE_NAME", Path.GetFileName(m_AppData.ExeName.Path));
            Add();

            AddComment("Version of Application:");
            AddDefine("APP_VERSION", m_AppData.AppVersion);
            Add();

            AddComment("Build of Application:");
            if (string.IsNullOrWhiteSpace(m_AppData.AppBuild))
                AddDefine("APP_BUILD", "build");
            else
                AddDefine("APP_BUILD", m_AppData.AppBuild);
            Add();

            AddComment("Architecture of Application:");
            AddDefine("APP_ARCH", m_AppData.Arch);
            Add();

            AddComment("Machine type of Application:");
            AddDefine("APP_MACHINE_TYPE", m_AppData.MachineType);
            Add();

            AddComment("Application Publisher (company, organisation, author):");
            AddDefine("APP_PUBLISHER", m_AppData.Publisher);
            Add();

            AddComment("Name or initials of the company, organisation or author:");
            if (IsCompanyDirEnabled())
                AddComment("!define COMPANY_NAME \"UNKNOWN\"");
            else
                AddDefine("COMPANY_NAME", m_AppData.Company);
            Add();

            AddComment("URL of the Application Website starting with 'https://' :");
            AddDefine("APP_URL", m_AppData.Url);
            Add();

            AddComment("Name of setup/installer EXE file (*.exe):");
            if (string.IsNullOrWhiteSpace(m_AppData.InstallerFileName))
                AddDefine("SETUP_EXE_NAME", $"Setup_${{APP_NAME}}_${{APP_VERSION}}_${{APP_BUILD}}_${{APP_MACHINE_TYPE}}_${{APP_ARCH}}.exe");
            else
            {
                AddDefine("SETUP_EXE_NAME", m_AppData.InstallerFileName);
                //AddComment("Just in case you want to use the dynamic setup-naming:");
                //AddComment($"!define SETUP_EXE_NAME \"Setup_${{APP_NAME}}_${{APP_VERSION}}_${{APP_BUILD}}_${{APP_MACHINE_TYPE}}_${{APP_ARCH}}.exe\"");
            }
        }

        private void AddCompressionSelection(bool pIsOver16MB)
        {
            AddComment("Available compressions: zlib, bzip2, lzma");
            if (pIsOver16MB)
                Add("SetCompressor lzma");
            else
                Add("SetCompressor zlib");
        }

        private void AddCharacterEncoding()
            => Add("Unicode true");

        private void AddInstallerExecutionAsAdmin()
        {
            if (!m_AppData.DoInstallPerUser)
                Add("RequestExecutionLevel admin");
        }
        private void AddAppName()
        {
            AddComment("Displayed and registered name:");
            Add("Name \"${APP_NAME} ${APP_VERSION}\"");
        }
        private void AddInstallerExeFilename()
        {
            AddComment("You can also use: \"Setup_${APP_NAME}_${APP_VERSION}.exe\"");
            Add("OutFile \"${SETUP_EXE_NAME}\"");
        }

        private void AddUninstallPathKey()
        {
            AddComment("Path of uninstallation keys in registry:");
            AddDefine("UNINST_KEY", "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\${APP_NAME}");
        }

        private void AddInstallationPerUserOrAllUsers()
        {
            // Installation mode:
            //  - Per User
            //  - Shared (for all users)
            if (m_AppData.DoInstallPerUser)
                AddDefine("UNINST_ROOT_KEY", "HKCU");
            else
                AddDefine("UNINST_ROOT_KEY", "HKLM");
        }

        private void AddModernGuiWithIconAndImages()
        {
            // Modern GUI
            AddComment("Using modern user interface for installer:");
            Add("!include \"MUI.nsh\"");
            Add();

            // Installer Icon
            AddComment("Installer icons (*.ico):");
            if (string.IsNullOrWhiteSpace(m_AppData.InstallerIcon))
                AddDefine("MUI_ICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-install.ico");
            else
                AddDefine("MUI_ICON", m_AppData.InstallerIcon);
            Add();

            // Uninstaller Icon
            AddComment("Uninstaller icon (*.ico):");
            if (string.IsNullOrWhiteSpace(m_AppData.InstallerIcon))
                AddDefine("MUI_UNICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-uninstall.ico");
            else
                AddDefine("MUI_UNICON", m_AppData.InstallerIcon);

            Add();

            // Installer/Uninstaller Header Images?
            if (!string.IsNullOrWhiteSpace(m_AppData.InstallerHeaderImage) || !string.IsNullOrWhiteSpace(m_AppData.UninstallerHeaderImage))
            {
                AddDefine("MUI_HEADERIMAGE");
                Add();
            }

            // Installer Header Image
            if (!string.IsNullOrWhiteSpace(m_AppData.InstallerHeaderImage))
            {
                AddComment("Installer Header Image:");
                AddDefine("MUI_HEADERIMAGE_BITMAP", m_AppData.InstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_BITMAP_NOSTRETCH");
                Add();
            }

            // Uninstaller Header Image
            if (!string.IsNullOrWhiteSpace(m_AppData.UninstallerHeaderImage))
            {
                AddComment("Uninstaller Header Image:");
                AddDefine("MUI_HEADERIMAGE_UNBITMAP", m_AppData.UninstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH");
                Add();
            }

            // Installer Wizard Image (left side)
            if (!string.IsNullOrWhiteSpace(m_AppData.InstallerWizardImage))
            {
                AddComment("Installer Wizard Image:");
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP", m_AppData.InstallerWizardImage);
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
                Add();
            }

            // Uninstaller Wizard Image (left side)
            if (!string.IsNullOrWhiteSpace(m_AppData.UninstallerWizardImage))
            {
                AddComment("Uninstaller Wizard Image:");
                AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP", m_AppData.InstallerWizardImage);
                AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
                Add();
            }
            Add();
            Add("!define MUI_ABORTWARNING");
        }

        private void AddPages()
        {
            AddComment("Show welcome page:");
            AddInsertMacro("MUI_PAGE_WELCOME");

            if (!string.IsNullOrWhiteSpace(m_AppData.License.Path))
            {
                AddComment("License file (*.txt|*.rtf):");
                AddInsertMacro("MUI_PAGE_LICENSE", m_AppData.License.Path);
            }

            AddInsertMacro("MUI_PAGE_DIRECTORY");
            AddInsertMacro("MUI_PAGE_COMPONENTS");
            AddInsertMacro("MUI_PAGE_INSTFILES");
            AddInsertMacro("MUI_PAGE_FINISH");
            AddInsertMacro("MUI_UNPAGE_INSTFILES");
        }

        private void AddLanguages()
        {
            // Packed Translations for installer:
            AddComment("Available languages (first one is the default):");
            if (m_Options.Languages == null || m_Options.Languages.IsEmpty())
            {
                AddInsertMacro("MUI_LANGUAGE", "English");
            }
            else
            {
                foreach (var lang in m_Options.Languages)
                    AddInsertMacro("MUI_LANGUAGE", lang.Name);
            }
            Add();
            AddComment("Function to show the language selection page:");
            Add("Function .onInit");
            AddInsertMacro("MUI_LANGDLL_DISPLAY");
            Add("FunctionEnd");
        }

        private void AddInstallationDir()
        {
            AddComment("Installation folder (Programs\\Company\\Application):");
            if (m_AppData.DoInstallPerUser)
            {
                if (m_AppData.DoCreateCompanyDir && !string.IsNullOrWhiteSpace(m_AppData.Company))
                    Add("InstallDir \"$LocalAppData\\Programs\\${COMPANY_NAME}\\${APP_NAME}\"");
                else
                    Add("InstallDir \"$LocalAppData\\Programs\\${APP_NAME}\"");
            }
            else
            {
                if (m_AppData.DoCreateCompanyDir && !string.IsNullOrWhiteSpace(m_AppData.Company))
                    Add("InstallDir \"$ProgramFiles\\${COMPANY_NAME}\\${APP_NAME}\"");
                else
                    Add("InstallDir \"$ProgramFiles\\${APP_NAME}\"");
            }
        }

        private void AddShowUnInstallationDetails()
        {
            AddComment("Showing details while (un)installation:");
            Add("ShowInstDetails show");
            Add("ShowUninstDetails show");
        }

        private void AddMainSection()
        {
            //Add($"Section \"{m_AppData.AppName}\" SEC01");
            Add($"Section !Required");
            Add("SectionIn RO");
            Add("SetOutPath \"$INSTDIR\"");
            Add("SetOverwrite ifnewer");
            Add();

            //if (m_AppData.GetDirectories().Any())
            //{
            //    AddComment("Add directories recursively (remove /r for non-recursively):");
            //    foreach (var s in m_AppData.GetDirectories())
            //        Add($"File /r \"{s}\"");
            //    AddStripline();
            //    Add();
            //}

            AddComment("Add files:");
            foreach (var s in m_AppData.GetFiles())
                Add($"File \"{s}\"");
            AddStripline();
            Add();

            AddComment("Create shortcuts on Desktop and Programs menu.");
            Add($"CreateShortcut \"$DESKTOP\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add($"CreateShortcut \"$SMPROGRAMS\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add("SectionEnd");
        }
        private int m_SectionCounter = 1;
        private void AddSections()
        {
            m_SectionCounter = 1;
            foreach (var dir in m_AppData.GetSections())
            {
                Add($"Section \"{dir.Name}\" SEC{m_SectionCounter++:d2}");
                if (string.IsNullOrEmpty(dir.TargetInstallDir))
                    Add($"SetOutPath \"$INSTDIR\"");
                else
                    Add($"SetOutPath \"$INSTDIR\\{dir.TargetInstallDir}\"");
                Add("SetOverwrite ifnewer");
                Add();

                AddComment("Add files:");
                foreach (var s in Directory.GetFiles(dir.SourcePath, "*", SearchOption.AllDirectories))
                    Add($"File \"{s}\"");
                Add("SectionEnd");
                AddStripline();
                Add();
            }
        }

        private void AddShortCutSection()
        {
            Add($"Section \"Create Shortcuts\" SEC{m_SectionCounter:d2}");
            AddComment("Create shortcuts on Desktop and Programs menu.");
            Add($"CreateShortcut \"$DESKTOP\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add($"CreateShortcut \"$SMPROGRAMS\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add("SectionEnd");
        }

        private void AddPostSection()
        {
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
        }

        private void AddUninstallMessage()
        {
            AddComment("After application is sucessfully uninstalled:");
            Add("Function un.onUninstSuccess");
            Add("HideWindow");
            AddComment("You can change the message to suite your needs:");
            Add("MessageBox MB_ICONINFORMATION|MB_OK \"Application successfully removed.\"");
            Add("FunctionEnd");
        }

        private void AddAskUserBeforeUninstall()
        {
            AddComment("Before starting to uninstall:");
            Add("Function un.onInit");
            AddComment("You can change the message to suite your needs:");
            Add("MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 \"Are you sure you want to remove ${APP_NAME}?\" IDYES +2");
            Add("Abort");
            Add("FunctionEnd");
        }

        private void AddUninstallSection()
        {
            Add("Section Uninstall");
            AddComment("Delete shortcuts on Desktop and Programs menu:");
            Add("Delete \"$DESKTOP\\${APP_NAME}.lnk\"");
            Add("Delete \"$SMPROGRAMS\\${APP_NAME}.lnk\"");


            if (m_AppData.GetSections().Any())
            {
                AddComment("Deleteing Subfolders:");
                foreach (var s in m_AppData.GetSections())
                {
                    if (!string.IsNullOrEmpty(s.TargetInstallDir))
                        Add($"RMDir /r \"$INSTDIR\\{Path.GetFileName(s.TargetInstallDir)}\"");
                }
                AddStripline();
            }

            AddComment("Deleting all files:");
            Add("Delete \"$INSTDIR\\*\"");
            Add("RMDir \"$INSTDIR\"");
            Add("RMDir \"$INSTDIR\\..\"");
            Add();
            AddComment("Deleting registration keys:");
            Add("DeleteRegKey ${UNINST_ROOT_KEY} \"${UNINST_KEY}\"");

            if (m_AppData.DoCreateCompanyDir && !string.IsNullOrWhiteSpace(m_AppData.Company))
                Add("DeleteRegKey ${UNINST_ROOT_KEY} \"SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\${COMPANY_NAME}\\${APP_NAME}\"");
            else
                Add("DeleteRegKey ${UNINST_ROOT_KEY} \"SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\${APP_NAME}\"");

            Add();
            Add("SetAutoClose true");
            Add("SectionEnd");
        }
        #endregion NSIS Code Creation

        #region Code Line Adding Methods
        private void Add()
        {
            sb.AppendLine();
            ln++;
        }

        private void Add(string s)
        {
            sb.AppendLine(s);
            ln++;
        }

        private void AddComment(string pCommentLine)
        {
            sb.AppendLine($"; {pCommentLine}");
            ln++;
        }

        private void AddCommentBlock(IEnumerable<string> pCommentLines)
        {
            foreach (var s in pCommentLines)
                sb.AppendLine($"; {s}");
            ln += pCommentLines.Count();
        }

        private void AddEmptyComment()
        {
            sb.AppendLine(";");
            ln++;
        }

        private void AddStripline(int pPadRight = STRIPLINE_LENGTH)
        {
            sb.AppendLine(";".PadRight(pPadRight, '-'));
            ln++;
        }

        private void AddDefine(string pVarName, string pValue)
        {
            sb.AppendLine($"!define {pVarName} \"{pValue}\"");
            ln++;
        }

        private void AddDefine(string pVarName)
        {
            sb.AppendLine($"!define {pVarName}");
            ln++;
        }

        private void AddLog(string pValue)
        {
            sb.AppendLine($"!echo \"{pValue}\"");
            ln++;
        }

        private void AddInsertMacro(string pMacro, string pValue)
        {
            sb.AppendLine($"!insertmacro {pMacro} \"{pValue}\"");
            ln++;
        }

        private void AddInsertMacro(string pMacro)
        {
            sb.AppendLine($"!insertmacro {pMacro}");
            ln++;
        }
        #endregion Code Line Adding Methods

        #endregion Methods
    }
}
