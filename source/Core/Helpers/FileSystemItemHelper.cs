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


using GeNSIS.Core.Enums;
using GeNSIS.Core.Interfaces;
namespace GeNSIS.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;


    internal static class FileSystemItemHelper
    {
        public static List<IFileSystemItem> GetMissingItems(IAppData p)
        {
            var pMissingFiles = new List<IFileSystemItem>();

            foreach (var f in p.GetFiles())
            {
                if (!FileSystemItemExists(f, p.RelativePath))
                    pMissingFiles.Add(f);
            }
            return pMissingFiles;
        }

        private static bool FileSystemItemExists(IFileSystemItem pItem, string pRelativePath)
        {
            switch(pItem.FSType)
            {
                case EFileSystemType.None:
                case EFileSystemType.File:      return FileExists(pItem, pRelativePath);
                case EFileSystemType.Directory: return DirectoryExists(pItem, pRelativePath);
                default: throw new NotImplementedException($"Handling case for enum:'{nameof(EFileSystemType)}.{pItem}' is not implemented!");
            }
        }

        private static bool DirectoryExists(IFileSystemItem pDir, string pRelativePath)
        {
            if (pDir.IsRelative)
                return Directory.Exists(Path.Combine(pRelativePath, pDir.Path));
            else
                return Directory.Exists(pDir.Path);
        }

        private static bool FileExists(IFileSystemItem pFile, string pRelativePath)
        {
            if (pFile.IsRelative)
                return File.Exists(Path.Combine(pRelativePath, pFile.Path));
            else
                return File.Exists(pFile.Path);
        }
    }
}
