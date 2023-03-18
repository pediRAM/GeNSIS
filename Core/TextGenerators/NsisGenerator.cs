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
            d = data;
            o = opt;

            var totalSize = data.GetFiles().Sum(x => new FileInfo(x).Length);
            var totalDirSize = data.GetDirectories().Sum(x => new DirectoryInfo(x).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));
            bool isOver16Mb = (totalDirSize + totalDirSize) > (ZLIB_SIZE_LIMIT);
            AddCommentHeader();

            AddComment("Variables:");
            AddComment("This is the DISPLAYNAME of your application (remove spaces!):");
            AddDefine("PRODUCT_NAME", d.AppName);
            AddComment("This is the FILENAME of the executable/binary (*.exe) of your application:");
            AddDefine("PRODUCT_EXE_NAME", Path.GetFileName(d.ExeName));
            AddComment("Name of setup/installer file (*.exe):");
            AddDefine("SETUP_EXE_NAME", d.InstallerFileName);
            AddDefine("PRODUCT_VERSION", d.AppVersion);
            AddDefine("PRODUCT_PUBLISHER", d.Publisher);
            AddDefine("COMPANY_NAME", d.Company);
            AddDefine("PRODUCT_WEBSITE", d.Url);
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
            Add("!define MUI_ABORTWARNING");
            AddStripline();

            AddComment("Application icon (*.ico):");
            AddDefine("MUI_ICON", d.AppIcon);
            AddDefine("MUI_UNICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-uninstall.ico");
            AddStripline();

            AddComment("Show welcome page:");
            AddInsertMacro("MUI_PAGE_WELCOME");
            AddStripline();

            if (d.License != null)
            {
                AddComment("License file (*.txt):");
                AddInsertMacro("MUI_PAGE_LICENSE", Path.GetFileName(d.License));
                AddStripline();
            }

            AddInsertMacro("MUI_PAGE_DIRECTORY");
            AddInsertMacro("MUI_PAGE_INSTFILES");
            AddInsertMacro("MUI_PAGE_FINISH");
            AddInsertMacro("MUI_UNPAGE_INSTFILES");
            AddStripline();

            AddComment("In installer used language:");
            AddInsertMacro("MUI_LANGUAGE", "English");
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

            if (d.GetDirectories().Any())
            {
                AddComment("Add directories recursively (remove /r for non-recursively):");
                foreach (var s in d.GetDirectories())
                    Add($"File /r \"{s}\"");
                AddStripline();
            }

            AddComment("Add files:");
            foreach (var s in d.GetFiles())
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


            if (d.GetDirectories().Any())
            {
                AddComment("Deleteing Subfolders:");
                foreach (var s in d.GetDirectories())
                    Add($"RMDir \"$INSTDIR\\{Path.GetDirectoryName(s)}");
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

        private void AddLog(string pValue)
            => sb.AppendLine($"!echo \"{pValue}\"");

        private void AddInsertMacro(string pMacro, string pValue)
            => sb.AppendLine($"!insertmacro {pMacro} \"{pValue}\"");

        private void AddInsertMacro(string pMacro)
            => sb.AppendLine($"!insertmacro {pMacro}");
        #endregion Methods
    }
}
