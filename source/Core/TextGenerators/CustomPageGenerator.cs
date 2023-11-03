using GeNSIS.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeNSIS.Core.TextGenerators
{
    public class CustomPageGenerator
    {
        private TextGeneratorOptions m_Options;
        public string Generate(TextGeneratorOptions pOptions)
        {
            m_Options = pOptions;

            var databaseGroup = new SettingGroup { Name = "Database", Title = "Database", Description = "You can modify the database settings here." };
            var serverGroup   = new SettingGroup { Name = "Server",   Title = "Server",   Description = "You can modify the server settings here." };
            var pathsGroup    = new SettingGroup { Name = "Paths",    Title = "Paths",    Description = "You can modify the paths here." };
            var loggingGroup  = new SettingGroup { Name = "Logging",  Title = "Logging",  Description = "You can modify the logger settings here." };

            // All groups.
            var settingGroups = new List<SettingGroup> { databaseGroup, serverGroup, /*pathsGroup, loggingGroup,*/ };

            // Populate DB group.
            databaseGroup.Settings.AddRange(new Setting[]{ 
                new Setting { Group = databaseGroup, Name = "IsExternal", ValueType = EValuType.Boolean, Default = false },
                new Setting { Group = databaseGroup, Name = "Username", ValueType = EValuType.String, Default = "admin" },
                new Setting { Group = databaseGroup, Name = "Password", ValueType = EValuType.Password, Default = "ClearTextPassword!" },
                new Setting { Group = databaseGroup, Name = "Folder", ValueType = EValuType.Path, Default = @"C:\Database files" },
                new Setting { Group = databaseGroup, Name = "IP", ValueType = EValuType.IPAddress, Default = "192.168.0.123" },
                new Setting { Group = databaseGroup, Name = "Port", ValueType = EValuType.Integer, Default = 1234 },
            });

            // Populate Server group.
            serverGroup.Settings.AddRange(new Setting[]{
                new Setting { Group = serverGroup, Name = "EnableAuthentication", ValueType = EValuType.Boolean, Default = false },
                new Setting { Group = serverGroup, Name = "Username", ValueType = EValuType.String, Default = "admin" },
                new Setting { Group = serverGroup, Name = "Password", ValueType = EValuType.Password, Default = "ClearTextPassword!" },
                new Setting { Group = serverGroup, Name = "IP", ValueType = EValuType.IPAddress, Default = "10.0.0.1" },
                new Setting { Group = serverGroup, Name = "Port", ValueType = EValuType.Integer, Default = 56789 },
            });

            var sb = new StringBuilder();
            sb.AppendLine(GetIncludes());
            sb.AppendLine("Var TargetHostname");

            foreach (var group in settingGroups)
            {
                sb.AppendLine(group.GetVarDeclString());
                sb.AppendLine();
            }

            foreach (var page in settingGroups)
            {
                sb.AppendLine($"Page custom OnCreateForm_{page.Name}_Enter OnCreateForm_{page.Name}_Leave");
            }

            sb.AppendLine(@"
; Show welcome page:
;!insertmacro MUI_PAGE_WELCOME
; License file (*.txt|*.rtf):
;!insertmacro MUI_PAGE_LICENSE ""C:\Users\pedra\source\repos\MyApp\Source\bin\Debug\Test For GeNSIS\License.txt""
;!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_INSTFILES
;-------------------------------------------------------------------------------

; Available languages (first one is the default):
");
            foreach (var lang in m_Options.Languages)
                sb.AppendLine($"!insertmacro MUI_LANGUAGE \"{lang.Name}\"");

            foreach (var page in settingGroups)
            {
                sb.AppendLine(page.GetCreateFormString());
                sb.AppendLine();
            }

            sb.AppendLine("Function .onInit");
            sb.AppendLine($@"	!insertmacro MUI_LANGDLL_DISPLAY
	ReadRegStr $0 HKLM ""System\CurrentControlSet\Control\ComputerName\ActiveComputerName"" ""ComputerName""
	StrCpy $1 $0 4 3
	StrCpy $TargetHostname $0");
            foreach (var page in settingGroups)
            {
                sb.AppendLine(page.ToOnInit());
            }
            sb.AppendLine("FunctionEnd");

            sb.AppendLine();

            

            sb.AppendLine();
            sb.AppendLine("Section");
            sb.AppendLine("\tDetailPrint \"Mission accomplished!\"");
            sb.AppendLine("SectionEnd");

            return sb.ToString();
        }

        public string GetIncludes()
        {
            return @"!include nsDialogs.nsh
!include LogicLib.nsh
!include MUI2.nsh

; Name of Application:
!define APP_NAME ""MyApp""

; Filename of Application EXE file (*.exe):
!define APP_EXE_NAME ""MyApp.exe""

; Version of Application:
!define APP_VERSION ""1.2.3""

; Build of Application:
!define APP_BUILD ""build""

; Architecture of Application:
!define APP_ARCH ""x64""

; Machine type of Application:
!define APP_MACHINE_TYPE ""AMD64""

; Application Publisher (company, organisation, author):
!define APP_PUBLISHER ""ACME Ltd.""

; Name or initials of the company, organisation or author:
!define COMPANY_NAME ""ACME-Ltd.""

; URL of the Application Website starting with 'https://' :
!define APP_URL ""https://pgh.geekgalaxy.com/""

; Name of setup/installer EXE file (*.exe):
!define SETUP_EXE_NAME ""Setup_${APP_NAME}_${APP_VERSION}_${APP_BUILD}_${APP_MACHINE_TYPE}_${APP_ARCH}.exe""
;-------------------------------------------------------------------------------

SetCompressor zlib
Unicode true
Name ""Testing Formulars""
OutFile Setup_TestingFormulars.exe
RequestExecutionLevel user
ShowInstDetails show

";
        }
    }
}
