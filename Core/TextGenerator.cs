namespace GeNSIS.Core
{
    #region Usings
    using System.ComponentModel;
    using System;
    #endregion Usings

    /// <summary>
    /// todo: implement and comment type: TextGenerator !
    /// Provides:
    /// <para>Events: <see cref="PropertyChanged"/>.</para>
    /// <para>Functions: <see cref="Generate"/>.</para>
    /// </summary>
    public class TextGenerator
    {
        #region Events
        /// <summary>
        /// Notifies listeners about PropertyChanged when invoked.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Functions
        /// <summary>
        /// todo: implement and comment function Generate !
        /// </summary>
        /// <returns>string</returns>
        public string Generate(AppData pAppData)
        {
            throw new System.NotImplementedException();
        }

        #endregion Functions

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
        }
    }
}
