namespace GeNSIS.Core
{
    using GeNSIS.Core.Models;
    using System.ComponentModel;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Note = string.Empty;
        private AppDataViewModel m_AppData;

        public string Version { get; set; } = AsmConst.MODEL_VERSION;

        public string Note
        {
            get { return m_Note; }
            set
            {
                if (value == m_Note) return;
                m_Note = value;
                NotifyPropertyChanged(nameof(Note));
            }
        }

        public AppDataViewModel AppData
        {
            get { return m_AppData; }
            set
            {
                if (value == m_AppData) return;
                m_AppData = value;
                NotifyPropertyChanged(nameof(AppData));
            }
        }

        public Project ToModel()
        {
            return new Project
            {
                Version = Version,
                Note = Note,
                AppData = AppData.ToModel(),
            };
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
    }
}
