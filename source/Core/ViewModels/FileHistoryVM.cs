namespace GeNSIS.Core.ViewModels
{
    using GeNSIS.Core.Serialization;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;


    public class FileHistoryVM
    {
        private IDeSerializer m_DeSerializer;
        private string m_Path;


        public FileHistoryVM()
            => Files = new ObservableCollection<string>();


        public FileHistoryVM(string pPath) : this()
        {
            m_Path = pPath;
            m_DeSerializer = new DeSerializationProvider().GetDeSerializerByExtension(new FileInfo(pPath).Extension);
        }

        public virtual ObservableCollection<string> Files { get; set; } 


        public virtual void Add(string pFilePath)
        {
            var foundPath = Files.FirstOrDefault(p => p.Equals(pFilePath, System.StringComparison.OrdinalIgnoreCase));
            if (foundPath == null)
            {
                Files.Insert(0, pFilePath);
            }
            else
            {
                int index = Files.IndexOf(foundPath);
                if (index != 0)
                    Files.Move(index, 0);
            }

            if (Files.Count > GConst.MAX_LAST_FILES)
                Files.RemoveAt(Files.Count - 1);
        }

        public virtual void AddRange(IEnumerable<string> pFilePaths)
        {
            foreach(var  pFilePath in pFilePaths)
                Files.Add(pFilePath);
        }

        public virtual void Clear() => Files.Clear();

        public virtual string[] GetFiles() => Files.ToArray();

        public virtual void Load()
        {
            Clear();
            if (File.Exists(m_Path))
                AddRange(m_DeSerializer.Deserialize<string[]>(File.ReadAllText(m_Path, encoding: System.Text.Encoding.UTF8)));
        }

        public void Save()
        {
            File.WriteAllText(m_Path, m_DeSerializer.Serialize<string[]>(Files.ToArray()), encoding: System.Text.Encoding.UTF8);
        }

    }
}
