/*
GeNSIS (GEnerates NullSoft Installer Script)
Copyright (C) 2023 Pedram GANJEH HADIDI

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace GeNSIS.Core.Converters
{
    public class FileExtensionToImageSourceConverter : AValueConverter
    {
        public override object Convert(object pValue)
        {
            if (pValue == null) 
                return null;
            
            try
            {
                var fileExtension = System.IO.Path.GetExtension((string)pValue);
                return GetImage(fileExtension);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return null;
            }
        }

        private static BitmapImage GetImage(string PFileExtension)
        {
            if (string.IsNullOrWhiteSpace(PFileExtension))
                return CreateImage("unknown.png");

            switch (PFileExtension.ToLower())
            {
                case ".bat":
                case ".cmd": return CreateImage("application-x-shellscript.png");

                case ".dll": return CreateImage("kcmsystem.png");

                case ".doc":
                case ".docx":
                case ".rtf": return CreateImage("application-x-abiword.png");

                case ".7z":
                case ".7zip":
                case ".rar":
                case ".zip": return CreateImage("application-x-zip.png");

                case ".css":
                case ".js":                
                case ".htm":
                case ".html":
                case ".xml": return CreateImage("application-xhtml+xml.png");

                case ".pdf": return CreateImage("application-pdf.png");

                case ".md": return CreateImage("template_source.png");

                case ".txt": return CreateImage("txt.png");

                case ".exe": return CreateImage("preferences-system-windows.png");

                default: return CreateImage("unknown.png");
            }
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
