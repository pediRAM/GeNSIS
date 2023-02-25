namespace GeNSIS.Core.Models
{
    using System.Xml.Serialization;

    [XmlRoot]
    public class Project
    {
        [XmlElement]
        public string Version { get; set; } = AsmConst.MODEL_VERSION;

        [XmlElement]
        public string Note { get; set; }

        [XmlElement]
        public AppData AppData { get; set; }

        public ProjectViewModel ToViewModel()
        {
            return new ProjectViewModel
            {
                Version = Version,
                Note = Note,
                AppData = AppData.ToViewModel()
            };
        }

    }
}
