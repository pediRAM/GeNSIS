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
    #region Usings
    using GeNSIS.Core.Commands;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using GeNSIS.Core.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Eventing.Reader;
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
        private bool m_CreateCompanyFolder;
        private string m_Arch;
        private string m_MachineType;
        private bool m_DoInstallPerUser;
        private bool m_DoAddFWRule;
        private string m_AppName;
        private FileSystemItemVM m_ExeName;
        private string m_AssociatedExtension;
        private string m_AppVersion;
        private string m_AppBuild;
        private string m_Company;
        private FileSystemItemVM m_License;
        private string m_Publisher = Environment.UserName;
        private string m_Url;
        private string m_InstallerFileName = GConst.Default.INSTALLER_FILENAME;
        private string m_InstallerIcon;
        private string m_InstallerHeaderImage;
        private string m_InstallerWizardImage;

        private string m_UninstallerIcon;
        private string m_UninstallerHeaderImage;
        private string m_UninstallerWizardImage;
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

        public bool DoCreateCompanyDir
        {
            get { return m_CreateCompanyFolder; }
            set
            {
                if (value == m_CreateCompanyFolder) return;
                m_CreateCompanyFolder = value;
                NotifyPropertyChanged(nameof(DoCreateCompanyDir));
            }
        }

        public string Arch
        {
            get { return m_Arch; }
            set
            {
                if (value == m_Arch) return;
                m_Arch = value;
                NotifyPropertyChanged(nameof(Arch));
            }
        }

        public string MachineType
        {
            get { return m_MachineType; }
            set
            {
                if (value == m_MachineType) return;
                m_MachineType = value;
                NotifyPropertyChanged(nameof(MachineType));
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

        public IFileSystemItem ExeName
        {
            get { return  m_ExeName; }
            set
            {
                if (value == m_ExeName) return;

                if (value is FileSystemItemVM)
                    m_ExeName = value as FileSystemItemVM;
                else if (value != null)
                    m_ExeName = new FileSystemItemVM(value);
                else m_ExeName = null;

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

        public IFileSystemItem License
        {
            get { return  m_License; }
            set
            {
                if (value == m_License) return;

                if (value is FileSystemItemVM)
                    m_License = value as FileSystemItemVM;
                else if (value != null)
                    m_License = new FileSystemItemVM(value);
                else
                    m_License = null;                

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

        public string InstallerHeaderImage
        {
            get { return m_InstallerHeaderImage; }
            set
            {
                if (value == m_InstallerHeaderImage) return;
                m_InstallerHeaderImage = value;
                NotifyPropertyChanged(nameof(InstallerHeaderImage));
            }
        }

        public string InstallerWizardImage
        {
            get { return m_InstallerWizardImage; }
            set
            {
                if (value == m_InstallerWizardImage) return;
                m_InstallerWizardImage = value;
                NotifyPropertyChanged(nameof(InstallerWizardImage));
            }
        }

        public string UninstallerIcon
        {
            get { return m_UninstallerIcon; }
            set
            {
                if (value == m_UninstallerIcon) return;
                m_UninstallerIcon = value;
                NotifyPropertyChanged(nameof(UninstallerIcon));

            }
        }

        public string UninstallerHeaderImage
        {
            get { return m_UninstallerHeaderImage; }
            set
            {
                if (value == m_UninstallerHeaderImage) return;
                m_UninstallerHeaderImage = value;
                NotifyPropertyChanged(nameof(UninstallerHeaderImage));
            }
        }

        public string UninstallerWizardImage
        {
            get { return m_UninstallerWizardImage; }
            set
            {
                if (value == m_UninstallerWizardImage) return;
                m_UninstallerWizardImage = value;
                NotifyPropertyChanged(nameof(UninstallerWizardImage));
            }
        }

        public ObservableCollection<FileSystemItemVM> Files { get; set; } = new ObservableCollection<FileSystemItemVM>();

        public ObservableCollection<SectionVM> Sections { get; set; } = new ObservableCollection<SectionVM>();

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
        public IEnumerable<IFileSystemItem> GetFiles() => Files;
        public IEnumerable<ISection> GetSections() => Sections;
        public void ResetHasUnsavedChanges() => m_HasUnsavedChanges = false;

        public AppData ToModel()
        {
            var clone = new AppData
            {
                AppBuild = AppBuild,
                AppName = AppName,
                AppVersion = AppVersion,
                AssociatedExtension = AssociatedExtension,
                Company = Company,
                DoAddFWRule = DoAddFWRule,
                DoInstallPerUser = DoInstallPerUser,
                ExeName = ExeName,
                InstallerHeaderImage = InstallerHeaderImage,
                UninstallerHeaderImage = UninstallerHeaderImage,
                InstallerFileName = InstallerFileName,
                InstallerIcon = InstallerIcon,
                UninstallerIcon = UninstallerIcon,
                InstallerWizardImage = InstallerWizardImage,
                UninstallerWizardImage = UninstallerWizardImage,
                Is64BitApplication = Is64BitApplication,
                DoCreateCompanyDir = DoCreateCompanyDir,
                Arch = Arch,
                MachineType = MachineType,
                License = License,
                Publisher = Publisher,
                Url = Url,
            };

            clone.Sections = new List<Section>();
            foreach (var s in Sections)
                clone.Sections.Add(new Section(s));

            clone.Files = new List<FileSystemItem>();
            foreach (var f in Files)
                clone.Files.Add(new FileSystemItem(f));

            return clone;
        }

        public void UpdateValues(IAppData pAppData)
        {
            AppBuild = pAppData.AppBuild;
            AppName = pAppData.AppName;
            AppVersion = pAppData.AppVersion;
            AssociatedExtension = pAppData.AssociatedExtension;
            Company = pAppData.Company;
            DoInstallPerUser = pAppData.DoInstallPerUser;
            ExeName = (pAppData.ExeName == null) ? new FileSystemItemVM() : new FileSystemItemVM(pAppData.ExeName);
            InstallerHeaderImage = pAppData.InstallerHeaderImage;
            UninstallerHeaderImage = pAppData.UninstallerHeaderImage;
            InstallerFileName = pAppData.InstallerFileName;
            InstallerIcon = pAppData.InstallerIcon;
            UninstallerIcon = pAppData.UninstallerIcon;
            InstallerWizardImage = pAppData.InstallerWizardImage;
            UninstallerWizardImage = pAppData.UninstallerWizardImage;
            Is64BitApplication = pAppData.Is64BitApplication;
            DoCreateCompanyDir = pAppData.DoCreateCompanyDir;
            Arch = pAppData.Arch;
            MachineType = pAppData.MachineType;
            License = (pAppData.License == null) ? new FileSystemItemVM() : new FileSystemItemVM(pAppData.License);
            Publisher = pAppData.Publisher;
            Url = pAppData.Url;

            Files.Clear();
            foreach (var f in pAppData.GetFiles())
                Files.Add(new FileSystemItemVM(f));

            Sections.Clear();
            foreach (var s in pAppData.GetSections())
                Sections.Add(new SectionVM(s));
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => m_HasUnsavedChanges = true;

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
                m_AutoRetrieveExeDataCommand ??= new AutoFillAppDataCommand(this);
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


        private ICommand m_ResetInstallerNameCommand;
        public ICommand ResetInstallerNameCommand
        {
            get
            {
                m_ResetInstallerNameCommand ??= new ResetInstallerNameCommand(this);
                return m_ResetInstallerNameCommand;
            }
            set => m_ResetInstallerNameCommand = value;
        }


        public ICommand m_ClearInstallerNameCommand;
        public ICommand ClearInstallerNameCommand
        {
            get
            {
                m_ClearInstallerNameCommand ??= new ClearInstallerNameCommand(this);
                return m_ClearInstallerNameCommand;
            }
            set => m_ClearInstallerNameCommand = value;
        }
        #endregion Commands
    }
}
