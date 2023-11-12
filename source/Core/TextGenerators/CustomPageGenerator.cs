using GeNSIS.Core.Enums;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Models;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace GeNSIS.Core.TextGenerators
{
    public class CustomPageGenerator
    {
        private TextGeneratorOptions m_Options;
        private IEnumerable<SettingGroup> m_SettingGroups;

        public void SetSettingGroups(IEnumerable<SettingGroup> settingGroups) => m_SettingGroups = settingGroups;

        /// <summary>
        /// <code>Page custom OnCreateFormXXXXX_Entered OnCreatedFormXXXXX_Leaved</code>
        /// </summary>
        /// <returns></returns>
        public string GetPages()
        {
            var sb = new StringBuilder();

            foreach (var page in m_SettingGroups)
                sb.AppendLine($"Page custom {page.GetCreateFormEnterName()} {page.GetCreateFormLeaveName()}");

            return sb.ToString();
        }

        public string GetVariablesDeclarations()
        {
            var sb = new StringBuilder();

            foreach (var group in m_SettingGroups)
                sb.AppendLine(group.GetVarDeclString());

            return sb.ToString();
        }

        public string GetVariableInitialisationFunctions()
        {
            var sb = new StringBuilder();

            foreach (var group in m_SettingGroups)
                sb.AppendLine(group.GetInitVariablesFunction());

            return sb.ToString();
        }

        public string GetPageCreationFunctions()
        {
            var sb = new StringBuilder();

            foreach (var page in m_SettingGroups)
            {
                sb.AppendLine(page.GetCreateFormFunction());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GetOnInitFunctionLines()
        {
            var sb = new StringBuilder();

            sb.AppendLine($@"ReadRegStr $0 HKLM ""System\CurrentControlSet\Control\ComputerName\ActiveComputerName"" ""ComputerName""
	StrCpy $1 $0 4 3
	StrCpy $TargetHostname $0");

            foreach (var page in m_SettingGroups)
                sb.AppendLine($"\tCall Init{page.Name.UpperCamelCase()}Variables");

            return sb.ToString();
        }


        public IEnumerable<SettingGroup> GetSettingGroups()
        {


            var databaseGroup = new SettingGroup { Name = "Database", Title = "Database", Description = "You can modify the database settings here." };
            var serverGroup   = new SettingGroup { Name = "Server",   Title = "Server",   Description = "You can modify the server settings here." };
            var pathsGroup    = new SettingGroup { Name = "Paths",    Title = "Paths",    Description = "You can modify the paths here." };
            var loggingGroup  = new SettingGroup { Name = "Logging",  Title = "Logging",  Description = "You can modify the logger settings here." };

            // All groups.
            m_SettingGroups = new List<SettingGroup> { databaseGroup, serverGroup, /*pathsGroup, loggingGroup,*/ };

            // Populate DB group.
            databaseGroup.Settings.AddRange(new Setting[]{ 
                new Setting { Group = databaseGroup, Name = "IsExternal", SettingType = ESettingType.Boolean, Default = false },
                new Setting { Group = databaseGroup, Name = "Username", SettingType = ESettingType.String, Default = "admin" },
                new Setting { Group = databaseGroup, Name = "Password", SettingType = ESettingType.Password, Default = "ClearTextPassword!" },
                new Setting { Group = databaseGroup, Name = "Folder", SettingType = ESettingType.Directory, Default = @"C:\Database files" },
                new Setting { Group = databaseGroup, Name = "IP", SettingType = ESettingType.IPAddress, Default = "192.168.0.123" },
                new Setting { Group = databaseGroup, Name = "Port", SettingType = ESettingType.Integer, Default = 1234 },
            });

            // Populate Server group.
            serverGroup.Settings.AddRange(new Setting[]{
                new Setting { Group = serverGroup, Name = "EnableAuthentication", SettingType = ESettingType.Boolean, Default = false },
                new Setting { Group = serverGroup, Name = "Username", SettingType = ESettingType.String, Default = "admin" },
                new Setting { Group = serverGroup, Name = "Password", SettingType = ESettingType.Password, Default = "ClearTextPassword!" },
                new Setting { Group = serverGroup, Name = "IP", SettingType = ESettingType.IPAddress, Default = "10.0.0.1" },
                new Setting { Group = serverGroup, Name = "Port", SettingType = ESettingType.Integer, Default = 56789 },
            });

            return m_SettingGroups;
        }
    }
}
