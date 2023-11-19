using GeNSIS.Core.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeNSIS.UI
{
    /// <summary>
    /// Interaction logic for UiPage.xaml
    /// </summary>
    public partial class UiPage : UserControl
    {
        public UiPage()
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
            var fwe = sender as FrameworkElement;
            if (fwe.Tag == null)
                return;

            if (fwe.Tag is ESettingType)
                AddSetting((ESettingType)fwe.Tag);
            else if (fwe.Tag is string)
                AddSetting((string)fwe.Tag);
        }

        private void AddSetting(string tag)
        {
            throw new NotImplementedException();
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
