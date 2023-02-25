namespace GeNSIS.Core.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot]
    public class AppData
    {
        [XmlElement]
        public bool Is64BitApplication { get; set; }

        [XmlElement]
        public bool DoInstallPerUser { get; set; }

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
        public string AppIcon { get; set; }

        [XmlElement]
        public string Company { get; set; }

        [XmlElement]
        public string License { get; set; }

        [XmlElement]
        public string Publisher { get; set; }

        [XmlElement]
        public string Url { get; set; }

        [XmlElement]
        [XmlArray]
        public List<string> Files { get; set; } = new List<string>();

        [XmlElement]
        [XmlArray]
        public List<Directory> Directories { get; set; } = new List<Directory>();

        public AppDataViewModel ToViewModel()
        {
            return new AppDataViewModel
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
                Files = new System.Collections.ObjectModel.ObservableCollection<string>(Files),
                Directories = new System.Collections.ObjectModel.ObservableCollection<Directory>(Directories),
            };
        }
    }
}

