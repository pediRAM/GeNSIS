using System;
using System.Windows;

namespace GeNSIS.Core.Converters
{
    internal class LicToVisibilityConverter : AValueConverter
    {
        public Visibility WhenEqual { get; set; } = Visibility.Visible;
        public Visibility Else { get; set; } = Visibility.Collapsed;
        public override object Convert(object pValue)
        {
            if (pValue == null) return Else;
            string path = (string)pValue;
            if ((IsTXT(path) || IsRTF(path)) && ContainsLicenseOrEula(path))
                return WhenEqual;
            else return Else;
        }

        private static bool IsTXT(string path)
            => System.IO.Path.GetExtension(path).Equals(".txt", StringComparison.OrdinalIgnoreCase);


        private static bool IsRTF(string path)
            => System.IO.Path.GetExtension(path).Equals(".rtf", StringComparison.OrdinalIgnoreCase);

        private static bool ContainsLicenseOrEula(string path)
        {
            string filename = System.IO.Path.GetFileName(path);
            return filename.Contains("license", StringComparison.OrdinalIgnoreCase) || filename.Contains("eula", StringComparison.OrdinalIgnoreCase);
        }
    }
}
