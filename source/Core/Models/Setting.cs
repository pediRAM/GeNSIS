using GeNSIS.Core.Enums;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Helpers;
using GeNSIS.Core.Interfaces;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace GeNSIS.Core.Models
{


    public class Setting : ISetting
    {
        public Setting() { }

        public Setting(ISetting pSetting) : this()
            => UpdateValues(pSetting);

        /// <summary>
        /// The group to which this setting belongs.
        /// </summary>
        public ISettingGroup Group { get; set; }

        /// <summary>
        /// Internal (semantic) name of setting, like: "Firstname", "City", "IP",...
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// External (displayname) name of setting, which is shown at left, like: "Firstname:", "Port:", "Enabled:",...
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Default value.
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// Type of setting.
        /// </summary>
        public ESettingType SettingType { get; set; } = ESettingType.String;

        public void UpdateValues(ISetting pSetting)
        {
            Default = pSetting.Default;
            Group = pSetting.Group;
            Name = pSetting.Name;
            SettingType = pSetting.SettingType;
        }

        public string GetUniqueName() => $"{Group.Name.UpperCamelCase()}{Name.UpperCamelCase()}";
        public string GetTitleVariableName() => $"tit_{GetUniqueName()}";
        public string GetValueVariableName() => $"val_{GetUniqueName()}";

        public string GetEventHanderName() => GetEventHanderName(SettingType, GetUniqueName());

        public string GetEventHanderName(ESettingType pSettingType, string pUniqueName)
        {
            switch (pSettingType)
            {
                case ESettingType.Boolean:      return $"On{pUniqueName}_CheckChanged";
                case ESettingType.ButtonBrowse: return $"On{pUniqueName}_Clicked";
                /*
                case ESettingType.File:         return $"fil_{GetUniqueName()}";
                case ESettingType.Directory:    return $"dir_{GetUniqueName()}";
                case ESettingType.String:       return $"tbx_{GetUniqueName()}";
                case ESettingType.Integer:      return $"num_{GetUniqueName()}";
                
                case ESettingType.Password:     return $"pwd_{GetUniqueName()}";
                case ESettingType.IPAddress:    return $"ipx_{GetUniqueName()}";
                */
            }
            return $"???{nameof(GetUIVariableName)}???_{Name}";
        }

        public string GetVarDeclString()
        {
            // todo: remove string eventVar = SettingType == ESettingType.Boolean ? $"Var On{GetUniqueName()}_Changed\r\n" : null;
            return $@"; {Name} (default: {Default}).
;Var par_{GetUniqueName()}
Var {GetTitleVariableName()}
Var {GetUIVariableName()}
Var {GetValueVariableName()}
";
        }


        public string GetValueInitialization()
        {
            if (SettingType == ESettingType.Boolean)
            {
                int value = Default == null ? 0 : ((bool)Default).To01();
                return $@"IntOp  ${GetValueVariableName()} {value} + 0";
            }
            else
                return $@"StrCpy ${GetValueVariableName()} ""{Default}""";
        }

        public string GetTitleDefInitCode()
            => $@"StrCpy ${GetTitleVariableName()} ""{Name}""";


        public string GetUIVariableName()
        {
            switch (SettingType)
            {
                case ESettingType.File:         return $"fil_{GetUniqueName()}";
                case ESettingType.Directory:    return $"dir_{GetUniqueName()}";
                case ESettingType.String:       return $"tbx_{GetUniqueName()}";
                case ESettingType.Integer:      return $"num_{GetUniqueName()}";
                case ESettingType.Boolean:      return $"cbx_{GetUniqueName()}";
                case ESettingType.Password:     return $"pwd_{GetUniqueName()}";
                case ESettingType.IPAddress:    return $"ipx_{GetUniqueName()}";
            }
            return $"; !!! NO CASE FOR SETTING TYPE '{SettingType}' IN METHOD {nameof(GetUIVariableName)} IMPLEMENTED !!!";
        }

        public string GetCreateUIString(PDC pPDC)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\t; {pPDC.Counter}. {Name}");

            string uiType = null;
            string title = $"${GetTitleVariableName()}";
            string val_ = $"\"${GetValueVariableName()}\"";

            switch (SettingType)
            {
                case ESettingType.File:      uiType = "FileRequest"; break;
                case ESettingType.Directory: uiType = "DirRequest";  break;
                case ESettingType.String:    uiType = "Text";        break;
                case ESettingType.Integer:   uiType = "Number";      break;                
                case ESettingType.Password:  uiType = "Password";    break;

                case ESettingType.Boolean:   uiType = "CheckBox";  val_ = @""""""; break;
                case ESettingType.IPAddress: uiType = "IPAddress"; val_ = @"""""";  break;
            }

            sb.AppendLine($"\t${{NSD_CreateLabel}} {pPDC.GetPosDim(ESettingType.Label)} \"{title}\"");
            sb.AppendLine($"\tPop $0");

            sb.AppendLine($"\t${{NSD_Create{uiType}}} {pPDC.GetPosDim(SettingType)} {val_}");
            sb.AppendLine($"\tPop ${GetUIVariableName()}");

            if (SettingType == ESettingType.Boolean)
            {
                sb.AppendLine($"\t${{NSD_OnClick}} ${GetUIVariableName()} {GetEventHanderName()}\r\n" +
                    $"\t${{NSD_SetState}} ${GetUIVariableName()} ${GetValueVariableName()}");
            }
            else if (SettingType == ESettingType.IPAddress)
            {
                sb.AppendLine($"\t${{NSD_SetText}} ${GetUIVariableName()} \"${GetValueVariableName()}\"");
            }
            else if (SettingType == ESettingType.File || SettingType == ESettingType.Directory)
            {
                sb.AppendLine($"\t${{NSD_CreateBrowseButton}} {pPDC.GetPosDim(ESettingType.ButtonBrowse)} \"…\"\r\n" +
                    $"\tPop $0\r\n" +
                    $"\t${{NSD_OnClick}} $0 {GetEventHanderName(ESettingType.ButtonBrowse, GetUniqueName())}");
            }

            return sb.ToString();
            // sb.AppendLine($"\t");

        }


        public string GetFormLeaveFunction()
        {
            switch (SettingType)
            {
                case ESettingType.File:
                case ESettingType.Directory:
                case ESettingType.String:
                case ESettingType.Integer:
                case ESettingType.Password:
                case ESettingType.IPAddress:
                return $"\t${{NSD_GetText}} ${GetUIVariableName()} ${GetValueVariableName()}";

                case ESettingType.Boolean:
                return $"\t${{NSD_GetState}} ${GetUIVariableName()} ${GetValueVariableName()}";
            }
            return $"; !!! NO CASE FOR SETTING TYPE '{SettingType}' IN METHOD '{nameof(GetFormLeaveFunction)}' IMPLEMENTED !!!";
        }

        public string GetBrowseButtonClickedFunction()
        {
            return $@"
Function {GetEventHanderName(ESettingType.ButtonBrowse, GetUniqueName())}
    DetailPrint ""Browse button {GetUIVariableName()} clicked.""
    ${{NSD_GetText}} ${GetUIVariableName()} $0
    nsDialogs::Select{GetDirOrFile()}Dialog ""${GetUIVariableName()}"" ""$0""
    Pop $0
    ${{If}} $0 != error
        ${{NSD_SetText}} ${GetUIVariableName()} ""$0""
		${{NSD_SetText}} ${GetValueVariableName()} ""$0""
    ${{EndIf}}
    ; Uncomment and modify below line to enable/disable related user interfaces, or remove it.
	; EnableWindow $xxx_SettingName ${GetValueVariableName()}
FunctionEnd
";
        }

        private object GetDirOrFile()
        {
            if (SettingType == ESettingType.File)
                return "File";
            else if (SettingType == ESettingType.Directory) 
                return "Folder";
#if DEBUG
            throw new NotImplementedException();
#endif
            return "???";
        }

        public string GetCheckBoxChangedFuncString()
        {
            if (SettingType != ESettingType.Boolean)
                return string.Empty;

            return $@"
Function {GetEventHanderName()}
    DetailPrint ""Checkbox {GetUIVariableName()} checked/unchecked.""
    ${{NSD_GetState}} ${GetUIVariableName()} ${GetValueVariableName()}
    ; Uncomment and modify below line to enable/disable related user interfaces, or remove it.
	; EnableWindow $xxx_SettingName ${GetValueVariableName()}
FunctionEnd
";
            /*
	${{If}} $0 == ${{BST_CHECKED}}
	    StrCpy ${GetValueVariableName()} ""true""

	${{Else}}
		StrCpy ${GetValueVariableName()} ""false""
		; EnableWindow $xxx_OptionalUI 0
	${{EndIf}}
*/
        }
    }
}
