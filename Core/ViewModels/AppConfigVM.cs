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
    using GeNSIS.Core.Extensions;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    public class AppConfigVM : IAppConfig
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string m_CompanyName;
        private string m_Publisher;
        private string m_Website;

        private string m_GeNSISProjectsDirectory;
        private string m_ScriptsDirectory;
        private string m_InstallersDirectory;

        private string m_NsisInstallationDirectory;

        public AppConfigVM() { }
        public AppConfigVM(bool pRegisterForChanges) : this() 
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

        public string GeNSISProjectsDirectory
        {
            get { return m_GeNSISProjectsDirectory; }
            set
            {
                if (value == m_GeNSISProjectsDirectory) return;
                m_GeNSISProjectsDirectory = value;
                NotifyPropertyChanged(nameof(GeNSISProjectsDirectory));
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

        public ObservableCollection<string> LastProjects { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> LastScripts { get; set; } = new ObservableCollection<string>();

        public List<string> GetLastProjects() => LastProjects.ToList();
        public List<string> GetLastScripts() => LastScripts.ToList();
        public AppConfig ToModel()
        {
            return new AppConfig
            {
                CompanyName = CompanyName,
                Publisher = Publisher,
                Website = Website,
                GeNSISProjectsDirectory = GeNSISProjectsDirectory,
                ScriptsDirectory = ScriptsDirectory,
                InstallersDirectory = InstallersDirectory,
                NsisInstallationDirectory = NsisInstallationDirectory,

                LastProjects = LastProjects.ToList(),
                LastScripts = LastScripts.ToList(),
            };
        }

        public void UpdateValues(IAppConfig pIAppConfig)
        {
            CompanyName = pIAppConfig.CompanyName;
            Publisher = pIAppConfig.Publisher;
            Website = pIAppConfig.Website;

            GeNSISProjectsDirectory = pIAppConfig.GeNSISProjectsDirectory;
            ScriptsDirectory = pIAppConfig.ScriptsDirectory;
            InstallersDirectory = pIAppConfig.InstallersDirectory;

            NsisInstallationDirectory = pIAppConfig.NsisInstallationDirectory;

            LastProjects.Clear();
            LastProjects.AddRange(pIAppConfig.GetLastProjects());

            LastScripts.Clear();
            LastScripts.AddRange(pIAppConfig.GetLastScripts());
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}


