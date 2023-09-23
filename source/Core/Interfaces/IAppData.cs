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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeNSIS.Core.Interfaces
{
    public interface IAppData
    {
        string RelativePath { get; }

        string AppName { get; }
        bool IsService { get; }
        IServiceData Service { get; }
        bool Is64BitApplication { get; }

        EInstallTargetType InstallationTarget { get; }
        string CustomInstallDir { get; }
        bool DoAddFWRule { get; }

        IFileSystemItem ExeName { get; }       

        string AssociatedExtension { get; }

        string AppVersion { get; }

        string AppBuild { get; }
        bool DoCreateCompanyDir { get; }
        string Arch { get; }
        string MachineType { get; }

        string Company { get; }

        IFileSystemItem License { get; }

        string Publisher { get; }

        string Url { get; }

        string InstallerFileName { get; }
        string InstallerIcon { get; }
        string InstallerHeaderImage { get; }
        string InstallerWizardImage { get; }

        string UninstallerIcon { get; }
        string UninstallerHeaderImage { get; }
        string UninstallerWizardImage { get; }

        IEnumerable<IFileSystemItem> GetFiles();

        IEnumerable<ISection> GetSections();

        IEnumerable<IFirewallRule> GetFirewallRules();

        void UpdateValues(IAppData pAppData);

        string GetFullPath(IFileSystemItem pItem) 
            => pItem.IsRelative ? Path.Combine(RelativePath, pItem.Path) : pItem.Path;

        string GetNsisPath(IFileSystemItem pItem)
            => pItem.IsRelative ? $"${{{GConst.Nsis.BASE_DIR}}}\\{pItem.Name}" : pItem.Path;

        long GetSize(IFileSystemItem pItem) => 
            pItem.FSType == Enums.EFileSystemType.File? new FileInfo(GetFullPath(pItem)).Length :
            GetFiles().Where(x => x.FSType == Enums.EFileSystemType.Directory).Sum(x => new DirectoryInfo(GetFullPath(x)).GetFiles("*", SearchOption.AllDirectories).Sum(y => y.Length));

        long GetTotalSize() => GetFiles().Sum(x => GetSize(x));
    }
}
