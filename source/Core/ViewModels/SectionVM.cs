/***************************************************************************************
* GeNSIS - a free and open source NSIS installer script generator tool.                *
* Copyright (C) 2023 Pedram Ganjeh Hadidi                                              *
*                                                                                      *
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

namespace GeNSIS.Core.ViewModels
{
    #region Usings
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using System.ComponentModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    #endregion Usings

    [XmlRoot]
    public class SectionVM : ISection, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events


        #region Variables
        private string m_Name;
        private string m_SourcePath;
        private string m_TargetInstallDir;
        #endregion Variables


        public SectionVM() { }
        public SectionVM(string pDirPath) : this()
        {
            SourcePath = pDirPath;
            Name = Path.GetFileName(pDirPath);
            TargetInstallDir = Name;
        }
        public SectionVM(ISection pSection) : this()
        {
            UpdateValues(pSection);
        }


        #region Properties
        [XmlElement]
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (value == m_Name) return;
                m_Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        [XmlElement]
        public string SourcePath
        {
            get { return m_SourcePath; }
            set
            {
                if (value == m_SourcePath) return;
                m_SourcePath = value;
                NotifyPropertyChanged(nameof(SourcePath));
            }
        }

        [XmlElement]
        public string TargetInstallDir
        {
            get { return m_TargetInstallDir; }
            set
            {
                if (value == m_TargetInstallDir) return;
                m_TargetInstallDir = value;
                NotifyPropertyChanged(nameof(TargetInstallDir));
            }
        }
        #endregion Properties


        #region Methods
        public Section ToModel()
        {
            return new Section
            {
                Name = Name,
                SourcePath = SourcePath,
                TargetInstallDir = TargetInstallDir,
            };
        }

        public void UpdateValues(ISection s)
        {
            Name = s.Name;
            SourcePath = s.SourcePath;
            TargetInstallDir = s.TargetInstallDir;
        }

        #endregion Methods

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }


}
