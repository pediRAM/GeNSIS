using GeNSIS.Core.Enums;
using GeNSIS.Core.Interfaces;
using GeNSIS.Core.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.UI
{
    /// <summary>
    /// Interaction logic for PagePreviewUI.xaml
    /// </summary>
    public partial class PagePreviewUI : UserControl, INotifyPropertyChanged
    {
        private SettingGroupVM m_SettingGroup = new SettingGroupVM
        {
            Name = "Doubleclick to edit!",
            Title = "Doubleclick to edit Title and Description!",
            Description = "Doubleclick to edit Title and Description!"
        };

        public PagePreviewUI()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingGroupVM SettingGroup
        {
            get => m_SettingGroup;
            set
            {
                if (m_SettingGroup == value)
                    return;

                m_SettingGroup = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SettingGroup)));    
            }
        }

        private void OnPageTitleDoubleClicked(object sender, MouseButtonEventArgs e) => ShowPageDialog();
        private void ShowPageDialog()
        {
            var dlg = new EntityWindow();
            dlg.SetSettingGroup(m_SettingGroup);

            if (dlg.ShowDialog() == true)
                UpdatePageValues(dlg.GetSettingGroup());
        }

        private void UpdatePageValues(ISettingGroup x)
        {
            SettingGroup.Title = x.Title;
            SettingGroup.Description = x.Description;
            SettingGroup.Name = x.Name;
        }

        private void OnPageDescDoubleClicked(object sender, MouseButtonEventArgs e) => ShowPageDialog();
        private void OnGroupBoxDoubleClicked(object sender, MouseButtonEventArgs e) => ShowPageDialog();


        private void OnDropped(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(SettingPreviewUI)))
            {
                try
                {
                    // Retrieve the dropped data as a string
                    SettingPreviewUI dropObj = (SettingPreviewUI)e.Data.GetData(typeof(SettingPreviewUI));
                    if (dropObj == null)
                        return;
                    
                    
                    var dlg = new EntityWindow();
                    //dlg.EntityName = dropObj.SettingType.ToString();
                    dlg.Label = dropObj.Label;
                    dlg.SettingType = dropObj.SettingType;

                    if (dlg.ShowDialog() != true)
                        return;

                    var setting = dlg.GetSetting();
                    setting.Group = SettingGroup;
                    dropObj.Label = setting.Label;
                    dropObj.EntityName = setting.Name;
                    dropObj.SetDefaultValue(setting.Default);
                    SettingGroup.Settings.Add(setting);
                    stackpanel.Children.Add(dropObj);
                    stackpanel.UpdateLayout();
                }
                catch (Exception ex)
                {
                    var x = ex.ToString(); 
                    System.Diagnostics.Trace.TraceError(x);
                }
            }
        }

        private void AddSetting(ESettingType pSettingType)
        {
            switch (pSettingType)
            {
                case ESettingType.Boolean: break;
                case ESettingType.Integer: break;
                case ESettingType.String: break;
                case ESettingType.Password: break;
                case ESettingType.File: break;
                case ESettingType.Directory: break;

                default: throw new NotImplementedException();
            }
        }
    }
}
