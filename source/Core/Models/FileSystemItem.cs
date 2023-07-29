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

namespace GeNSIS.Core.Models
{
    #region Usings
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.ViewModels;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    #endregion Usings


    [XmlRoot]
    public class FileSystemItem : IFileSystemItem
    {
        #region Ctors
        public FileSystemItem() { }

        public FileSystemItem(string pPath) : this()
        {
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

        public FileSystemItem(IFileSystemItem pFileSystemItem) : this()
        {
            UpdateValues(pFileSystemItem);
        }
        #endregion Ctors


        #region Properties
        [XmlElement]
        public EFileSystemType FileSystemType { get; set; }

        [XmlElement]
        public string Path { get; set; }

        [XmlElement]
        public string Name { get; set; }
        #endregion Properties


        #region Functions
        public FileSystemItemVM ToModelView()
        {
            return new FileSystemItemVM
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

        public static IEnumerable<FileSystemItem> From(IEnumerable<string> pFilePaths)
        {
            foreach (var path in pFilePaths)
                yield return new FileSystemItem(path);
        }
        #endregion Functions
    }
}
