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
using System.Windows.Media.Imaging;

namespace GeNSIS.Core.Converters
{
    public class StringToImageSourceConverter : AValueConverter
    {
        public override object Convert(object pValue)
        {
            if (pValue == null) return null;

            string path = (string)pValue;
            try
            {
                return new BitmapImage(new Uri(path, uriKind: UriKind.Absolute));
            }
            catch(UriFormatException)
            {
                try
                {
                    return new BitmapImage(new Uri(path, uriKind: UriKind.Relative));
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                return null;
            }
        }
    }
}
