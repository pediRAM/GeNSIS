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


namespace GeNSIS.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot]
    public class AppData : IAppData
    {
        [XmlElement]
        public bool Is64BitApplication { get; set; }

        [XmlElement]
        public bool DoInstallPerUser { get; set; }
        
        [XmlElement]
        public bool DoAddFWRule { get; set; }

        [XmlElement]
        public string AppName { get; set; }

        [XmlElement]
        public string ExeName { get; set; }

        [XmlElement]
        public string InstallerFileName { get; set; }

        [XmlElement]
        public string AssociatedExtension { get; set; }

        [XmlElement]
        public string AppVersion { get; set; }

        [XmlElement]
        public string AppBuild { get; set; }

        [XmlElement]
        public string AppIcon { get; set; }

        [XmlElement]
        public string Company { get; set; }

        [XmlElement]
        public string License { get; set; }

        [XmlElement]
        public string Publisher { get; set; } = Environment.UserName;

        [XmlElement]
        public string Url { get; set; }

        [XmlElement]
        [XmlArray]
        public List<string> Files { get; set; } = new List<string>();

        [XmlElement]
        [XmlArray]
        public List<string> Directories { get; set; } = new List<string>();

        public IEnumerable<string> GetFiles() => Files;
        public IEnumerable<string> GetDirectories() => Directories;

        public AppDataViewModel ToViewModel()
        {
            return new AppDataViewModel
            {
                Is64BitApplication = Is64BitApplication,
                DoInstallPerUser = DoInstallPerUser,
                DoAddFWRule = DoAddFWRule,
                AppName = AppName,
                ExeName = ExeName,
                InstallerFileName = InstallerFileName,
                AssociatedExtension = AssociatedExtension,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                AppIcon = AppIcon,
                Company = Company,
                License = License,
                Publisher = Publisher,
                Url = Url,
                Files = new System.Collections.ObjectModel.ObservableCollection<string>(Files),
                Directories = new System.Collections.ObjectModel.ObservableCollection<string>(Directories),
            };
        }
    }
}

