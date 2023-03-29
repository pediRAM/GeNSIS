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


using GeNSIS.Core.Models;
using System;

namespace GeNSIS.Core.Helpers
{
    internal class PathHelper
    {
        //private static IAppConfig s_AppConfig;
        //public static void SetAppConfig(IAppConfig pIAppConfig)
        //    => s_AppConfig = pIAppConfig;

        public static string GetProgramFilesX64NsisDir()
            => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + GConst.Nsis.SUBDIR;

        public static string GetProgramFilesX86NsisDir()
            => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + GConst.Nsis.SUBDIR;

        public static string GetUserProgramFilesNsisDir()
            => Environment.GetFolderPath(Environment.SpecialFolder.Programs) + GConst.Nsis.SUBDIR;
        public static string GetMyDocuments()
            => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string GetGeNSISDir()
            => GetMyDocuments() + GConst.GeNSIS.SUBDIR;

        public static string GetGeNSISProjectsDir() => $"{GetGeNSISDir()}\\Projects";

        public static string GetGeNSISScriptsDir() => $"{GetGeNSISDir()}\\Scripts";

        public static string GetGeNSISInstallerssDir() => $"{GetGeNSISDir()}\\Installers";

        internal static string GetNewScriptName(IAppData pAppData)
            => $"{pAppData.AppName}_{pAppData.AppVersion}_{pAppData.AppBuild}.nsi";

        internal static string GetNewProjectName(IAppData pAppData)
            => $"{pAppData.AppName}_{pAppData.AppVersion}_{pAppData.AppBuild}.xml";
    }
}
