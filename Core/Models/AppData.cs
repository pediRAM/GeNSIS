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
        public string InstallerHeaderImage { get; set; }

        [XmlElement]
        public string InstallerWizardImage { get; set; }

        [XmlElement]
        public string UninstallerIcon { get; set; }
        [XmlElement]
        public string UninstallerHeaderImage { get; set; }

        [XmlElement]
        public string UninstallerWizardImage { get; set; }

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
                InstallerHeaderImage = InstallerHeaderImage,
                UninstallerHeaderImage = UninstallerHeaderImage,
                InstallerFileName = InstallerFileName,
                InstallerIcon = InstallerIcon,
                UninstallerIcon = UninstallerIcon,
                InstallerWizardImage = InstallerWizardImage,
                UninstallerWizardImage = UninstallerWizardImage,
                Is64BitApplication = Is64BitApplication,
                License = License,
                Publisher = Publisher,
                Url = Url,
            };
        }

        public void UpdateValues(IAppData pAppData)
        {
            AppBuild = pAppData.AppBuild;
            AppName = pAppData.AppName;
            AppVersion = pAppData.AppVersion;
            AssociatedExtension = pAppData.AssociatedExtension;
            Company = pAppData.Company;
            Directories = pAppData.GetDirectories().ToList();
            DoInstallPerUser = pAppData.DoInstallPerUser;
            ExeName = pAppData.ExeName;
            Files = pAppData.GetFiles().ToList();
            InstallerFileName = pAppData.InstallerFileName;
            InstallerHeaderImage = pAppData.InstallerHeaderImage;
            InstallerIcon = pAppData.InstallerIcon;
            InstallerWizardImage = pAppData.InstallerWizardImage;
            Is64BitApplication = pAppData.Is64BitApplication;
            License = pAppData.License;
            Publisher = pAppData.Publisher;
            UninstallerHeaderImage = pAppData.UninstallerHeaderImage;
            UninstallerIcon = pAppData.UninstallerIcon;
            UninstallerWizardImage = pAppData.UninstallerWizardImage;
            Url = pAppData.Url;
        }
    }
}

