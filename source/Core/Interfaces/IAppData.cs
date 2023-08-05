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


using System.Collections.Generic;

namespace GeNSIS.Core.Interfaces
{
    public interface IAppData
    {
        string AppName { get; }

        bool Is64BitApplication { get; }

        bool DoInstallPerUser { get; }
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

        void UpdateValues(IAppData pAppData);
    }
}
