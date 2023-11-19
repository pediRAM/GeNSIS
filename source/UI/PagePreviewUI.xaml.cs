using GeNSIS.Core.Enums;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.UI
{
    /// <summary>
    /// Interaction logic for PagePreviewUI.xaml
    /// </summary>
    public partial class PagePreviewUI : UserControl
    {
        public PagePreviewUI()
        {
            InitializeComponent();
        }

        private void OnPageTitleDoubleClicked(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnPageDescDoubleClicked(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnGroupBoxDoubleClicked(object sender, MouseButtonEventArgs e)
        {

        }

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
                    dropObj.Label = setting.Label;
                    dropObj.EntityName = dlg.EntityName;
                    dropObj.SetDefaultValue(setting.Default);
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
