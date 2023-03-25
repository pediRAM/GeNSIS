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
        string AppName { get; }

        bool Is64BitApplication { get; }

        bool DoInstallPerUser { get; }
        bool DoAddFWRule { get; }
        string ExeName { get; }
        string InstallerFileName { get; }

        string AssociatedExtension { get; }

        string AppVersion { get; }

        string AppBuild { get; }

        string AppIcon { get; }

        string Company { get; }

        string License { get; }

        string Publisher { get; }

        string Url { get; }

        IEnumerable<string> GetFiles();

        IEnumerable<string> GetDirectories();

        void UpdateValues(IAppData pAppData);
    }
}
