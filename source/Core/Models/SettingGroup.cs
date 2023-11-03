using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeNSIS.Core.Models
{
    public class SettingGroup
    {
        const string HR = "; ---------------------------------------------------------------------------------------------------";

        public string Name        { get; set; }
        public string Title       { get; set; }
        public string Description { get; set; }
        public List<Setting> Settings { get; set; } = new List<Setting>();

        public string GetVarDeclString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(HR);
            sb.AppendLine($"; Page: {Name}");
            sb.AppendLine($"Var dlg_{GetUIName()}");

            foreach (var s in Settings)
                sb.AppendLine(s.GetVarDeclString());

            return sb.ToString();
        }


        public string ToOnInit()
        {
            var sb = new StringBuilder();
            sb.AppendLine(HR);
            sb.AppendLine($"\t; Page {Name}:");
            foreach (var s in Settings)
                sb.AppendLine($"\t{s.GetValueDefInitCode()}");

            foreach (var s in Settings)
                sb.AppendLine($"\t{s.GetTitleDefInitCode()}");

            foreach (var s in Settings.Where(x => x.ValueType == EValuType.Boolean))
                sb.AppendLine($"\t${{NSD_OnClick}} ${s.GetUIName()} On{Name}{s.Name}_Changed");

            return sb.ToString();
        }

        public string GetUIName() => Name.Replace(" ", "_").Replace("-", "_");

        public string GetCreateFormString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Function OnCreateForm_{GetUIName()}_Enter");
            sb.AppendLine($"\t!insertmacro MUI_HEADER_TEXT \"{Title}\" \"{Description}\"");
            sb.AppendLine($"\tnsDialogs::Create 1018");
            sb.AppendLine($"\tPop $dlg_{GetUIName()}");
            sb.AppendLine($"\t${{If}} $dlg_{GetUIName()} == error\r\n\t\tAbort\r\n\t${{EndIf}}\r\n");
            sb.AppendLine($"\t${{NSD_CreateGroupBox}} 5% 0u 90% {(Settings.Count + 1) * 14 + 2}u \"{Title}\"\r\n\tPop $0");
            sb.AppendLine();
            int i = 1;
            foreach (var s in Settings)
                sb.AppendLine(s.GetCreateUIString(i++));

            sb.AppendLine("\tnsDialogs::Show");
            sb.AppendLine("FunctionEnd");
            sb.AppendLine();
            sb.AppendLine($"Function OnCreateForm_{GetUIName()}_Leave");
            foreach(var s in Settings.OrderBy(x => x.ValueType))
            {
                sb.AppendLine(s.GetFormLeaveString());
            }

            sb.AppendLine("\r\n\r\n\t; ONLY FOR TESTING! REMOVE OR COMMENT FOR USING IN PRODUCTION ENVIRONMENT!");
            sb.Append($"\tMessageBox MB_OK \"Results of form {Name}:$\\n");
            foreach (var s in Settings)
                sb.Append($"{s.Name}: $val_{Name}{s.Name},$\\n");
            sb.AppendLine("\"");

            sb.AppendLine("FunctionEnd");

            foreach(var s in Settings.Where(x => x.ValueType == EValuType.Boolean))
            {
                sb.AppendLine(s.GetCheckBoxChangedFuncString());
            }
            return sb.ToString();
        }
    }
}
