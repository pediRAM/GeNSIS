namespace GeNSIS.Core
{
    #region Usings
    using System.ComponentModel;
    using System;
    #endregion Usings

    /// <summary>
    /// todo: implement and comment type: ProjectManager !
    /// Provides:
    /// <para>Events: <see cref="PropertyChanged"/>.</para>
    /// <para>Functions: <see cref="Load"/>.</para>
    /// <para>Methods: <see cref="Save"/>.</para>
    /// </summary>
    public class ProjectManager
    {
        #region Events
        /// <summary>
        /// Notifies listeners about PropertyChanged when invoked.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Functions
        /// <summary>
        /// todo: implement and comment function Load !
        /// </summary>
        /// <returns>Project</returns>
        public Project Load(string pPath)
        {
            throw new System.NotImplementedException();
        }

        #endregion Functions

        #region Methods
        /// <summary>
        /// todo: implement and comment method Save !
        /// </summary>
        public void Save(string pPath)
        {
            throw new System.NotImplementedException();
        }
        #endregion Methods

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
        }
    }
}
