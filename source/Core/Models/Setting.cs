using System.Text;

namespace GeNSIS.Core.Models
{
    public enum EValuType { Boolean, String, Path, Password, Integer, IPAddress, /*List, Options */}

    public class Setting
    {
        public SettingGroup Group { get; set; }
        public string Name { get; set; }
        public object Default { get; set; }
        public EValuType ValueType { get; set; } = EValuType.String;


        public string GetVarDeclString()
        {
            string eventVar = ValueType == EValuType.Boolean ? $"Var On{Group.Name}{Name}_Changed\r\n" : null;
            return $@"; {Name} (default: {Default}).
;Var par_{Group.Name}{Name}
Var tit_{Group.Name}{Name}
Var {GetUIName()}
Var val_{Group.Name}{Name}
{eventVar}";

        }


        public string GetValueDefInitCode()
            => $@"StrCpy $val_{Group.Name}{Name} ""{Default}""";

        public string GetTitleDefInitCode()
            => $@"StrCpy $tit_{Group.Name}{Name} ""{GetNameAsTitle()}""";


        public string GetUIName()
        {
            switch (ValueType)
            {
                case EValuType.Path:
                case EValuType.String:    return $"tbx_{Group.Name}{Name}";
                case EValuType.Integer:   return $"num_{Group.Name}{Name}";
                case EValuType.Boolean:   return $"cbx_{Group.Name}{Name}";
                case EValuType.Password:  return $"pwd_{Group.Name}{Name}";
                case EValuType.IPAddress: return $"ipx_{Group.Name}{Name}";
            }
            return $"???{nameof(GetUIName)}???_{Name}";
        }

        public string GetNameAsTitle() => Name.Replace("_", " ");


        public string GetCreateUIString(int i)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\t; {i}. {Name}");

            string uiType = null;
            string title = $"$tit_{Group.Name}{Name}:";
            string val_ = $"\"$val_{Group.Name}{Name}\"";
            switch (ValueType)
            {
                case EValuType.Path:
                case EValuType.String:    uiType = "Text";     break;
                case EValuType.Integer:   uiType = "Number";   break;
                case EValuType.Boolean:   uiType = "CheckBox"; val_ = null;  break;
                case EValuType.Password:  uiType = "Password"; break;
                case EValuType.IPAddress: uiType = "IPAddress"; val_ = null; break;
            }

            sb.AppendLine($"\t${{NSD_CreateLabel}} 10% {i * 14}u 15% 10u \"{title}\"");
            sb.AppendLine($"\tPop $0");

            sb.AppendLine($"\t${{NSD_Create{uiType}}} 30% {(i*14)-2}u 60% 12u {val_}");
            sb.AppendLine($"\tPop ${GetUIName()}");
            if (ValueType == EValuType.Boolean)
                sb.AppendLine($"\t${{NSD_OnClick}} ${GetUIName()} On{Group.Name}{Name}_Changed");
            else if (ValueType == EValuType.IPAddress)
                sb.AppendLine($"\t${{NSD_SetText}} ${GetUIName()} $val_{Group.Name}{Name}");

            return sb.ToString();
            // sb.AppendLine($"\t");

        }


        public string GetFormLeaveString()
        {
            switch (ValueType)
            {
                case EValuType.Path:
                case EValuType.String:
                case EValuType.Integer:
                case EValuType.Password:
                case EValuType.IPAddress:
                return $"\t${{NSD_GetText}} ${GetUIName()} $val_{Group.Name}{Name}";

                case EValuType.Boolean: 
                    return $"\t${{NSD_GetState}} ${GetUIName()} $0\r\n" +
                        $"\t${{If}} $0 == ${{BST_CHECKED}}\r\n" +
                        $"\t\tStrCpy $val_{Group.Name}{Name} \"true\"\r\n" +
                        $"\t\t; EnableWindow $xxx_OptionalUI 1\r\n" +
                        $"\t${{Else}}\r\n" +
                        $"\t\tStrCpy $val_{Group.Name}{Name} \"false\"\r\n" +
                        $"\t\t; EnableWindow $xxx_OptionalUI 0\r\n" +
                        $"\t${{EndIf}}\r\n";
            }
            return $"???_{Name}";
        }

        public string GetCheckBoxChangedFuncString()
        {
            if (ValueType != EValuType.Boolean)
                return string.Empty ;

            return $@"
Function On{Group.Name}{Name}_Changed
    ${{NSD_GetState}} ${GetUIName()} $0
	${{If}} $0 == ${{BST_CHECKED}}
	    StrCpy $val_{Group.Name}{Name} ""true""
		; EnableWindow $xxx_OptionalUI 1
	${{Else}}
		StrCpy $val_{Group.Name}{Name} ""false""
		; EnableWindow $xxx_OptionalUI 0
	${{EndIf}}
FunctionEnd
";
        }
    }
}
