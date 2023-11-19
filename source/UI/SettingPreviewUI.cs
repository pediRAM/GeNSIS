using System.Windows;
using System.Windows.Controls;

namespace GeNSIS.UI
{

    namespace GeNSIS.UI
    {
        public class SettingPreviewUI : Control
        {
            static SettingPreviewUI()
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingPreviewUI), new FrameworkPropertyMetadata(typeof(SettingPreviewUI)));
            }

            public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(nameof(PageTitle), typeof(string), typeof(MainWindow));
            public static readonly DependencyProperty PageDescProperty = DependencyProperty.Register(nameof(PageDesc), typeof(string), typeof(MainWindow));
            public static readonly DependencyProperty PageNameProperty = DependencyProperty.Register(nameof(PageName), typeof(string), typeof(MainWindow));

            public string PageTitle
            {
                get => (string)GetValue(PageTitleProperty);
                set => SetValue(PageTitleProperty, value);
            }

            public string PageDesc
            {
                get => (string)GetValue(PageDescProperty);
                set => SetValue(PageDescProperty, value);
            }

            public string PageName
            {
                get => (string)GetValue(PageNameProperty);
                set => SetValue(PageNameProperty, value);
            }
        }
    }
}
