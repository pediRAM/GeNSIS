

namespace GeNSIS.Core.ViewModels
{
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class SettingGroupVM : ISettingGroup, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Name;
        private string m_Title;
        private string m_Description;


        public string Name
        {
            get { return m_Name; }
            set
            {
                if (value == m_Name) return;
                m_Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        public string Title
        {
            get { return m_Title; }
            set
            {
                if (value == m_Title) return;
                m_Title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get { return m_Description; }
            set
            {
                if (value == m_Description) return;
                m_Description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public ObservableCollection<ISetting> Settings { get; set; } = new ObservableCollection<ISetting>();

        public IEnumerable<ISetting> GetSettings() => Settings;

        public SettingGroup ToModel(ISettingGroup pSettingGroup)
        {
            return new SettingGroup
            {
                Name = pSettingGroup.Name,
                Title = pSettingGroup.Title,
                Description = pSettingGroup.Description,
            };
        }

        public void SetSettings(IEnumerable<ISetting> pPSettings)
        {
            Settings.Clear();
            Settings.AddRange(pPSettings);
        }

        public void UpdateValues(ISettingGroup pSettingGroup)
        {
            Name = pSettingGroup.Name;
            Title = pSettingGroup.Title;
            Description = pSettingGroup.Description;
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}



