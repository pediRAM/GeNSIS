namespace GeNSIS.Core
{
    #region Usings
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion Usings

    /// <summary>
    /// todo: implement and comment type: AppData !
    /// Provides:
    /// <para>Events: <see cref="PropertyChanged"/>.</para>
    /// <para>Properties: <see cref="AppName"/>, <see cref="ExeName"/>, <see cref="AssociatedExtension"/>, <see cref="AppVersion"/>, <see cref="AppBuild"/>, <see cref="AppIcon"/>, <see cref="Company"/>, <see cref="License"/>, <see cref="Publisher"/>, <see cref="Url"/>, <see cref="Files"/>, <see cref="Directories"/>, <see cref="HasUnsavedChanges"/>.</para>
    /// <para>Functions: <see cref="Validate"/>.</para>
    /// </summary>
    public class AppData : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Notifies listeners about PropertyChanged when invoked.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Variables
        private string m_AppName;
        private string m_ExeName;
        private string m_AssociatedExtension;
        private string m_AppVersion;
        private string m_AppBuild;
        private string m_AppIcon;
        private string m_Company;
        private string m_License;
        private string m_Publisher;
        private string m_Url;
        private List<string> m_Files;
        private List<Directory> m_Directories;
        private bool m_HasUnsavedChanges;
        #endregion Variables

        public AppData() : base() { }
        public AppData(bool pFollowChanges) : base() 
        {
            if (pFollowChanges)
                PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) => m_HasUnsavedChanges = true;

        #region Properties
        /// <summary>
        /// Gets/Sets AppName.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets ExeName.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets AssociatedExtension.
        /// </summary>
        public string AssociatedExtension
        {
            get { return  m_AssociatedExtension; }
            set
            {
                if (value == m_AssociatedExtension) return;
                m_AssociatedExtension = value;
            }
        }

        /// <summary>
        /// Gets/Sets AppVersion.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets AppBuild.
        /// </summary>
        public string AppBuild
        {
            get { return  m_AppBuild; }
            set
            {
                if (value == m_AppBuild) return;
                m_AppBuild = value;
            }
        }

        /// <summary>
        /// Gets/Sets AppIcon.
        /// </summary>
        public string AppIcon
        {
            get { return  m_AppIcon; }
            set
            {
                if (value == m_AppIcon) return;
                m_AppIcon = value;
                NotifyPropertyChanged(nameof(AppIcon));

            }
        }

        /// <summary>
        /// Gets/Sets Company.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets License.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets Publisher.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets Url.
        /// </summary>
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

        /// <summary>
        /// Gets/Sets Files.
        /// </summary>
        public List<string> Files
        {
            get { return  m_Files; }
            set
            {
                if (value == m_Files) return;
                m_Files = value;
                NotifyPropertyChanged(nameof(Files));

            }
        }

        /// <summary>
        /// Gets/Sets Directories.
        /// </summary>
        public List<Directory> Directories
        {
            get { return  m_Directories; }
            set
            {
                if (value == m_Directories) return;
                m_Directories = value;
                NotifyPropertyChanged(nameof(Directories));

            }
        }

        /// <summary>
        /// Gets/Sets HasUnsavedChanges.
        /// </summary>
        public bool HasUnsavedChanges => m_HasUnsavedChanges;

        #endregion Properties

        #region Functions


        public void ResetHasUnsavedChanges() => m_HasUnsavedChanges = false;
        #endregion Functions

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
        }
    }
}
