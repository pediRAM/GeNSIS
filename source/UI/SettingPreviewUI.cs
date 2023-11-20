
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

        public static readonly DependencyProperty ButtonVisibilityProperty = DependencyProperty.Register(nameof(ButtonVisibility), typeof(Visibility), typeof(SettingPreviewUI));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(SettingPreviewUI));

        public static readonly DependencyProperty EntityNameProperty   = DependencyProperty.Register(nameof(EntityName),  typeof(string), typeof(SettingPreviewUI));
        public static readonly DependencyProperty LabelProperty        = DependencyProperty.Register(nameof(Label),       typeof(string), typeof(SettingPreviewUI));
        public static readonly DependencyProperty DisplayNameProperty  = DependencyProperty.Register(nameof(DisplayName), typeof(string), typeof(SettingPreviewUI));
        public static readonly DependencyProperty SettingTypeProperty  = DependencyProperty.Register(nameof(SettingType), typeof(ESettingType), typeof(SettingPreviewUI));
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(nameof(DefaultValue), typeof(ESettingType), typeof(SettingPreviewUI));
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

        public string DisplayName
        {
            get => $"{(ESettingType)GetValue(SettingTypeProperty)} {(string)GetValue(EntityNameProperty)}";
            //set => SetValue(EntityNameProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public object DefaultValue
        {
            get => (object)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        public Visibility ButtonVisibility
        {
            get => (Visibility)GetValue(ButtonVisibilityProperty);
            set => SetValue(ButtonVisibilityProperty, value);
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
            clone.ButtonVisibility = ButtonVisibility;
            clone.Content = GetContent(SettingType);
            return clone;
        }

        private object GetContent(ESettingType settingType)
        {
            var c = (Content as Control);
            var p = GetContentProperties(c);

            switch (settingType)
            {
                case ESettingType.Boolean:
                
                return new CheckBox
                {
                    VerticalAlignment = p.VA,
                    HorizontalAlignment = p.HA,
                    Height = p.H,
                    Width = p.W
                };

                case ESettingType.String:
                return new TextBox
                {
                    VerticalAlignment = p.VA,
                    HorizontalAlignment = p.HA,
                    Height = p.H,
                    Width = p.W
                };


                case ESettingType.Integer:

                return new NumberBox
                {
                    VerticalAlignment = p.VA,
                    HorizontalAlignment = p.HA,
                    Height = p.H,
                    Width = p.W
                };

                case ESettingType.File: 
                case ESettingType.Directory:

                return new TextBox
                {
                    VerticalAlignment = p.VA,
                    HorizontalAlignment = p.HA,
                    Height = p.H,
                    Width = p.W
                };

                case ESettingType.IPAddress: return new IpField
                {
                    VerticalAlignment = p.VA,
                    HorizontalAlignment = p.HA,
                    Height = p.H,
                    Width = p.W
                };

                case ESettingType.Password: return new PasswordBox
                {
                    VerticalAlignment = p.VA,
                    HorizontalAlignment = p.HA,
                    Height = p.H,
                    Width = p.W
                };
            }

            return new Button { Content = "Häh?" };
        }

        private (HorizontalAlignment HA, VerticalAlignment VA, double W, double H) GetContentProperties(Control control)
             => (control.HorizontalAlignment, control.VerticalAlignment, control.ActualWidth, control.ActualHeight);
        

        public void SetDefaultValue(object value)
        {
            switch (SettingType)
            {
                case ESettingType.Boolean: (Content as CheckBox).IsChecked = (bool)value; break;
                
                case ESettingType.Integer: (Content as NumberBox).Text = (string)value; break;

                case ESettingType.File:
                case ESettingType.Directory:
                case ESettingType.String: (Content as TextBox).Text = (string)value; break;

                case ESettingType.IPAddress: (Content as IpField).IpAddress = (string)value; break; //<-- Not tested!
                case ESettingType.Password: (Content as PasswordBox).Password = (string) value; break;
            }
        }
    }
}

