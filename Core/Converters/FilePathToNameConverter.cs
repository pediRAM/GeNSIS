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


using GeNSIS.Core.Interfaces;
using System;

namespace GeNSIS.Core.Converters
{
    class FilePathToNameConverter : AValueConverter
    {
        public override object Convert(object pValue)
        {
            if (null != pValue)
            {
                try
                {
                    if (pValue is IFileSystemItem)
                        return System.IO.Path.GetFileName(((IFileSystemItem)pValue).Path);

                    else if (pValue is ISection)
                        return ((ISection)pValue).Name;

                    else if (pValue is string)
                        return System.IO.Path.GetFileName((string)pValue);

                    return "Unknown Type!";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.ToString());
                }
            }

            return pValue;
        }
    }
}
