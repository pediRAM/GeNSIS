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
    using System.ComponentModel;
    public class AppConfigVM : IAppConfig
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string m_CompanyName;
        private string m_Publisher;
        private string m_GeNSISProjectsDirectory;
        private string m_ScriptsDirectory;
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

        public AppConfig ToModel()
        {
            return new AppConfig
            {
                CompanyName = CompanyName,
                Publisher = Publisher,
                GeNSISProjectsDirectory = GeNSISProjectsDirectory,
                ScriptsDirectory = ScriptsDirectory,
                NsisInstallationDirectory = NsisInstallationDirectory,
            };
        }

        public void UpdateValues(IAppConfig pIAppConfig)
        {
            CompanyName = pIAppConfig.CompanyName;
            Publisher = pIAppConfig.Publisher;
            GeNSISProjectsDirectory = pIAppConfig.GeNSISProjectsDirectory;
            ScriptsDirectory = pIAppConfig.ScriptsDirectory;
            NsisInstallationDirectory = pIAppConfig.NsisInstallationDirectory;
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}


