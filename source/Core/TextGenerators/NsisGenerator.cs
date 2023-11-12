/*
GeNSIS (GEnerates NullSoft Installer Script)
Copyright (C) 2023 Pedram GANJEH HADIDI

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace GeNSIS.Core.TextGenerators
{
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public partial class NsisGenerator : ITextGenerator
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
        private string m_FileExtension = null;
        private CustomPageGenerator cpg = new CustomPageGenerator();
        #endregion Variables


        #region Methods

        #region Help Methods
        public bool IsCompanyDirEnabled()
            => (m_AppData.DoCreateCompanyDir && string.IsNullOrWhiteSpace(m_AppData.Company));

        public bool HasOptionalSections()
            => (m_AppData.GetSections().Count() > 0);

        public bool HasLicenseFile()
            => (m_AppData.License != null && !string.IsNullOrWhiteSpace(m_AppData.License.Path));

        public bool HasRelativePath()
            => !string.IsNullOrEmpty(m_AppData.RelativePath);
        #endregion Help Methods


        #region NSIS Code Creation

        public string Generate(IAppData pAppData, TextGeneratorOptions pOptions)
        {
            ln = 1;
            sb.Clear();
            m_AppData = pAppData;
            m_Options = pOptions;

            cpg.SetSettingGroups(pOptions.SettingGroups);

            //var totalFilesSizeMainSection = pAppData.GetFiles().Where(x => x.FSType == Enums.EFileSystemType.File).Sum(x => new FileInfo(x.Path).Length);
            //var totalDirsSizeMainSection  = pAppData.GetFiles().Where(x => x.FSType == Enums.EFileSystemType.Directory).Sum(x => new DirectoryInfo(x.Path).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));

            //var totalDirSize = pAppData.GetSections().Sum(x => new DirectoryInfo(x.SourcePath).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));
            bool isOver16Mb = pAppData.GetTotalSize() > ZLIB_SIZE_LIMIT;

            // Does file extension (if any) start with '.'?
            if (!string.IsNullOrWhiteSpace(m_AppData.AssociatedExtension))
            {
                if (m_AppData.AssociatedExtension.StartsWith('.'))
                    m_FileExtension = m_AppData.AssociatedExtension;
                else
                    m_FileExtension = $".{m_AppData.AssociatedExtension}";
            }

            AddCommentHeader();
            Add();

            AddIncludeDirectives();
            AddStripline();
            Add();

            AddConstantsDefinitions();
            AddStripline();
            Add();

            AddRequestExecutionLevel();
            Add();

            AddCompressionSelection(isOver16Mb);
            Add();

            AddCharacterEncoding();
            Add();

            AddInstallerExecutionAsAdmin();

            AddAppName();
            Add();

            AddVariableDeclarations();
            AddStripline();
            Add();

            AddFunctions();
            AddStripline();
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
            //AddStripline();
            //Add();

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

        private void AddRequestExecutionLevel()
        {
            Add("RequestExecutionLevel user");
        }

        private void AddFunctions()
        {
            Add(cpg.GetVariableInitialisationFunctions());
            Add(cpg.GetPageCreationFunctions());
        }

        private void AddVariableDeclarations()
        {
            Add("Var TargetHostname");
            Add();
            if (m_Options.SettingGroups.Any())
                Add(cpg.GetVariablesDeclarations());
        }

        private void AddIncludeDirectives()
        {
            Add(@"; Included libraries:
!include nsDialogs.nsh
!include LogicLib.nsh
!include MUI2.nsh
!include FileFunc.nsh
!insertmacro GetTime
"); // !include EnvVarUpdate.nsh

        }

        private void AddCommentHeader()
        {
            AddStripline();
            AddComment($"Generated by GeNSIS {AsmConst.FULL_VERSION} at {DateTime.Now:yyyy-MM-dd HH:mm}");
            AddComment("For more information visit: https://github.com/pediRAM/GeNSIS");
            AddStripline();
            AddComment($"Copyright (C){DateTime.Now.Year} by Pedram GANJEH-HADIDI");
            AddEmptyComment();
            AddComment($"This script creates installer for: {m_AppData.AppName} {m_AppData.AppVersion}");
            AddStripline();
        }

        private void AddConstantsDefinitions()
        {
            AddComment("Constants:");
            Add();
            Add(@"; Timestamp of installer compilation:
!define /date COMPILED_AT ""%Y-%m-%d %H:%M:%S""");
            if (HasRelativePath())
            {
                AddComment("THIS VARIABLE DEFINITION IS FOR GeNSIS PROJECTS WHICH IS EXPORTED AS PORTABLE ONLY!");
                AddComment("Change this if you have moved or renamed the directory, which contains the content files.");
                AddComment("Path to the directory containing the files and folders to install.");
                AddDefine(GConst.Nsis.BASE_DIR, m_AppData.RelativePath);
                Add();
            }

            //Add(@"EnvVarUpdate.nsh");

            AddComment("Name of Application:");
            AddDefine("APP_NAME", m_AppData.AppName);
            Add();

            AddComment("Filename of Application EXE file (*.exe):");
            AddDefine("APP_EXE_NAME", m_AppData.ExeName.Name);
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

            if (m_FileExtension != null)
            {
                AddComment("File extension associated to Application:");
                AddDefine("FILE_EXTENSION", m_FileExtension);
                Add();
            }

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
            if (m_AppData.InstallationTarget == Enums.EInstallTargetType.AllUsers)
            {
                Add("RequestExecutionLevel admin");
                Add();
            }
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
            //  - Custom
            switch (m_AppData.InstallationTarget)
            {
                case EInstallTargetType.Custom:
                case EInstallTargetType.PerUser: AddDefine("UNINST_ROOT_KEY", "HKCU"); break;

                case EInstallTargetType.AllUsers: AddDefine("UNINST_ROOT_KEY", "HKLM"); break;
            }
        }

        private void AddModernGuiWithIconAndImages()
        {
            // Modern GUI
            //AddComment("Using modern user interface for installer:");
            //Add("!include \"MUI.nsh\"");
            //Add();

            Add("BrandingText \"${COMPILED_AT}\"");
            Add();

            // Installer Icon
            AddComment("Installer icons (*.ico):");
            if (string.IsNullOrWhiteSpace(m_AppData.InstallerIcon))
                AddDefine("MUI_ICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-install.ico");
            else if (HasRelativePath())
                AddDefine("MUI_ICON", $"${{{GConst.Nsis.BASE_DIR}}}\\{m_AppData.InstallerIcon}");
            else
                AddDefine("MUI_ICON", m_AppData.InstallerIcon);
            Add();

            // Uninstaller Icon
            AddComment("Uninstaller icon (*.ico):");
            if (string.IsNullOrWhiteSpace(m_AppData.UninstallerIcon))
                AddDefine("MUI_UNICON", "${NSISDIR}\\Contrib\\Graphics\\Icons\\modern-uninstall.ico");
            else if (HasRelativePath())
                AddDefine("MUI_UNICON", $"${{{GConst.Nsis.BASE_DIR}}}\\{m_AppData.UninstallerIcon}");
            else
                AddDefine("MUI_UNICON", m_AppData.UninstallerIcon);

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
                if (HasRelativePath())
                    AddDefine("MUI_HEADERIMAGE_BITMAP", $"${{{GConst.Nsis.BASE_DIR}}}\\{m_AppData.InstallerHeaderImage}");
                else
                    AddDefine("MUI_HEADERIMAGE_BITMAP", m_AppData.InstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_BITMAP_NOSTRETCH");
                Add();
            }

            // Uninstaller Header Image
            if (!string.IsNullOrWhiteSpace(m_AppData.UninstallerHeaderImage))
            {
                AddComment("Uninstaller Header Image:");
                if (HasRelativePath())
                    AddDefine("MUI_HEADERIMAGE_UNBITMAP", $"${{{GConst.Nsis.BASE_DIR}}}\\{m_AppData.UninstallerHeaderImage}");
                else
                    AddDefine("MUI_HEADERIMAGE_UNBITMAP", m_AppData.UninstallerHeaderImage);
                AddDefine("MUI_HEADERIMAGE_UNBITMAP_NOSTRETCH");
                Add();
            }

            // Installer Wizard Image (left side)
            if (!string.IsNullOrWhiteSpace(m_AppData.InstallerWizardImage))
            {
                AddComment("Installer Wizard Image:");
                if (HasRelativePath())
                    AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP", $"${{{GConst.Nsis.BASE_DIR}}}\\{m_AppData.InstallerWizardImage}");
                else
                    AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP", m_AppData.InstallerWizardImage);
                AddDefine("MUI_WELCOMEFINISHPAGE_BITMAP_NOSTRETCH");
                Add();
            }

            // Uninstaller Wizard Image (left side)
            if (!string.IsNullOrWhiteSpace(m_AppData.UninstallerWizardImage))
            {
                AddComment("Uninstaller Wizard Image:");
                if (HasRelativePath())
                    AddDefine("MUI_UNWELCOMEFINISHPAGE_BITMAP", $"${{{GConst.Nsis.BASE_DIR}}}\\{m_AppData.InstallerWizardImage}");
                else
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

            if (HasLicenseFile())
            {
                AddComment("License file (*.txt|*.rtf):");
                AddInsertMacro("MUI_PAGE_LICENSE", m_AppData.GetNsisPath(m_AppData.License));
                Add("LicenseForceSelection checkbox \"I accept the terms of the license.\"");
            }

            AddInsertMacro("MUI_PAGE_DIRECTORY");

            if (m_Options.SettingGroups.Any())
                Add(cpg.GetPages());

            if (HasOptionalSections())
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
            AddComment("!include \"German.nsh\"");
            AddComment("!include \"English.nsh\"");
            AddComment("!include \"French.nsh\"");
            Add();
            AddComment("Function to show the language selection page:");
            AddFunction(".onInit");
            AddInsertMacro("MUI_LANGDLL_DISPLAY");
            Add(cpg.GetOnInitFunctionLines());
            AddFunctionEnd();
        }

        private void AddInstallationDir()
        {
            AddComment("Installation folder (Programs\\Company\\Application):");
            if (m_AppData.InstallationTarget == EInstallTargetType.PerUser)
            {
                if (m_AppData.DoCreateCompanyDir && !string.IsNullOrWhiteSpace(m_AppData.Company))
                    Add("InstallDir \"$LocalAppData\\Programs\\${COMPANY_NAME}\\${APP_NAME}\"");
                else
                    Add("InstallDir \"$LocalAppData\\Programs\\${APP_NAME}\"");
            }
            else if (m_AppData.InstallationTarget == EInstallTargetType.AllUsers)
            {
                if (m_AppData.DoCreateCompanyDir && !string.IsNullOrWhiteSpace(m_AppData.Company))
                    Add("InstallDir \"$ProgramFiles\\${COMPANY_NAME}\\${APP_NAME}\"");
                else
                    Add("InstallDir \"$ProgramFiles\\${APP_NAME}\"");
            }
            else // EInstallTargetType.Custom:
            {
                Add($"InstallDir \"{m_AppData.CustomInstallDir}\"");
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
            AddComment("Main Section (first component/section), which is mandatory. This means:");
            AddComment("user cannot unselect this component/section (if there are two or more).");
            if (HasOptionalSections())
            {
                AddSection("!Required");
                Add("SectionIn RO");
            }
            else
            {
                AddSection($"\"Required\" SEC01");
            }

            Add("SetOutPath \"$INSTDIR\"");
            Add("SetOverwrite ifnewer");
            Add();

            AddStopService();
            Add();

            if (m_AppData.GetFiles().Any(x => x.FSType == Enums.EFileSystemType.Directory))
            {
                var emptyDirs = new List<string>();
                AddCommentBlock(
                "Add directories recursively (or remove '/r' for non-recursively).",
                "ATTENTION:",
                "   Copying empty directories causes error during compilation!",
                "   Replace 'File /r' for empty directories, with 'CreateDirectory '$INSTDIR\\DirectoryName' to",
                "   create the empty directories during installation and avoid compiler error!");

                foreach (var s in m_AppData.GetFiles().Where(s => s.FSType == Enums.EFileSystemType.Directory))
                {
                    string fullPath = m_AppData.GetFullPath(s);
                    if (!Directory.EnumerateFileSystemEntries(fullPath).Any())
                        emptyDirs.Add(fullPath);
                    else
                        Add($"File /r \"{m_AppData.GetNsisPath(s)}\"");
                }
                AddStripline();
                Add();

                if (emptyDirs.Any())
                {
                    AddComment("Creating empty directories:");
                    foreach (var dir in emptyDirs)
                        Add($"CreateDirectory \"$INSTDIR\\{Path.GetFileName(dir)}\"");
                    AddStripline();
                    Add();
                }
            }

            AddComment("Add files:");
            foreach (var s in m_AppData.GetFiles().Where(s => s.FSType == Enums.EFileSystemType.File))
            {
                Add($"File \"{m_AppData.GetNsisPath(s)}\"");
            }
            AddStripline();
            Add();

            if (m_FileExtension != null)
            {
                AddComment("Create association of file extension to application:");
                Add(@"WriteRegStr HKCR ""${FILE_EXTENSION}""              """" ""${APP_NAME}""");
                Add(@"WriteRegStr HKCR ""${APP_NAME}""                    """" ""${APP_NAME} File""");
                Add(@"WriteRegStr HKCR ""${APP_NAME}\DefaultIcon""        """" ""$INSTDIR\${APP_EXE_NAME},0""");
                Add(@"WriteRegStr HKCR ""${APP_NAME}\Shell\Open\Command"" """" ""$\""$INSTDIR\${APP_EXE_NAME}$\"" $\""%1$\""""");
                AddStripline();
                Add();
            }

            // Add Shortcuts only for desktop application (never for services)!
            if (m_AppData.IsService)
            {
                AddServiceInstall();
                AddStripline();
                Add();
            }
            else
            {
                AddComment("Create shortcut in Program menu.");
                Add($"CreateShortcut \"$SMPROGRAMS\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");

                AddComment("Asking user whether to create shortcut on desktop or not:");
                var endLabel = AddDialogYesNo("Create shortcuts on Desktop?", "CreateShortcut");
                Add($"CreateShortcut \"$DESKTOP\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
                Add($"{endLabel}:");
                AddStripline();
                Add();
            }



            if (m_AppData.GetFirewallRules().Count() > 0)
            {
                AddComment("Add firewall rules.");
                var label = AddDialogYesNo("Add firewall rules?", "AddFWRules");
                AddAllFirewallRules();
                AddLabel(label);
            }
            else
                Add();

            AddSectionEnd();
        }

        private int m_SectionCounter = 1;
        private void AddSections()
        {
            m_SectionCounter = 1;
            foreach (var dir in m_AppData.GetSections())
            {
                AddSection($"\"{dir.Name}\" SEC{m_SectionCounter++:d2}");
                if (string.IsNullOrEmpty(dir.TargetInstallDir))
                    Add($"SetOutPath \"$INSTDIR\"");
                else
                    Add($"SetOutPath \"$INSTDIR\\{dir.TargetInstallDir}\"");
                Add("SetOverwrite ifnewer");
                Add();

                AddComment("Add files:");
                foreach (var s in Directory.GetFiles(dir.SourcePath, "*", SearchOption.AllDirectories))
                    Add($"File \"{s}\"");
                AddSectionEnd();
                AddStripline();
                Add();
            }
        }

        private void AddShortCutSection()
        {
            AddSection($"\"Create Shortcuts\" SEC{m_SectionCounter:d2}");
            AddComment("Create shortcuts on Desktop and Programs menu.");
            Add($"CreateShortcut \"$DESKTOP\\${{APP_NAME}}.lnk\"    \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add($"CreateShortcut \"$SMPROGRAMS\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            AddSectionEnd();
        }

        private void AddPostSection()
        {
            AddSection("-Post");
            if (m_AppData.IsService)
            {
                AddServiceUninstall();
                AddStripline();
                Add();
            }

            Add("WriteUninstaller \"$INSTDIR\\uninst.exe\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"DisplayName\"          \"$(^Name)\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"DisplayIcon\"          \"$INSTDIR\\${APP_EXE_NAME}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"DisplayVersion\"       \"${APP_VERSION}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"URLInfoAbout\"         \"${APP_URL}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"Publisher\"            \"${APP_PUBLISHER}\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"UninstallString\"      \"$INSTDIR\\uninst.exe\"");
            Add("WriteRegStr ${UNINST_ROOT_KEY} \"${UNINST_KEY}\" \"QuietUninstallString\" '\"$INSTDIR\\uninst.exe\" /S'");
            AddSectionEnd();
        }

        private void AddUninstallMessage()
        {
            AddComment("After application is sucessfully uninstalled:");
            AddFunction("un.onUninstSuccess");
            Add("HideWindow");
            AddComment("You can change the message to suite your needs:");
            Add("MessageBox MB_ICONINFORMATION|MB_OK \"Application successfully removed.\"");
            AddFunctionEnd();
        }

        private void AddAskUserBeforeUninstall()
        {
            AddComment("Before starting to uninstall:");
            AddFunction("un.onInit");
            AddComment("You can change the message to suite your needs:");
            Add("MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 \"Are you sure you want to remove ${APP_NAME}?\" IDYES +2");
            Add("Abort");
            AddFunctionEnd();
        }

        private void AddUninstallSection()
        {
            AddSection("Uninstall");

            // Services have no shortcuts!
            if (!m_AppData.IsService)
            {
                AddComment("Delete shortcuts on Desktop and Programs menu:");
                Add("Delete \"$DESKTOP\\${APP_NAME}.lnk\"");
                Add("Delete \"$SMPROGRAMS\\${APP_NAME}.lnk\"");
            }

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

            if (m_FileExtension != null)
                Add(@"DeleteRegKey ${UNINST_ROOT_KEY} ""${FILE_EXTENSION}""");

            if (m_AppData.DoCreateCompanyDir && !string.IsNullOrWhiteSpace(m_AppData.Company))
                Add("DeleteRegKey ${UNINST_ROOT_KEY} \"SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\${COMPANY_NAME}\\${APP_NAME}\"");
            else
                Add("DeleteRegKey ${UNINST_ROOT_KEY} \"SOFTWARE\\Microsoft\\.NETFramework\\v2.0.50727\\AssemblyFoldersEx\\${APP_NAME}\"");

            Add(@"DeleteRegKey ${UNINST_ROOT_KEY} ""${APP_NAME}""");

            // Any firewall rules?
            if (m_AppData.GetFirewallRules().Count() > 0)
            {
                AddStripline();
                Add();

                AddComment("Remove firewall rules (closing ports).");
                var label = AddDialogYesNo("Remove firewall rules?", "RemoveFWRules");
                AddDeleteAllFirewallRules();
                AddLabel(label);
            }
            else
                Add();

            Add("SetAutoClose true");
            AddSectionEnd();
        }

        /// <summary>
        /// Adds the script code for showing a Yes-No-MessageBox to the user and returns the endLabel.
        /// </summary>
        /// <param name="pQuestion">The Question.</param>
        /// <param name="pLabelName">Simple name of label (without "Label"), like: "addDesktopShortcut", "addShortcutToStartMenu", "addFirewallRules" aso.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string AddDialogYesNo(string pQuestion, string pLabelName)
        {
            if (string.IsNullOrWhiteSpace(pLabelName))
                throw new ArgumentNullException(nameof(pLabelName), $"Name of a Label cannot be NULL/empty/whitespace!");

            AddComment($"Ask user Yes-No question: {pQuestion}");
            Add($"MessageBox MB_YESNO \"{pQuestion}\" IDYES {GetLabelYes(pLabelName)}");
            Add($"Goto {GetLabelEnd(pLabelName)}");
            AddLabel(GetLabelYes(pLabelName));
            return GetLabelEnd(pLabelName);
        }

        private string GetLabelYes(string pName) => $"L_{pName}_Yes";
        private string GetLabelNo(string pName) => $"L_{pName}_No";
        private string GetLabelEnd(string pName) => $"L_{pName}_End";


        /// <summary>
        /// Add installation directory to the %PATH% variable of user.
        /// </summary>
        private void AddInstallDirToUserEnvVarPATH()
        {
            AddSection(true, "Add PATH");
            AddComment("Adding intallation directory to environment variable of user %PATH%:");
            Add("ReadEnvStr $0 PATH");
            Add("${StrContains} $1 $0 $INSTDIR");
            Add("${If} $1 == \"\"");
            Add("; Path does not contain $INSTDIR, so append it");
            Add("${EnvVarUpdate} $0 \"PATH\" \"A\" \"S\" \"$INSTDIR\"");
            Add("${EndIf}");
            AddSectionEnd();

        }

        private void RemoveDirFromUserEnvVarPATH()
        {
            Add(@"
Section

    ; Read the current user PATH
    ReadEnvStr $0 PATH

    ; Check if the InstallDir is in PATH
    ${StrStr} $1 $0 ""$InstallDir""
    ${If} $1 != """"
        ; Remove the InstallDir from the PATH
        ${EnvVarUpdate} $0 ""PATH"" ""R"" ""U"" ""$InstallDir""
        
        ; Update the user's PATH
        System::Call 'Kernel32::SetEnvironmentVariableA(t, t) i(""PATH"", r0)'
    ${EndIf}

SectionEnd
");
        }

        private void AddInstallDirToSys64EnvVarPath()
        {
            AddSection(true, "Add PATH");
            Add("SetRegView 64  ; Use 64-bit registry view on 64-bit systems.");
            Add(@"ReadRegStr $0 HKLM ""SYSTEM\CurrentControlSet\Control\Session Manager\Environment"" ""Path""");
            AddComment("Check if $INSTDIR is already in the PATH.");
            Add("${StrStr} $1 $0 \"$INSTDIR\"");
            Add("${If} $1 == \"\"");
            AddComment("	$INSTDIR is not in the PATH => add it!");
            Add("	StrCpy $0 \"$0;$INSTDIR\"");
            AddComment("	Write the updated PATH back to the registry.");
            Add(@"	WriteRegExpandStr HKLM ""SYSTEM\CurrentControlSet\Control\Session Manager\Environment"" ""Path"" $0");
            AddComment("	Notify the system about the environment variable change.");
            Add(@"	SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 ""STR: Environment"" /TIMEOUT=5000");
            Add("${EndIf}");
            AddSectionEnd();

        }

        private void RemoveDirFromSys64EnvVarPATH()
        {
            Add(@"
Section

    ; Set the registry view to 64-bit
    SetRegView 64

    ; Read the current system PATH
    ReadRegStr $0 HKLM ""SYSTEM\CurrentControlSet\Control\Session Manager\Environment"" ""PATH""

    ; Check if the InstallDir is in PATH
    ${StrStr} $1 $0 ""$InstallDir""
    ${If} $1 != """"
        ; Remove the InstallDir from the PATH
        ${EnvVarUpdate} $0 ""PATH"" ""R"" ""A"" ""$InstallDir""
        
        ; Update the system's PATH in the registry
        WriteRegStr HKLM ""SYSTEM\CurrentControlSet\Control\Session Manager\Environment"" ""PATH"" ""$0""
        
        ; Notify the system about the environment variable change
        SendMessage ${HWND_BROADCAST} ${WM_WININICHANGE} 0 ""STR:Environment"" /TIMEOUT=5000
    ${EndIf}

SectionEnd
");
        }
        #endregion NSIS Code Creation

        #endregion Methods
    }
}
