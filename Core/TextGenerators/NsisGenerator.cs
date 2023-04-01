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
            AddComment("This is the DISPLAYNAME of your application (remove spaces!):");
            AddDefine("PRODUCT_NAME", this.d.AppName);
            AddComment("This is the FILENAME of the executable/binary (*.exe) of your application:");
            AddDefine("PRODUCT_EXE_NAME", Path.GetFileName(this.d.ExeName));
            AddComment("Name of setup/installer file (*.exe):");
            AddDefine("SETUP_EXE_NAME", this.d.InstallerFileName);
            AddDefine("PRODUCT_VERSION", this.d.AppVersion);
            AddDefine("PRODUCT_PUBLISHER", this.d.Publisher);
            AddDefine("COMPANY_NAME", this.d.Company);
            AddDefine("PRODUCT_WEBSITE", this.d.Url);
            AddStripline();

            AddComment("Available compressions: zlib, bzip2, lzma");
            if (isOver16Mb)
                Add("SetCompressor lzma");
            else
                Add("SetCompressor zlib");

            Add("Unicode True");

            AddComment("Displayed and registered name:");
            Add("Name \"${PRODUCT_NAME} ${PRODUCT_VERSION}\"");

            
            AddComment("You can also use: \"Setup_${PRODUCT_NAME}_${PRODUCT_VERSION}.exe\"");
            Add("OutFile \"${SETUP_EXE_NAME}\"");


            AddStripline();

            AddComment("Do not change this values!");
            AddDefine("PRODUCT_UNINST_KEY", "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\${PRODUCT_NAME}");
            AddDefine("PRODUCT_UNINST_ROOT_KEY", "HKLM");
            AddStripline();

            AddComment("Using modern user interface for installer:");
            Add("!include \"MUI.nsh\"");

            if (!string.IsNullOrWhiteSpace(this.d.InstallerHeaderImage))
            {
                AddDefine("MUI_HEADERIMAGE");
                AddDefine("MUI_HEADERIMAGE_BITMAP", this.d.InstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_BITMAP_NOSTRETCH");
                AddDefine("MUI_HEADERIMAGE_UNBITMAP", this.d.InstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH");
            }

            if (!string.IsNullOrWhiteSpace(this.d.InstallerWizardImage))
            {
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP", this.d.InstallerWizardImage);
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
                AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP", this.d.InstallerWizardImage);
                AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
            }

            AddComment("Installer icons (*.ico):");
            if (string.IsNullOrWhiteSpace(this.d.InstallerIcon))
            {
                AddDefine("MUI_ICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-install.ico");
                AddDefine("MUI_UNICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-uninstall.ico");
            }
            else
            {
                AddDefine("MUI_ICON", this.d.InstallerIcon);
                AddDefine("MUI_UNICON", this.d.InstallerIcon);
            }
            AddStripline();
            Add("!define MUI_ABORTWARNING");
            AddStripline();

            AddComment("Show welcome page:");
            AddInsertMacro("MUI_PAGE_WELCOME");
            AddStripline();

            if (!string.IsNullOrWhiteSpace(this.d.License))
            {
                AddComment("License file (*.txt):");
                AddInsertMacro("MUI_PAGE_LICENSE", this.d.License);
                AddStripline();
            }

            AddInsertMacro("MUI_PAGE_DIRECTORY");
            AddInsertMacro("MUI_PAGE_INSTFILES");
            AddInsertMacro("MUI_PAGE_FINISH");
            AddInsertMacro("MUI_UNPAGE_INSTFILES");
            AddStripline();

            AddComment("Available languages (first one is the default):");
            if (o.Languages == null || o.Languages.IsEmpty())
            {
                AddInsertMacro("MUI_LANGUAGE", "English");
            }
            else
            {
                foreach (var lang in o.Languages.OrderBy(x => x.Name))
                {
                    AddInsertMacro("MUI_LANGUAGE", lang.Name);
                }
            }
            AddComment("Function to show the language selection page:");
            Add("Function .onInit");
            AddInsertMacro("MUI_LANGDLL_DISPLAY");
            Add("FunctionEnd");
            AddStripline();

            //AddComment("Displayed and registered name:");
            //Add("Name \"${PRODUCT_NAME} ${PRODUCT_VERSION}\"");

            //AddComment("Name of setup file (*.exe):");
            //Add("OutFile \"Setup_${PRODUCT_NAME}_${PRODUCT_VERSION}.exe\"");

            AddComment("Installation folder (Programs\\Company\\Application):");
            Add("InstallDir \"$PROGRAMFILES\\${COMPANY_NAME}\\${PRODUCT_NAME}\"");

            AddComment("Showing details while (un)installation:");
            Add("ShowInstDetails show");
            Add("ShowUnInstDetails show");
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
            foreach (var s in this.d.GetFiles())
                Add($"File \"{s}\"");
            AddStripline();

            AddComment("Create shortcuts on Desktop and Programs menu.");
            Add($"CreateShortCut \"$DESKTOP\\${{PRODUCT_NAME}}.lnk\" \"$INSTDIR\\${{PRODUCT_EXE_NAME}}\" \"\"");
            Add($"CreateShortCut \"$SMPROGRAMS\\${{PRODUCT_NAME}}.lnk\" \"$INSTDIR\\${{PRODUCT_EXE_NAME}}\" \"\"");
            Add("SectionEnd");
            AddStripline();


            Add("Section -Post");
            Add("WriteUninstaller \"$INSTDIR\\uninst.exe\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"DisplayName\" \"$(^Name)\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"DisplayIcon\" \"$INSTDIR\\${PRODUCT_EXE_NAME}\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"DisplayVersion\" \"${PRODUCT_VERSION}\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"URLInfoAbout\" \"${PRODUCT_WEBSITE}\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"Publisher\" \"${PRODUCT_PUBLISHER}\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"UninstallString\" \"$INSTDIR\\uninst.exe\"");
            Add("WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\" \"QuietUninstallString\" '\"$INSTDIR\\uninst.exe\" /S'");
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
            Add("MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 \"Are you sure you want to remove ${PRODUCT_NAME}?\" IDYES +2");
            Add("Abort");
            Add("FunctionEnd");
            AddStripline();


            Add("Section Uninstall");
            AddComment("Delete shortcuts on Desktop and Programs menu:");
            Add("Delete \"$DESKTOP\\${PRODUCT_NAME}.lnk\"");
            Add("Delete \"$SMPROGRAMS\\${PRODUCT_NAME}.lnk\"");


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
            Add("DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} \"${PRODUCT_UNINST_KEY}\"");
            Add("DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} \"SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\${COMPANY_NAME}\\${PRODUCT_NAME}\"");
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
            => sb.AppendLine(";".PadRight(pPadRight, '*'));

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
