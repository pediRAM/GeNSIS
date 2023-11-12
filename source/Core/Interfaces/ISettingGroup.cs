using System.Collections.Generic;
using System.Linq;

namespace GeNSIS.Core.Interfaces
{
    public interface ISettingGroup
    {
        /// <summary>
        /// Title of page.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Subtitle or description of page (under title).
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Internal name used for the groupbox.
        /// </summary>
        string Name { get; set; }
        IEnumerable<ISetting> GetSettings();
        void SetSettings(IEnumerable<ISetting> pSettings);
        bool HasLongFields() => GetSettings().Any(s => s.SettingType == Enums.ESettingType.File || s.SettingType == Enums.ESettingType.Directory);
    }
}