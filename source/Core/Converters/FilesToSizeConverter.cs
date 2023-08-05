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


using System.Globalization;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GeNSIS.Core.Converters
{
    internal class FilesToSizeConverter : AMultiValueConverter
    {
        public override object Convert(object[] pValues, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var files = (IEnumerable<string>)pValues[0];
                var dirs  = (IEnumerable<string>)pValues[1];
                var totalSize = files.Sum(x => new FileInfo(x).Length);
                var totalDirSize = dirs.Sum(x => new DirectoryInfo(x).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));
                var total = (totalSize + totalDirSize) / 1024;

                return (total / 1024) >= 1024 ? total / 1024 + " MB" : total + " KB";
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
    }
}
