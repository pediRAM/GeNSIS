namespace GeNSIS.Core
{
    #region Usings
    using System.ComponentModel;
    using System;
    #endregion Usings

    /// <summary>
    /// todo: implement and comment type: Project !
    /// Provides:
    /// <para>Events: <see cref="PropertyChanged"/>.</para>
    /// <para>Properties: <see cref="Version"/>, <see cref="Note"/>.</para>
    /// </summary>
    public class Project
    {
        #region Events
        /// <summary>
        /// Notifies listeners about PropertyChanged when invoked.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Variables
        private string m_Version;
        private string m_Note;
        #endregion Variables

        #region Properties
        /// <summary>
        /// Gets/Sets Version.
        /// </summary>
        public string Version
        {
            get { return  m_Version; }
            set
            {
                if (value == m_Version) return;
                m_Version = value;
            }
        }

        /// <summary>
        /// Gets/Sets Note.
        /// </summary>
        public string Note
        {
            get { return  m_Note; }
            set
            {
                if (value == m_Note) return;
                m_Note = value;
            }
        }
        #endregion Properties

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
        }
    }
}
