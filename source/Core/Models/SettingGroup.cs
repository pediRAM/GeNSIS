namespace GeNSIS.Core.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Helpers;
    using GeNSIS.Core.Interfaces;


    public class SettingGroup : ISettingGroup
    {
        const string HR = "; ---------------------------------------------------------------------------------------------------";

        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Setting> Settings { get; set; } = new List<Setting>();

        public bool HasLongUIs => Settings.Any(s => s.SettingType == ESettingType.File || s.SettingType == ESettingType.Directory);
        public IEnumerable<ISetting> GetSettings() => Settings;
        public void SetSettings(IEnumerable<ISetting> pSettings) 
            => Settings = pSettings.Select(x => new Setting(x)).ToList();

        public string GetVarDeclString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(HR);
            sb.AppendLine($"; ********** Custom Page (form) {Name} **********");
            sb.AppendLine($"Var {GetDialogVariableName()}");
            sb.AppendLine($"Var {GetDialogTitleVariableName()}");
            sb.AppendLine($"Var {GetDialogDescriptionVariableName()}");
            sb.AppendLine($"Var {GetGroupBoxTitleVariableName()}");
            sb.AppendLine();

            foreach (var s in Settings)
                sb.AppendLine(s.GetVarDeclString());

            return sb.ToString();
        }

        public string GetInitVariablesFunction()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"; Initialises the variables of form {Name} (custom page) with default values.");
            sb.AppendLine($"Function Init{Name.UpperCamelCase()}Variables");

            sb.AppendLine($"\tStrCpy ${GetDialogTitleVariableName()} \"{Title}\"");
            sb.AppendLine($"\tStrCpy ${GetDialogDescriptionVariableName()} \"{Description}\"\r\n");            
            sb.AppendLine($"\tStrCpy ${GetGroupBoxTitleVariableName()} \"{Title}\"\r\n");

            foreach (var s in Settings)
                sb.AppendLine($"\t{s.GetValueInitialization()}");

            sb.AppendLine();
            foreach (var s in Settings)
                sb.AppendLine($"\t{s.GetTitleDefInitCode()}");

            sb.AppendLine($"FunctionEnd");
            return sb.ToString();
        }

        public string GetDialogVariableName() => $"dlg_{Name.UpperCamelCase()}";
        public string GetDialogTitleVariableName() => $"dlg_{Name.UpperCamelCase()}Title";
        public string GetDialogDescriptionVariableName() => $"dlg_{Name.UpperCamelCase()}Desc";
        public string GetGroupBoxVariableName() => $"grp_{Name.UpperCamelCase()}";
        public string GetGroupBoxTitleVariableName() => $"grp_{Name.UpperCamelCase()}Title";
        public string GetCreateFormEnterName() => $"OnCreateForm{Name.UpperCamelCase()}_Entered";
        public string GetCreateFormLeaveName() => $"OnCreateForm{Name.UpperCamelCase()}_Leaved";

        public string GetCreateFormFunction()
        {
            var sb = new StringBuilder();
            PDC pdc = new PDC(0, HasLongUIs);
            sb.AppendLine($"Function {GetCreateFormEnterName()}");
            sb.AppendLine($"\t!insertmacro MUI_HEADER_TEXT \"{Title}\" \"{Description}\"");
            sb.AppendLine($"\tnsDialogs::Create 1018");
            sb.AppendLine($"\tPop ${GetDialogVariableName()}");

            sb.AppendLine(
                $"\t${{If}} ${GetDialogVariableName()} == error\r\n" +
                $"\t\tAbort\r\n" +
                $"\t${{EndIf}}\r\n");

            sb.AppendLine(
                $"\t${{NSD_CreateGroupBox}} {pdc.GetGroupBoxPosDim(Settings.Count)} \"{Title}\"\r\n" +
                $"\tPop $0");

            pdc.Increment();

            sb.AppendLine();
            
            foreach (var s in Settings)
            {
                sb.AppendLine(s.GetCreateUIString(pdc));
                pdc.Increment();
            }

            sb.AppendLine("\tnsDialogs::Show");
            sb.AppendLine("FunctionEnd");
            sb.AppendLine();
            sb.AppendLine($"Function {GetCreateFormLeaveName()}");
            foreach (var s in Settings.OrderBy(x => x.SettingType))
            {
                sb.AppendLine(s.GetFormLeaveFunction());
            }

            sb.AppendLine("\r\n\r\n\t; ONLY FOR TESTING! REMOVE OR COMMENT FOR USING IN PRODUCTION ENVIRONMENT!");
            sb.Append($"\tMessageBox MB_OK \"Results of form {Name}:$\\n");
            foreach (var s in Settings)
                sb.Append($"{s.Name}: $val_{Name}{s.Name},$\\n");
            sb.AppendLine("\"");

            sb.AppendLine("FunctionEnd");

            foreach (var s in Settings)
            {
                if (s.SettingType == ESettingType.Boolean)
                    sb.AppendLine(s.GetCheckBoxChangedFuncString());
                else if (s.SettingType == ESettingType.File || s.SettingType == ESettingType.Directory)
                    sb.AppendLine(s.GetBrowseButtonClickedFunction());
            }
            return sb.ToString();
        }
    }
}
