namespace GeNSIS.Core
{
    #region Usings
    using System.ComponentModel;
    #endregion Usings

    /// <summary>
    /// todo: implement and comment type: Directory !
    /// Provides:
    /// <para>Events: <see cref="PropertyChanged"/>.</para>
    /// <para>Properties: <see cref="Path"/>, <see cref="IsRecursive"/>.</para>
    /// </summary>
    public class Directory
    {
        #region Events
        /// <summary>
        /// Notifies listeners about PropertyChanged when invoked.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Variables
        private string m_Path;
        private bool m_IsRecursive;
        #endregion Variables

        #region Properties
        /// <summary>
        /// Gets/Sets Path.
        /// </summary>
        public string Path
        {
            get { return  m_Path; }
            set
            {
                if (value == m_Path) return;
                m_Path = value;
            }
        }

        /// <summary>
        /// Gets/Sets IsRecursive.
        /// </summary>
        public bool IsRecursive
        {
            get { return  m_IsRecursive; }
            set
            {
                if (value == m_IsRecursive) return;
                m_IsRecursive = value;
            }
        }
        #endregion Properties

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
        }
    }
}
