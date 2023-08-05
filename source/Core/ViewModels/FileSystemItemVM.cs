/*
GeNSIS (GEnerates NullSoft Installer Script)
Copyright (C) 2023 Pedram GANJEH HADIDI

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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
        private string m_Name;
        #endregion Variables


        #region Ctor
        public FileSystemItemVM() { }

        public FileSystemItemVM(string pPath) : this()
        {
            Path = pPath;

            if (File.Exists(pPath))
            {
                FileSystemType = EFileSystemType.File;
                Name = System.IO.Path.GetFileName(pPath);
            }
            else if (Directory.Exists(pPath))
            {
                FileSystemType = EFileSystemType.Directory;
                Name = System.IO.Path.GetFileName(pPath);
            }
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
        #endregion Properties


        #region Methods
        public FileSystemItem ToModel()
        {
            return new FileSystemItem
            {
                FileSystemType = FileSystemType,
                Path = Path,
                Name = Name,
            };
        }

        public void UpdateValues(IFileSystemItem pFileSystemItem)
        {
            FileSystemType = pFileSystemItem.FileSystemType;
            Path = pFileSystemItem.Path;
            Name = pFileSystemItem.Name;
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
