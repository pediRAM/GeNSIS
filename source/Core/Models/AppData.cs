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
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot]
    public class AppData : IAppData
    {
        [XmlElement]
        public string RelativePath { get; set; }

        [XmlElement]
        public bool Is64BitApplication { get; set; }

        [XmlElement]
        public bool IsService { get; set; }

        [XmlElement]
        public IServiceData Service { get; set; } = new ServiceData();

        [XmlElement]
        public EInstallTargetType InstallationTarget { get; set; } = EInstallTargetType.PerUser;

        [XmlElement]
        public string CustomInstallDir { get; set; }
        
        [XmlElement]
        public bool DoAddFWRule { get; set; }

        [XmlArray]
        [XmlArrayItem(typeof(FirewallRule))]
        public List<FirewallRule> FirewallRules { get; set; } = new List<FirewallRule>();


        [XmlElement]
        public string AppName { get; set; }

        [XmlElement(typeof(FileSystemItem))]        
        public IFileSystemItem ExeName { get; set; }

        [XmlElement]
        public bool DoCreateCompanyDir { get; set; }

        [XmlElement]
        public string Arch { get; set; }

        [XmlElement]
        public string MachineType { get; set; }

        [XmlElement]
        public string AssociatedExtension { get; set; }

        [XmlElement]
        public string AppVersion { get; set; }

        [XmlElement]
        public string AppBuild { get; set; }

        [XmlElement]
        public string Company { get; set; }

        [XmlElement(typeof(FileSystemItem))]
        public IFileSystemItem License { get; set; }

        [XmlElement]
        public string Publisher { get; set; } = Environment.UserName;

        [XmlElement]
        public string Url { get; set; }

        [XmlArray]
        [XmlArrayItem(typeof(FileSystemItem))]
        public List<FileSystemItem> Files { get; set; } = new List<FileSystemItem>();


        [XmlArray]
        public List<Section> Sections { get; set; } = new List<Section>();

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

        public IEnumerable<IFileSystemItem> GetFiles() => Files;
        public IEnumerable<ISection> GetSections() => Sections;
        public IEnumerable<IFirewallRule> GetFirewallRules() => FirewallRules;

        public AppDataVM ToViewModel()
        {
            var vm = new AppDataVM
            {
                RelativePath = RelativePath,
                AppBuild = AppBuild,
                AppName = AppName,
                AppVersion = AppVersion,
                AssociatedExtension = AssociatedExtension,
                Company = Company,
                DoAddFWRule = DoAddFWRule,
                InstallationTarget = InstallationTarget,
                CustomInstallDir = CustomInstallDir,
                
                InstallerHeaderImage = InstallerHeaderImage,
                UninstallerHeaderImage = UninstallerHeaderImage,
                InstallerFileName = InstallerFileName,
                InstallerIcon = InstallerIcon,
                UninstallerIcon = UninstallerIcon,
                InstallerWizardImage = InstallerWizardImage,
                UninstallerWizardImage = UninstallerWizardImage,
                Is64BitApplication = Is64BitApplication,
                IsService = IsService,
                Service = new ServiceDataVM(Service),
                DoCreateCompanyDir = DoCreateCompanyDir,
                Arch = Arch,
                MachineType = MachineType,
                
                Publisher = Publisher,
                Url = Url,
            };

            vm.ExeName = (ExeName as FileSystemItem)?.ToViewModel();
            vm.License = (License as FileSystemItem)?.ToViewModel();

            vm.Files = new System.Collections.ObjectModel.ObservableCollection<FileSystemItemVM>();
            foreach (var f in Files)
                vm.Files.Add(f.ToViewModel());

            vm.Sections = new System.Collections.ObjectModel.ObservableCollection<SectionVM>();
            foreach (var s in Sections)
                vm.Sections.Add(s.ToViewModel());

            vm.FirewallRules = new System.Collections.ObjectModel.ObservableCollection<FirewallRuleVM>();
            foreach (var fwr in FirewallRules)
                vm.FirewallRules.Add(fwr.ToViewModel());

            return vm;
        }

        public void UpdateValues(IAppData pAppData)
        {
            RelativePath = pAppData.RelativePath;
            AppBuild = pAppData.AppBuild;
            AppName = pAppData.AppName;
            AppVersion = pAppData.AppVersion;
            AssociatedExtension = pAppData.AssociatedExtension;
            Company = pAppData.Company;
            DoAddFWRule = pAppData.DoAddFWRule;
            InstallationTarget = pAppData.InstallationTarget;
            CustomInstallDir = pAppData.CustomInstallDir;
            ExeName = (pAppData.ExeName == null) ? new FileSystemItem() : new FileSystemItem(pAppData.ExeName);
            InstallerFileName = pAppData.InstallerFileName;
            InstallerHeaderImage = pAppData.InstallerHeaderImage;
            InstallerIcon = pAppData.InstallerIcon;
            InstallerWizardImage = pAppData.InstallerWizardImage;
            Is64BitApplication = pAppData.Is64BitApplication;
            IsService = pAppData.IsService;
            Service.UpdateValues(pAppData.Service);
            DoCreateCompanyDir = pAppData.DoCreateCompanyDir;
            Arch = pAppData.Arch;
            MachineType = pAppData.MachineType;
            License = (pAppData.License == null) ? new FileSystemItem() : new FileSystemItem(pAppData.License);
            Publisher = pAppData.Publisher;
            UninstallerHeaderImage = pAppData.UninstallerHeaderImage;
            UninstallerIcon = pAppData.UninstallerIcon;
            UninstallerWizardImage = pAppData.UninstallerWizardImage;
            Url = pAppData.Url;

            Files.Clear();
            foreach (var f in pAppData.GetFiles())
                Files.Add(new FileSystemItem(f));

            Sections.Clear();
            foreach (var s in pAppData.GetSections())
                Sections.Add(new Section(s));

            FirewallRules.Clear();
            foreach (var fwr in pAppData.GetFirewallRules())
                FirewallRules.Add(new FirewallRule(fwr));
        }
    }
}

