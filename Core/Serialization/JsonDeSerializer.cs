namespace GeNSIS.Core.Serialization
{
    using Newtonsoft.Json;

    public class JsonDeSerializer : IDeSerializer
    {
        public string DisplayName => "Json";

        public string Extension => ".json";

        public Project ToProject(string pModelString)
            => JsonConvert.DeserializeObject<Project>(pModelString);

        public string ToString(Project project)
            => JsonConvert.SerializeObject(project, Formatting.Indented);
    }
}
