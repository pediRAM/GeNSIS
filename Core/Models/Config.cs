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
    using GeNSIS.Core.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class Config : IConfig
    {
        public string CompanyName { get; set; }

        public string Publisher { get; set; }
        public string Website { get; set; }

        public string GeNSISProjectsDirectory { get; set; }

        public string ScriptsDirectory { get; set; }
        public string InstallersDirectory { get; set; }

        public string NsisInstallationDirectory { get; set; }

  
        public List<string> LastProjects { get; set; } = new List<string>();
        public List<string> LastScripts { get; set; } = new List<string>();

        public List<string> GetLastProjects() => LastProjects;
        public List<string> GetLastScripts() => LastScripts;

        public ConfigVM Clone()
        {
            return new ConfigVM
            {
                CompanyName = CompanyName,
                Publisher = Publisher,
                Website = Website,
                GeNSISProjectsDirectory = GeNSISProjectsDirectory,
                ScriptsDirectory = ScriptsDirectory,
                InstallersDirectory = InstallersDirectory,
                NsisInstallationDirectory = NsisInstallationDirectory,

                LastScripts = new System.Collections.ObjectModel.ObservableCollection<string>(LastScripts),
                LastProjects = new System.Collections.ObjectModel.ObservableCollection<string>(LastProjects),
            };
        }

        public void UpdateValues(IConfig pIAppConfig)
        {
            CompanyName = pIAppConfig.CompanyName;
            Publisher = pIAppConfig.Publisher;
            Website = pIAppConfig.Website;

            GeNSISProjectsDirectory = pIAppConfig.GeNSISProjectsDirectory;
            ScriptsDirectory = pIAppConfig.ScriptsDirectory;
            InstallersDirectory = pIAppConfig.InstallersDirectory;

            NsisInstallationDirectory = pIAppConfig.NsisInstallationDirectory;

            LastProjects = pIAppConfig.GetLastProjects();
            LastScripts = pIAppConfig.GetLastScripts();
        }
    }
}


