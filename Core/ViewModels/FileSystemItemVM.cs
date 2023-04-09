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
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    #endregion Usings

    [XmlRoot]
    public class FileSystemItemVM : IFileSystemItem, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events


        #region Variables
        private EFileSystemType m_FileSystemType;
        private string m_Path;
        #endregion Variables


        #region Ctor
        public FileSystemItemVM() { }

        public FileSystemItemVM(string pPath) : this()
        {
            Path = pPath;

            if (File.Exists(pPath))
                FileSystemType = EFileSystemType.File;
            else if (Directory.Exists(pPath))
                FileSystemType = EFileSystemType.Directory;
            else
                FileSystemType = EFileSystemType.None;

            
        }
        public FileSystemItemVM(IFileSystemItem pFileSystemItem) :this()
        {
            UpdateValues(pFileSystemItem);
        }
        #endregion Ctor


        #region Properties
        [XmlElement]
        public EFileSystemType FileSystemType
        {
            get { return m_FileSystemType; }
            set
            {
                if (value == m_FileSystemType) return;
                m_FileSystemType = value;
                NotifyPropertyChanged(nameof(FileSystemType));
            }
        }

        [XmlElement]
        public string Path
        {
            get { return m_Path; }
            set
            {
                if (value == m_Path) return;
                m_Path = value;
                NotifyPropertyChanged(nameof(Path));
            }
        }
        #endregion Properties


        #region Methods
        public FileSystemItem ToModel()
        {
            // todo: check and review Clone() function!
            return new FileSystemItem
            {
                FileSystemType = FileSystemType,
                Path = Path,
            };
        }

        public void UpdateValues(IFileSystemItem pFileSystemItem)
        {
            FileSystemType = pFileSystemItem.FileSystemType;
            Path = pFileSystemItem.Path;
        }

        public static IEnumerable<FileSystemItemVM> From(IEnumerable<string> pFilePaths)
        {
            foreach (var path in pFilePaths)
                yield return new FileSystemItemVM(path);
        }

        #endregion Methods

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }


}
