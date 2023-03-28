﻿/***************************************************************************************
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
    #region Usings
    using GeNSIS.Core.Commands;
    using GeNSIS.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    #endregion Usings

    /// <summary>
    /// Application data model: contains meta-data used in NSIS script.
    /// </summary>
    public class AppDataVM : IAppData, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events


        #region Variables
        private bool m_Is64BitApplication;
        private bool m_DoInstallPerUser;
        private bool m_DoAddFWRule;
        private string m_AppName;
        private string m_ExeName;
        private string m_AssociatedExtension;
        private string m_AppVersion;
        private string m_AppBuild;
        private string m_Company;
        private string m_License;
        private string m_Publisher = Environment.UserName;
        private string m_Url;
        private string m_InstallerFileName;
        private string m_InstallerIcon;
        private string m_InstallerBannerImage;
        private string m_InstallerWelcomeLeftImage;
        private bool m_HasUnsavedChanges;
        #endregion Variables


        #region Constructors
        /// <summary>
        /// This default ctor is defined by ProjectManager when loading project.
        /// Use other constructor (with parameter) when creating new project in GUI!
        /// </summary>
        public AppDataVM() { }


        public AppDataVM(bool pFollowChanges) : this() 
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

        public bool DoAddFWRule
        {
            get { return m_DoAddFWRule; }
            set
            {
                if (value == m_DoAddFWRule) return;
                m_DoAddFWRule = value;
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
                NotifyPropertyChanged(nameof(AppBuild));
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


        public string InstallerFileName
        {
            get { return m_InstallerFileName; }
            set
            {
                if (value == m_InstallerFileName) return;
                m_InstallerFileName = value;
                NotifyPropertyChanged(nameof(InstallerFileName));
            }
        }

        public string InstallerIcon
        {
            get { return m_InstallerIcon; }
            set
            {
                if (value == m_InstallerIcon) return;
                m_InstallerIcon = value;
                NotifyPropertyChanged(nameof(InstallerIcon));

            }
        }

        public string InstallerBannerImage
        {
            get { return m_InstallerBannerImage; }
            set
            {
                if (value == m_InstallerBannerImage) return;
                m_InstallerBannerImage = value;
                NotifyPropertyChanged(nameof(InstallerBannerImage));
            }
        }

        public string InstallerWizardImage
        {
            get { return m_InstallerWelcomeLeftImage; }
            set
            {
                if (value == m_InstallerWelcomeLeftImage) return;
                m_InstallerWelcomeLeftImage = value;
                NotifyPropertyChanged(nameof(InstallerWizardImage));
            }
        }

        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> Directories { get; set; } = new ObservableCollection<string>();

        public bool HasUnsavedChanges
        {
            get { return m_HasUnsavedChanges; }
            private set
            {
                if (value == m_HasUnsavedChanges) return;
                m_HasUnsavedChanges = value;
                NotifyPropertyChanged(nameof(HasUnsavedChanges));
            }
        }
        #endregion Properties


        #region Methods
        public IEnumerable<string> GetFiles() => Files;
        public IEnumerable<string> GetDirectories() => Directories;
        public void ResetHasUnsavedChanges() => m_HasUnsavedChanges = false;

        public AppData ToModel()
        {
            return new AppData
            {
                AppBuild = AppBuild,
                AppName = AppName,
                AppVersion = AppVersion,
                AssociatedExtension = AssociatedExtension,
                Company = Company,
                Directories = Directories.ToList(),
                DoAddFWRule = DoAddFWRule,
                DoInstallPerUser = DoInstallPerUser,
                ExeName = ExeName,
                Files = Files.ToList(),
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
            DoInstallPerUser = p.DoInstallPerUser;
            ExeName = p.ExeName;
            InstallerBannerImage = p.InstallerBannerImage;
            InstallerFileName = p.InstallerFileName;
            InstallerIcon = p.InstallerIcon;
            InstallerWizardImage = p.InstallerWizardImage;
            Is64BitApplication = p.Is64BitApplication;
            License = p.License;
            Publisher = p.Publisher;
            Url = p.Url;

            Files.Clear();
            foreach (var f in p.GetFiles()) 
                Files.Add(f);

            Directories.Clear();
            foreach (var d in p.GetDirectories()) 
                Directories.Add(d);
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_HasUnsavedChanges = true;
            System.Diagnostics.Trace.TraceInformation($">>>>>>>>>>>> Property: {e.PropertyName} changed.");
        }

        #endregion Methods

        #region Commands
        private ICommand m_RemoveSelectedFileCommand;
        public ICommand RemoveSelectedFileCommand
        {
            get
            {
                m_RemoveSelectedFileCommand ??= new RemoveSelectedFileCommand(this);
                return m_RemoveSelectedFileCommand;
            }
            set => m_RemoveSelectedFileCommand = value;
        }

        private ICommand m_RemoveSelectedDirectoryCommand;
        public ICommand RemoveSelectedDirectoryCommand
        {
            get
            {
                m_RemoveSelectedDirectoryCommand ??= new RemoveSelectedDirectoryCommand(this);
                return m_RemoveSelectedDirectoryCommand;
            }
            set => m_RemoveSelectedDirectoryCommand = value;
        }

        private ICommand m_ClearFilesCommand;
        public ICommand ClearFilesCommand
        {
            get
            {
                m_ClearFilesCommand ??= new ClearFilesCommand(this);
                return m_ClearFilesCommand;
            }
            set => m_ClearFilesCommand = value;
        }

        private ICommand m_SetLicenseFileCommand;
        public ICommand SetLicenseFileCommand
        {
            get
            {
                m_SetLicenseFileCommand ??= new SetLicenseFileCommand(this);
                return m_SetLicenseFileCommand;
            }
            set => m_SetLicenseFileCommand = value;
        }

        private ICommand m_SetExecutableFileCommand;
        public ICommand SetExecutableFileCommand
        {
            get
            {
                m_SetExecutableFileCommand ??= new SetExecutableFileCommand(this);
                return m_SetExecutableFileCommand;
            }
            set => m_SetExecutableFileCommand = value;
        }

        private ICommand m_SetIconFileCommand;
        public ICommand SetIconFileCommand
        {
            get
            {
                m_SetIconFileCommand ??= new SetIconFileCommand(this);
                return m_SetIconFileCommand;
            }
            set => m_SetIconFileCommand = value;
        }

        private ICommand m_AutoRetrieveExeDataCommand;
        public ICommand AutoRetrieveExeDataCommand
        {
            get
            {
                m_AutoRetrieveExeDataCommand ??= new AutoRetrieveExeDataCommand(this);
                return m_AutoRetrieveExeDataCommand;
            }
            set => m_AutoRetrieveExeDataCommand = value;
        }

        private ICommand m_AutoCreateInstallerNameCommand;
        public ICommand AutoCreateInstallerNameCommand
        {
            get
            {
                m_AutoCreateInstallerNameCommand ??= new AutoCreateInstallerNameCommand(this);
                return m_AutoCreateInstallerNameCommand;
            }
            set => m_AutoCreateInstallerNameCommand = value;
        }
        #endregion Commands
    }
}
