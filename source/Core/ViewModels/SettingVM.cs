namespace GeNSIS.Core.ViewModels
{
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using System.ComponentModel;


    public class SettingVM : ISetting, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ESettingType m_SettingType = ESettingType.String;
        private object m_Default;
        private ISettingGroup m_Group;
        private string m_Name;
        private string m_Label;

        public SettingVM() { }

        public SettingVM(ISetting pSetting) : this()
            => UpdateValues(pSetting);

        public object Default
        {
            get { return m_Default; }
            set
            {
                if (value == m_Default) return;
                m_Default = value;
                NotifyPropertyChanged(nameof(Default));
            }
        }

        public ISettingGroup Group
        {
            get { return m_Group; }
            set
            {
                if (value == m_Group) return;
                m_Group = value;
                NotifyPropertyChanged(nameof(Group));
            }
        }

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

        public ESettingType SettingType
        {
            get { return m_SettingType; }
            set
            {
                if (value == m_SettingType) return;
                m_SettingType = value;
                NotifyPropertyChanged(nameof(SettingType));
            }
        }

        public string Label 
        {
            get { return m_Label; }
            set
            {
                if (value == m_Label) return;
                m_Label = value;
                NotifyPropertyChanged(nameof(Label));
            }
        }

        public Setting ToModel(ISetting pSetting)
        {
            return new Setting
            {
                Default = pSetting.Default,
                Group = pSetting.Group,
                Name = pSetting.Name,
                SettingType = pSetting.SettingType,
            };
        }

        public void UpdateValues(ISetting pSetting)
        {
            Default = pSetting.Default;
            Group = pSetting.Group;
            Name = pSetting.Name;
            SettingType = pSetting.SettingType;
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }

        public string GetUIVariableName()
        {
            throw new System.NotImplementedException();
        }

        public string GetTitleVariableName()
        {
            throw new System.NotImplementedException();
        }

        public string GetValueVariableName()
        {
            throw new System.NotImplementedException();
        }
    }
}
