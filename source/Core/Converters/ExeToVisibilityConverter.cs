using System;
using System.Windows;

namespace GeNSIS.Core.Converters
{
    internal class ExeToVisibilityConverter : AValueConverter
    {
        public Visibility WhenEqual { get; set; } = Visibility.Visible;
        public Visibility Else { get; set; } = Visibility.Collapsed;
        public override object Convert(object pValue)
        {
            if (pValue == null) return Else;
            string path = (string)pValue;
            if (System.IO.Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                return WhenEqual;
            else return Else;
        }
    }
}
