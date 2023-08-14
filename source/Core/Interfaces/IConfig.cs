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
    public interface IConfig
    {
        string CompanyName { get; set; }
        string Publisher { get; set; }
        string Website { get; set; }
        string GeNSISProjectsDirectory { get; set; }
        string ScriptsDirectory { get; set; }
        string InstallersDirectory { get; set; }
        string NsisInstallationDirectory { get; set; }
        string ExternalEditor { get; set; }
        List<string> GetLastProjects();
        List<string> GetLastScripts();
        void UpdateValues(IConfig pIAppConfig);
    }
}
