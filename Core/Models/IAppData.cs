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


using System.Collections.Generic;

namespace GeNSIS.Core.Models
{
    public interface IAppData
    {
        string AppName { get; set; }

        bool Is64BitApplication { get; set; }

        bool DoInstallPerUser { get; set; }

        string ExeName { get; set; }

        string AssociatedExtension { get; set; }

        string AppVersion { get; set; }

        string AppBuild { get; set; }

        string AppIcon { get; set; }

        string Company { get; set; }

        string License { get; set; }

        string Publisher { get; set; }

        string Url { get; set; }

        IEnumerable<string> GetFiles();

        IEnumerable<string> GetDirectories();
    }
}
