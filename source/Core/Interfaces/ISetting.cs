using GeNSIS.Core.Enums;

namespace GeNSIS.Core.Interfaces
{
    public interface ISetting
    {
        /// <summary>
        /// The group to which this setting belongs.
        /// </summary>
        ISettingGroup Group { get; set; }

        /// <summary>
        /// Internal (semantic) name of setting, like: "Firstname", "City", "IP",...
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// External (displayname) name of setting, which is shown at left, like: "Firstname:", "Port:", "Enabled:",...
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Type of setting.
        /// </summary>
        ESettingType SettingType { get; set; }

        /// <summary>
        /// Default value.
        /// </summary>
        object Default { get; set; }

        /// <summary>
        /// Returns the name of input-UI, like: "pwd_GroupnameSettingname".
        /// </summary>
        /// <returns></returns>
        string GetUIVariableName();
        string GetTitleVariableName();
        string GetValueVariableName();
    }
}