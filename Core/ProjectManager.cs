using GeNSIS.Core.Models;
using GeNSIS.Core.Serialization;
using System.IO;

namespace GeNSIS.Core
{
    public class ProjectManager
    {
        public Project Load(string pPath)
        {
            var provider = new DeSerializationProvider();
            var fileInfo = new FileInfo(pPath);
            var deserializer = provider.GetDeSerializerByExtension(fileInfo.Extension);
            return deserializer.ToProject(File.ReadAllText(pPath, encoding: System.Text.Encoding.UTF8));
        }

        public void Save(string pPath, Project pProject)
        {
            var provider = new DeSerializationProvider();
            var deserializer = provider.GetDeSerializerByExtension(Path.GetExtension(pPath));
            File.WriteAllText(pPath, deserializer.ToString(pProject), encoding: System.Text.Encoding.UTF8);
        }
    }
}
