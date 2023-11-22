using GeNSIS.Core.Enums;
using GeNSIS.Core.Interfaces;
using GeNSIS.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeNSIS.UI
{
    /// <summary>
    /// Interaction logic for EntityWindow.xaml
    /// </summary>
    public partial class EntityWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private bool m_BoolValue;
        private ESettingType m_SettingType = ESettingType.Page;
        private int m_IntegerValue;
        private string m_EntityName = string.Empty;
        private string m_Title = string.Empty;
        private string m_Description = string.Empty;
        private string m_Label = string.Empty;
        private string m_StringValue = string.Empty;
        private string m_PasswordValue = string.Empty;
        private string m_IpValue = "192.168.12.34";

        public EntityWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnEntityDialogLoaded;            
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SettingType))
            {
                UpdateVisibilities();
            }
        }

        private void OnEntityDialogLoaded(object sender, RoutedEventArgs e)
        {
            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
            UpdateVisibilities();
            tbx_Name.Focus();
        }



        public SettingVM GetSetting()
        {
            var s = new SettingVM();
            s.Name = tbx_Name.Text;
            s.Group = null;
            s.Label = Label;
            s.Default = GetDefaultValue(SettingType);
            
            return s;
        }

        public SettingGroupVM GetSettingGroup()
        {
            var s = new SettingGroupVM();
            s.Name = tbx_Name.Text;
            s.Title = PageTitle;
            s.Description = Description;

            return s;
        }

        public void SetSettingGroup(ISettingGroup s)
        {
            SettingType = ESettingType.Page;
            PageTitle = s.Title;
            Description = s.Description;
            EntityName = s.Name;
        }

        private object GetDefaultValue(ESettingType pSettingType)
        {
            switch (pSettingType)
            {
                case ESettingType.Boolean: return BoolValue;

                case ESettingType.File:
                case ESettingType.Directory:
                case ESettingType.String: return StringValue;

                case ESettingType.Password: return pbx_Password.Password;

                case ESettingType.Integer: return nbx_Integer.Text;

                case ESettingType.IPAddress: return IpValue;

                
            }
            return null;
        }

        private void UpdateVisibilities()
        {
            UpdateLabels();
            UpdateMetaText();
            UpdateValueUIs();
        }



        public string EntityName
        {
            get { return m_EntityName; }
            set
            {
                if (value == m_EntityName) return;
                m_EntityName = value;
                NotifyPropertyChanged(nameof(EntityName));
            }
        }

        public string PageTitle
        {
            get { return m_Title; }
            set
            {
                if (value == m_Title) return;
                m_Title = value;
                NotifyPropertyChanged(nameof(PageTitle));
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

        public bool BoolValue
        {
            get { return m_BoolValue; }
            set
            {
                if (value == m_BoolValue) return;
                m_BoolValue = value;
                NotifyPropertyChanged(nameof(BoolValue));
            }
        }

        public string StringValue
        {
            get { return m_StringValue; }
            set
            {
                if (value == m_StringValue) return;
                m_StringValue = value;
                NotifyPropertyChanged(nameof(StringValue));
            }
        }
        public string IpValue
        {
            get { return m_IpValue; }
            set
            {
                if (value == m_IpValue) return;
                m_IpValue = value;
                NotifyPropertyChanged(nameof(IpValue));
            }
        }


        public int IntegerValue
        {
            get { return m_IntegerValue; }
            set
            {
                if (value == m_IntegerValue) return;
                m_IntegerValue = value;
                NotifyPropertyChanged(nameof(IntegerValue));
            }
        }

        public string PasswordValue
        {
            get { return m_PasswordValue; }
            set
            {
                if (value == m_PasswordValue) return;
                m_PasswordValue = value;
                NotifyPropertyChanged(nameof(PasswordValue));
            }
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));

        private void UpdateLabels()
        {
            if (SettingType == ESettingType.Page)
            {
                lbl_Title.Visibility = Visibility.Visible;
                lbl_Desc.Visibility = Visibility.Visible;
                lbl_Label.Visibility = Visibility.Visible;
                lbl_Value.Visibility = Visibility.Collapsed;
            }
            else
            {
                lbl_Title.Visibility = Visibility.Collapsed;
                lbl_Desc.Visibility = Visibility.Collapsed;
                lbl_Label.Visibility = Visibility.Visible;
                lbl_Value.Visibility = Visibility.Visible;
            }
        }

        private void UpdateMetaText()
        {
            if (SettingType == ESettingType.Page)
            {
                tbx_Title.Visibility = Visibility.Visible;
                tbx_Desc.Visibility = Visibility.Visible;
                tbx_Label.Visibility = Visibility.Visible;
            }
            else
            {
                tbx_Title.Visibility = Visibility.Collapsed;
                tbx_Desc.Visibility = Visibility.Collapsed;
                tbx_Label.Visibility = Visibility.Visible;
            }
        }

        private void UpdateValueUIs()
        {
            cbx_Bool.Visibility = Visibility.Collapsed;
            tbx_String.Visibility = Visibility.Collapsed;
            pbx_Password.Visibility = Visibility.Collapsed;
            ipf_IP.Visibility = Visibility.Collapsed;
            nud_Integer.Visibility = Visibility.Collapsed;
            nbx_Integer.Visibility = Visibility.Collapsed;

            switch (SettingType)
            {
                case ESettingType.Page: break;

                case ESettingType.GroupBox: break;

                case ESettingType.Boolean:
                cbx_Bool.Visibility = Visibility.Visible;
                break;

                case ESettingType.String:
                tbx_String.Visibility = Visibility.Visible;
                break;

                case ESettingType.Password:
                pbx_Password.Visibility = Visibility.Visible;
                break;

                case ESettingType.File:
                tbx_String.Visibility = Visibility.Visible;
                break;

                case ESettingType.Directory:
                tbx_String.Visibility = Visibility.Visible;
                break;

                case ESettingType.Integer:
                nbx_Integer.Visibility = Visibility.Visible;
                break;

                case ESettingType.IPAddress:
                ipf_IP.Visibility = Visibility.Visible;
                break;
            }
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
