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


namespace GeNSIS.Core
{
    using GeNSIS.Core.Commands;
    #region Usings
    using GeNSIS.Core.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    #endregion Usings

    /// <summary>
    /// Application data model: contains meta-data used in NSIS script.
    /// </summary>
    public class AppDataViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events


        #region Variables
        private bool m_Is64BitApplication;
        private bool m_DoInstallPerUser;
        private string m_AppName;
        private string m_ExeName;
        private string m_AssociatedExtension;
        private string m_AppVersion;
        private string m_AppBuild;
        private string m_AppIcon;
        private string m_Company;
        private string m_License;
        private string m_Publisher = Environment.UserName;
        private string m_Url;
        private bool m_HasUnsavedChanges;
        #endregion Variables


        #region Constructors
        /// <summary>
        /// This default ctor is defined by ProjectManager when loading project.
        /// Use other constructor (with parameter) when creating new project in GUI!
        /// </summary>
        public AppDataViewModel() { }


        public AppDataViewModel(bool pFollowChanges) : this() 
        {
            if (pFollowChanges)
                PropertyChanged += OnPropertyChanged;
        }
        #endregion Constructors


        #region Properties
        public bool Is64BitApplication
        {
            get { return m_Is64BitApplication; }
            set
            {
                if (value == m_Is64BitApplication) return;
                m_Is64BitApplication = value;
            }
        }

        public bool DoInstallPerUser
        {
            get { return m_DoInstallPerUser; }
            set
            {
                if (value == m_DoInstallPerUser) return;
                m_DoInstallPerUser = value;
            }
        }

        public string AppName
        {
            get { return  m_AppName; }
            set
            {
                if (value == m_AppName) return;
                m_AppName = value;
                NotifyPropertyChanged(nameof(AppName));
            }
        }

        public string ExeName
        {
            get { return  m_ExeName; }
            set
            {
                if (value == m_ExeName) return;
                m_ExeName = value;
                NotifyPropertyChanged(nameof(ExeName));

            }
        }

        public string AssociatedExtension
        {
            get { return  m_AssociatedExtension; }
            set
            {
                if (value == m_AssociatedExtension) return;
                m_AssociatedExtension = value;
            }
        }

        public string AppVersion
        {
            get { return  m_AppVersion; }
            set
            {
                if (value == m_AppVersion) return;
                m_AppVersion = value;
                NotifyPropertyChanged(nameof(AppVersion));

            }
        }

        public string AppBuild
        {
            get { return  m_AppBuild; }
            set
            {
                if (value == m_AppBuild) return;
                m_AppBuild = value;
            }
        }

        public string AppIcon
        {
            get { return  m_AppIcon; }
            set
            {
                if (value == m_AppIcon) return;
                m_AppIcon = value;
                NotifyPropertyChanged(nameof(AppIcon));

            }
        }

        public string Company
        {
            get { return  m_Company; }
            set
            {
                if (value == m_Company) return;
                m_Company = value;
                NotifyPropertyChanged(nameof(Company));

            }
        }

        public string License
        {
            get { return  m_License; }
            set
            {
                if (value == m_License) return;
                m_License = value;
                NotifyPropertyChanged(nameof(License));

            }
        }

        public string Publisher
        {
            get { return  m_Publisher; }
            set
            {
                if (value == m_Publisher) return;
                m_Publisher = value;
                NotifyPropertyChanged(nameof(Publisher));

            }
        }

        public string Url
        {
            get { return  m_Url; }
            set
            {
                if (value == m_Url) return;
                m_Url = value;
                NotifyPropertyChanged(nameof(Url));

            }
        }

        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> Directories { get; set; } = new ObservableCollection<string>();

        public bool HasUnsavedChanges
        {
            get { return m_HasUnsavedChanges; }
            set
            {
                if (value == m_HasUnsavedChanges) return;
                m_HasUnsavedChanges = value;
                NotifyPropertyChanged(nameof(HasUnsavedChanges));
            }
        }
        #endregion Properties


        #region Functions
        public void ResetHasUnsavedChanges() => m_HasUnsavedChanges = false;

        public AppData ToModel()
        {
            return new AppData
            {
                Is64BitApplication = Is64BitApplication,
                DoInstallPerUser = DoInstallPerUser,
                AppName = AppName,
                ExeName = ExeName,
                AssociatedExtension = AssociatedExtension,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                AppIcon = AppIcon,
                Company = Company,
                License = License,
                Publisher = Publisher,
                Url = Url,
                Files = Files.ToList(),
                Directories = Directories.ToList(),
            };
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_HasUnsavedChanges = true;
            System.Diagnostics.Trace.TraceInformation($">>>>>>>>>>>> Property: {e.PropertyName} changed.");
        }

        #endregion Functions

        #region Commands
        private ICommand m_RemoveSelectedFileCommand;
        public ICommand RemoveSelectedFileCommand
        {
            get
            {
                if (m_RemoveSelectedFileCommand == null)
                    m_RemoveSelectedFileCommand = new RemoveSelectedFileCommand(this);

                return m_RemoveSelectedFileCommand;
            }
            set
            {
                m_RemoveSelectedFileCommand = value;
            }
        }

        private ICommand m_RemoveSelectedDirectoryCommand;
        public ICommand RemoveSelectedDirectoryCommand
        {
            get
            {
                if (m_RemoveSelectedDirectoryCommand == null)
                    m_RemoveSelectedDirectoryCommand = new RemoveSelectedDirectoryCommand(this);

                return m_RemoveSelectedDirectoryCommand;
            }
            set
            {
                m_RemoveSelectedDirectoryCommand = value;
            }
        }

        private ICommand m_ClearFilesCommand;
        public ICommand ClearFilesCommand
        {
            get
            {
                if (m_ClearFilesCommand == null)
                    m_ClearFilesCommand = new ClearFilesCommand(this);

                return m_ClearFilesCommand;
            }
            set
            {
                m_ClearFilesCommand = value;
            }
        }

        private ICommand m_SetLicenseFileCommand;
        public ICommand SetLicenseFileCommand
        {
            get
            {
                if (m_SetLicenseFileCommand == null)
                    m_SetLicenseFileCommand = new SetLicenseFileCommand(this);

                return m_SetLicenseFileCommand;
            }
            set
            {
                m_SetLicenseFileCommand = value;
            }
        }

        private ICommand m_SetExecutableFileCommand;
        public ICommand SetExecutableFileCommand
        {
            get
            {
                if (m_SetExecutableFileCommand == null)
                    m_SetExecutableFileCommand = new SetExecutableFileCommand(this);

                return m_SetExecutableFileCommand;
            }
            set
            {
                m_SetExecutableFileCommand = value;
            }
        }
        #endregion Commands
    }
}
