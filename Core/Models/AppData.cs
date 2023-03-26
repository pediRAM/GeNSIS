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
    using System.Linq;
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
        public string AssociatedExtension { get; set; }

        [XmlElement]
        public string AppVersion { get; set; }

        [XmlElement]
        public string AppBuild { get; set; }

        [XmlElement]
        public string Company { get; set; }

        [XmlElement]
        public string License { get; set; }

        [XmlElement]
        public string Publisher { get; set; } = Environment.UserName;

        [XmlElement]
        public string Url { get; set; }

        [XmlArray]
        public List<string> Files { get; set; } = new List<string>();

        [XmlArray]
        public List<string> Directories { get; set; } = new List<string>();

        [XmlElement]
        public string InstallerFileName { get; set; }

        [XmlElement]
        public string InstallerIcon { get; set; }
        [XmlElement]
        public string InstallerBannerImage { get; set; }

        [XmlElement]
        public string InstallerWizardImage { get; set; }

        public IEnumerable<string> GetFiles() => Files;
        public IEnumerable<string> GetDirectories() => Directories;

        public AppDataVM ToViewModel()
        {
            return new AppDataVM
            {
                AppBuild = AppBuild,
                AppName = AppName,
                AppVersion = AppVersion,
                AssociatedExtension = AssociatedExtension,
                Company = Company,
                Directories = new System.Collections.ObjectModel.ObservableCollection<string>(Directories),
                DoAddFWRule = DoAddFWRule,
                DoInstallPerUser = DoInstallPerUser,
                ExeName = ExeName,
                Files = new System.Collections.ObjectModel.ObservableCollection<string>(Files),
                InstallerBannerImage = InstallerBannerImage,
                InstallerFileName = InstallerFileName,
                InstallerIcon = InstallerIcon,
                InstallerWizardImage = InstallerWizardImage,
                Is64BitApplication = Is64BitApplication,
                License = License,
                Publisher = Publisher,
                Url = Url,
            };
        }

        public void UpdateValues(IAppData p)
        {
            AppBuild = p.AppBuild;
            AppName = p.AppName;
            AppVersion = p.AppVersion;
            AssociatedExtension = p.AssociatedExtension;
            Company = p.Company;
            Directories = p.GetDirectories().ToList();
            DoInstallPerUser = p.DoInstallPerUser;
            ExeName = p.ExeName;
            Files = p.GetFiles().ToList();
            InstallerBannerImage = p.InstallerBannerImage;
            InstallerFileName = p.InstallerFileName;
            InstallerIcon = p.InstallerIcon;
            InstallerWizardImage = p.InstallerWizardImage;
            Is64BitApplication = p.Is64BitApplication;
            License = p.License;
            Publisher = p.Publisher;
            Url = p.Url;
        }
    }
}

