
using GeNSIS.Core.Enums;
using IPUserControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;



namespace GeNSIS.UI
{
    [ContentProperty("Content")]
    public class SettingPreviewUI : Control, ICloneable
    {
        static SettingPreviewUI()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingPreviewUI), new FrameworkPropertyMetadata(typeof(SettingPreviewUI)));
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(SettingPreviewUI));

        public static readonly DependencyProperty EntityNameProperty = DependencyProperty.Register(nameof(EntityName), typeof(string), typeof(SettingPreviewUI));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(SettingPreviewUI));
        public static readonly DependencyProperty SettingTypeProperty = DependencyProperty.Register(nameof(SettingType), typeof(ESettingType), typeof(SettingPreviewUI));
        //public static readonly DependencyProperty PageNameProperty = DependencyProperty.Register(nameof(PageName), typeof(string), typeof(SettingPreviewUI));


        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public string EntityName
        {
            get => (string)GetValue(EntityNameProperty);
            set => SetValue(EntityNameProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public ESettingType SettingType
        {
            get => (ESettingType)GetValue(SettingTypeProperty);
            set => SetValue(SettingTypeProperty, value);
        }

        public object Clone()
        {
            SettingPreviewUI clone = new SettingPreviewUI();
            clone.EntityName = EntityName;
            clone.Label = Label;
            clone.SettingType = SettingType;

            clone.Content = GetContent(SettingType);
            return clone;
        }

        private object GetContent(ESettingType settingType)
        {
            switch (settingType)
            {
                case ESettingType.Boolean: return new CheckBox { };
                case ESettingType.String: return new TextBox { };
                case ESettingType.Integer: return new NumberBox { };

                case ESettingType.File: 
                case ESettingType.Directory: return new TextBox { };

                case ESettingType.IPAddress: return new IpField { };
                case ESettingType.Password: return new PasswordBox { };
            }
            return new Button { Content = "Häh?" };
        }
    }
}

