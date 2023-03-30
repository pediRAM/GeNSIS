/***************************************************************************************
* GeNSIS - a free and open source NSIS installer script generator tool.                *
* Copyright (C) 2023 Pedram Ganjeh Hadidi                                              *
*                                                                                      *
* This file is part of GeNSIS.                                                         *
*                                                                                      *
* GeNSIS is free software: you can redistribute it and/or modify it under the terms    *
* of the GNU General Public License as published by the Free Software Foundation,      *
* either version 3 of the License, or any later version.                               *
*                                                                                      *
* GeNSIS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;  *
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     *
* PURPOSE. See the GNU General Public License for more details.                        *
*                                                                                      *
* You should have received a copy of the GNU General Public License along with GeNSIS. *
* If not, see <https://www.gnu.org/licenses/>.                                         *
****************************************************************************************/


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
