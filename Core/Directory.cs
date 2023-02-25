/***************************************************************************************
* This file is part of GeNSIS.                                                         *
*                                                                                      *
* GeNSIS is free software: you can redistribute it and/or modify it under the terms    *
* of the GNU General Public License as published by the Free Software Foundation,      *
* either version 3 of the License, or any later version.                               *
*                                                                                      *
* GeNSIS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;  *
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR     *
* PURPOSE. See the GNU General Public License for more details.                        *
*                                                                                      *
* You should have received a copy of the GNU General Public License along with GeNSIS. *
* If not, see <https://www.gnu.org/licenses/>.                                         *
****************************************************************************************/


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
