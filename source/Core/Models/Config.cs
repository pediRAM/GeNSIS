﻿/*
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
        public string ProjectsDirectory { get; set; }
        public string ScriptsDirectory { get; set; }
        public string InstallersDirectory { get; set; }
        public string NsisInstallationDirectory { get; set; }
        public string ExternalEditor { get; set; } = GConst.Default.EXTERNAL_EDITOR;

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
                ProjectsDirectory = ProjectsDirectory,
                ScriptsDirectory = ScriptsDirectory,
                InstallersDirectory = InstallersDirectory,
                NsisInstallationDirectory = NsisInstallationDirectory,
                ExternalEditor = ExternalEditor,

                LastScripts = new System.Collections.ObjectModel.ObservableCollection<string>(LastScripts),
                LastProjects = new System.Collections.ObjectModel.ObservableCollection<string>(LastProjects),
            };
        }

        public void UpdateValues(IConfig pIAppConfig)
        {
            CompanyName = pIAppConfig.CompanyName;
            Publisher = pIAppConfig.Publisher;
            Website = pIAppConfig.Website;

            ProjectsDirectory = pIAppConfig.ProjectsDirectory;
            ScriptsDirectory = pIAppConfig.ScriptsDirectory;
            InstallersDirectory = pIAppConfig.InstallersDirectory;

            NsisInstallationDirectory = pIAppConfig.NsisInstallationDirectory;
            ExternalEditor = pIAppConfig.ExternalEditor;

            LastProjects = pIAppConfig.GetLastProjects();
            LastScripts = pIAppConfig.GetLastScripts();
        }

        public void AddProjectPath(string pProjectPath)
        {
            if (!LastProjects.Any(p => p.Equals(pProjectPath, System.StringComparison.OrdinalIgnoreCase)))
                LastProjects.Insert(0, pProjectPath);

            if (LastProjects.Count > GConst.MAX_LAST_FILES)
                LastProjects.RemoveAt(LastProjects.Count - 1);
        }

        public void AddScriptPath(string pScriptPath)
        {
            if (!LastScripts.Any(p => p.Equals(pScriptPath, System.StringComparison.OrdinalIgnoreCase)))
                LastScripts.Insert(0, pScriptPath);

            if (LastScripts.Count > GConst.MAX_LAST_FILES)
                LastScripts.RemoveAt(LastScripts.Count - 1);
        }
    }
}


