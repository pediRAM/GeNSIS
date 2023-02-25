namespace GeNSIS.Core.Serialization
{
    using System.IO;
    using System.Xml.Serialization;

    class XmlDeSerializer : IDeSerializer
    {
        public string DisplayName => "XML";

        public string Extension => ".xml";

        public Project ToProject(string pModelString)
        {
            Project project = null;
            var xmlSerializer = new XmlSerializer(typeof(Project));

            using (var stringReader = new StringReader(pModelString))
            {
                project = (Project)xmlSerializer.Deserialize(stringReader);
            }
            return project;
        }

        public string ToString(Project project)
        {
            var xmlString = string.Empty;
            var xmlSerializer = new XmlSerializer(typeof(Project));

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, project);
                xmlString = stringWriter.ToString();
            }
            return xmlString;
        }
    }
}
