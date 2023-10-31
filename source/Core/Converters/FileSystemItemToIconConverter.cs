using GeNSIS.Core.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GeNSIS.Core.Converters
{
    internal class FileSystemItemToIconConverter : AMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values == null || values.Length == 0) return null;
                IFileSystemItem selected = (IFileSystemItem)values[0];
                IFileSystemItem exeName = (IFileSystemItem)values[0];
                if (selected == null) return null;
                string ext = Path.GetExtension(selected.Path);

                if (selected.FSType == Enums.EFileSystemType.File)
                {
                    bool IsRTF = ext.Equals(".rtf", StringComparison.OrdinalIgnoreCase);
                    bool IsEXE = ext.Equals(".exe", StringComparison.OrdinalIgnoreCase);
                    if (IsEXE && selected.Path == exeName.Path)
                        return "preferences-system-windows.png";
                    else if ((IsRTF || IsTXT(ext)) && (ContainsEula(selected) || ContainsLicense(selected)) && selected.Path == exeName.Path)
                        return "paragraph.png";
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private static bool ContainsEula(IFileSystemItem selected)
        {
            return selected.Name.Contains("eula", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsLicense(IFileSystemItem selected)
        {
            return selected.Name.Contains("license", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTXT(string ext)
        {
            return ext.Equals(".txt", StringComparison.OrdinalIgnoreCase);
        }

        private static BitmapImage CreateImage(string pIconName)
        {
            try
            {
                return new BitmapImage(new Uri($"Resources/Icons/nuvola/22x22/{pIconName}", uriKind: UriKind.Relative));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return null;
            }
        }
    }
}
