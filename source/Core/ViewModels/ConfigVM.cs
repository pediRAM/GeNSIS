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


namespace GeNSIS.Core.Models
{
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Interfaces;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    public class ConfigVM : IConfig
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string m_CompanyName;
        private string m_Publisher;
        private string m_Website;

        private string m_GeNSISProjectsDirectory;
        private string m_ScriptsDirectory;
        private string m_InstallersDirectory;

        private string m_NsisInstallationDirectory;
        private string m_ExternalEditor = GConst.Default.EXTERNAL_EDITOR;

        public ConfigVM() { }
        public ConfigVM(bool pRegisterForChanges) : this() 
        {
            if (pRegisterForChanges)
                PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HasUnsavedChanges = true;
        }

        public bool HasUnsavedChanges { get; set; }

        public string CompanyName
        {
            get { return m_CompanyName; }
            set
            {
                if (value == m_CompanyName) return;
                m_CompanyName = value;
                NotifyPropertyChanged(nameof(CompanyName));
            }
        }

        public string Publisher
        {
            get { return m_Publisher; }
            set
            {
                if (value == m_Publisher) return;
                m_Publisher = value;
                NotifyPropertyChanged(nameof(Publisher));
            }
        }

        public string Website
        {
            get { return m_Website; }
            set
            {
                if (value == m_Website) return;
                m_Website = value;
                NotifyPropertyChanged(nameof(Website));
            }
        }

        public string ProjectsDirectory
        {
            get { return m_GeNSISProjectsDirectory; }
            set
            {
                if (value == m_GeNSISProjectsDirectory) return;
                m_GeNSISProjectsDirectory = value;
                NotifyPropertyChanged(nameof(ProjectsDirectory));
            }
        }

        public string ScriptsDirectory
        {
            get { return m_ScriptsDirectory; }
            set
            {
                if (value == m_ScriptsDirectory) return;
                m_ScriptsDirectory = value;
                NotifyPropertyChanged(nameof(ScriptsDirectory));
            }
        }

        public string InstallersDirectory
        {
            get { return m_InstallersDirectory; }
            set
            {
                if (value == m_InstallersDirectory) return;
                m_InstallersDirectory = value;
                NotifyPropertyChanged(nameof(InstallersDirectory));
            }
        }
        
        public string NsisInstallationDirectory
        {
            get { return m_NsisInstallationDirectory; }
            set
            {
                if (value == m_NsisInstallationDirectory) return;
                m_NsisInstallationDirectory = value;
                NotifyPropertyChanged(nameof(NsisInstallationDirectory));
            }
        }

        public string ExternalEditor
        {
            get { return m_ExternalEditor; }
            set
            {
                if (value == m_ExternalEditor) return;
                m_ExternalEditor = value;
                NotifyPropertyChanged(nameof(ExternalEditor));
            }
        }

        public ObservableCollection<string> LastProjects { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> LastScripts { get; set; } = new ObservableCollection<string>();

        public List<string> GetLastProjects() => LastProjects.ToList();
        public List<string> GetLastScripts() => LastScripts.ToList();
        public Config ToModel()
        {
            return new Config
            {
                CompanyName = CompanyName,
                Publisher = Publisher,
                Website = Website,
                ProjectsDirectory = ProjectsDirectory,
                ScriptsDirectory = ScriptsDirectory,
                InstallersDirectory = InstallersDirectory,
                NsisInstallationDirectory = NsisInstallationDirectory,
                ExternalEditor = ExternalEditor,

                LastProjects = LastProjects.ToList(),
                LastScripts = LastScripts.ToList(),
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

            LastProjects.Clear();
            LastProjects.AddRange(pIAppConfig.GetLastProjects());

            LastScripts.Clear();
            LastScripts.AddRange(pIAppConfig.GetLastScripts());
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
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


