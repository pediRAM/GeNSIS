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
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Interfaces;
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
        private IAppData m_AppData = null;
        private TextGeneratorOptions m_Options = null;
        private int ln = 0;
        private string m_FileExtension = null;
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

            AddVariableDeclarations();
            AddStripline();
            Add();

            AddCompressionSelection(isOver16Mb);
            Add();

            AddCharacterEncoding();
            Add();

            AddInstallerExecutionAsAdmin();

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
            Add();

            if (HasRelativePath())
            {
                AddComment("THIS VARIABLE DEFINITION IS FOR GeNSIS PROJECTS WHICH ARE EXPORTED AS PORTABLE ONLY!");
                AddComment("Change this if you have moved or renamed the directory, which contains the content files.");
                AddComment("Path to the directory containing the files and folders to install.");
                AddDefine(GConst.Nsis.BASE_DIR, m_AppData.RelativePath);
                Add();
            }

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
            if (!m_AppData.DoInstallPerUser)
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
            else if(HasRelativePath())
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
            }

            AddInsertMacro("MUI_PAGE_DIRECTORY");

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
            AddComment("Function to show the language selection page:");
            AddFunction(".onInit");
            AddInsertMacro("MUI_LANGDLL_DISPLAY");
            AddFunctionEnd();
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

            if (m_AppData.GetFiles().Any(x => x.FSType == Enums.EFileSystemType.Directory))
            {
                AddComment("Add directories recursively (remove /r for non-recursively):");
                foreach (var s in m_AppData.GetFiles().Where(s => s.FSType == Enums.EFileSystemType.Directory))
                    Add($"File /r \"{m_AppData.GetNsisPath(s)}\"");
                AddStripline();
                Add();
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

            
            AddComment("Create shortcuts on Desktop and Programs menu.");
            var endLabel = AddDialogYesNo("Create shortcuts on Desktop and Programs menu?", "CreateShortcuts");
            Add($"CreateShortcut \"$DESKTOP\\${{APP_NAME}}.lnk\"    \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add($"CreateShortcut \"$SMPROGRAMS\\${{APP_NAME}}.lnk\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"\"");
            Add($"{endLabel}:");

            if (m_AppData.GetFirewallRules().Count() > 0)
            {
                AddStripline();
                Add();

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

            var yesLabel = $"LabelYes_{pLabelName}";
            //var noLabel = $"LabelNo_{pLabelName}";
            var endLabel = $"LabelEnd_{pLabelName}";
            AddComment($"Ask user Yes/No question: {pQuestion}");
            Add($"MessageBox MB_YESNO \"{pQuestion}\" IDYES {yesLabel}");
            Add($"Goto {endLabel}");
            AddLabel(yesLabel);
            return endLabel;
        }



        #region Firewall Rules

        #region Add FW Rule
        private void AddAllFirewallRules()
        {
            foreach (IFirewallRule fwr in m_AppData.GetFirewallRules())
            {
                AddComment(GetCommentOpenFirewallRule(fwr));
                AddFirewallRule(fwr);
            }
        }

        private void AddFirewallRule(IFirewallRule fwr)
        {
            switch (fwr.ProtocolType)
            {
                case Enums.EProtocolType.TCP:
                Add(GetCommandAddTcpFirewallRule(fwr));
                break;

                case Enums.EProtocolType.UDP:
                Add(GetCommandAddUdpFirewallRule(fwr));
                break;

                case Enums.EProtocolType.Both:
                Add(GetCommandAddTcpFirewallRule(fwr));
                Add(GetCommandAddUdpFirewallRule(fwr));
                break;
            }
        }

        private string GetCommandAddTcpFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallAddRule("TCP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallAddRule("TCP", fwr.Port);
        }

        private string GetCommandAddUdpFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallAddRule("UDP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallAddRule("UDP", fwr.Port);
        }

        private string GetFirewallAddRule(string pProtocol, int pPortFrom, int pPortTo)
            => $"ExecWait 'netsh advfirewall firewall add rule name=\"Open {pProtocol.ToUpper()} Ports {pPortFrom}-{pPortTo}\" dir=in action=allow protocol={pProtocol.ToUpper()} localport={pPortFrom}-{pPortTo}'";

        private string GetFirewallAddRule(string pProtocol, int pPort)
            => $"ExecWait 'netsh advfirewall firewall add rule name=\"Open {pProtocol.ToUpper()} Port {pPort}\" dir=in action=allow protocol={pProtocol.ToUpper()} localport={pPort}'";

        private string GetCommentOpenFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return $"Open {fwr.ProtocolType.GetDisplayName()} ports from {fwr.Port} to {fwr.ToPort}.";
            else
                return $"Open {fwr.ProtocolType.GetDisplayName()} port {fwr.Port}.";
        }
        #endregion Add FW Rule


        #region Delete FW Rule
        private void AddDeleteAllFirewallRules()
        {
            foreach (IFirewallRule fwr in m_AppData.GetFirewallRules())
            {
                AddComment(GetCommentCloseFirewallRule(fwr));
                AddDeleteFirewallRule(fwr);
            }
        }

        private void AddDeleteFirewallRule(IFirewallRule fwr)
        {
            switch (fwr.ProtocolType)
            {
                case Enums.EProtocolType.TCP:
                Add(GetCommandDeleteTcpFirewallRule(fwr));
                break;

                case Enums.EProtocolType.UDP:
                Add(GetCommandDeleteUdpFirewallRule(fwr));
                break;

                case Enums.EProtocolType.Both:
                Add(GetCommandDeleteTcpFirewallRule(fwr));
                Add(GetCommandDeleteUdpFirewallRule(fwr));
                break;
            }
        }

        private string GetCommandDeleteTcpFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallDeleteRule("TCP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallDeleteRule("TCP", fwr.Port);
        }

        private string GetCommandDeleteUdpFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallDeleteRule("UDP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallDeleteRule("UDP", fwr.Port);
        }

        private string GetFirewallDeleteRule(string pProtocol, int pPortFrom, int pPortTo)
            => $"ExecWait 'netsh advfirewall firewall delete rule name=\"Open {pProtocol.ToUpper()} Ports {pPortFrom}-{pPortTo}\"'";

        private string GetFirewallDeleteRule(string pProtocol, int pPort)
            => $"ExecWait 'netsh advfirewall firewall delete rule name=\"Open {pProtocol.ToUpper()} Port {pPort}\"'";

        private string GetCommentCloseFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return $"Close {fwr.ProtocolType.GetDisplayName()} ports from {fwr.Port} to {fwr.ToPort}.";
            else
                return $"Close {fwr.ProtocolType.GetDisplayName()} port {fwr.Port}.";
        }
        #endregion Delete FW Rule

        #endregion Firewall Rules




        #endregion NSIS Code Creation

        #region Code Line Adding Methods
        private void Add()
        {
            sb.AppendLine();
            ln++;
        }

        private void Add(string s)
        {
            sb.AppendLine($"{Indent}{s}");
            ln++;
        }

        private bool HasSectionStarted { get; set; }
        private string Indent => HasSectionStarted ? "    " : string.Empty;
        private void AddSection(string pParameters)
        {
            Add($"Section {pParameters}");
            HasSectionStarted = true;
        }

        private void AddFunction(string pParameters)
        {
            Add($"Function {pParameters}");
            HasSectionStarted = true;
        }

        private void AddSection()
        {
            Add("Section");
            HasSectionStarted = true;
        }

        private void AddFunction()
        {
            Add("Function");
            HasSectionStarted = true;
        }

        private void AddSectionEnd()
        {
            HasSectionStarted = false;
            Add($"SectionEnd");
        }

        private void AddFunctionEnd()
        {
            HasSectionStarted = false;
            Add($"FunctionEnd");
        }

        private void AddComment(string pCommentLine)
        {
            sb.AppendLine($"{Indent}; {pCommentLine}");
            ln++;
        }

        private void AddLabel(string pName) => Add($"{pName}:");

        private void AddCommentBlock(IEnumerable<string> pCommentLines)
        {
            foreach (var s in pCommentLines)
                AddComment(s);
        }

        private void AddEmptyComment() => AddComment(string.Empty);

        private void AddStripline(int pPadRight = STRIPLINE_LENGTH)
        {
            Add($";".PadRight(pPadRight, '-'));
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
