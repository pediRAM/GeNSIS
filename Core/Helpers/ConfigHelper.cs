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

using NLog;
using GeNSIS.Core.Models;
using System;
using System.IO;
using System.Text.Json;

namespace GeNSIS.Core.Helpers
{
    internal static class ConfigHelper
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static Config s_AppConfig;

        public static Config GetAppConfig() => s_AppConfig;
        public static string GetNsisInstallationFoler() => s_AppConfig?.NsisInstallationDirectory;
        public static string GetNsisIconsFolder() => s_AppConfig?.NsisInstallationDirectory + GConst.Nsis.SUBDIR_NSIS_ICONS;
        public static string GetNsisWizardImagesFolder() => s_AppConfig?.NsisInstallationDirectory + GConst.Nsis.SUBDIR_NSIS_WIZARD_IMAGES;
        public static string GetNsisHeaderImagesFolder() => s_AppConfig?.NsisInstallationDirectory + GConst.Nsis.SUBDIR_NSIS_HEADER_IMAGES;
        public static bool AppConfigFileExists() => File.Exists(GConst.GeNSIS.FILENAME_CONFIG);

        public static Config ReadConfigFile()
        {
            var jsonString = File.ReadAllText(GConst.GeNSIS.FILENAME_CONFIG, System.Text.Encoding.UTF8);
            s_AppConfig = JsonSerializer.Deserialize<Config>(jsonString);
            return s_AppConfig;
        }

        public static void WriteConfigFile(Config pAppConfig)
        {
            string jsonString = JsonSerializer.Serialize<Config>(pAppConfig, new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(GConst.GeNSIS.FILENAME_CONFIG, jsonString, System.Text.Encoding.UTF8);
        }

        public static Config CreateConfig()
        {
            var config = new Config();
            config.NsisInstallationDirectory = GetNsisInstallationDirectoryOrNull();

            CreateGeNSISDirectoriesIfNotExist();

            config.GeNSISProjectsDirectory = PathHelper.GetGeNSISProjectsDir();
            config.ScriptsDirectory = PathHelper.GetGeNSISScriptsDir();
            config.InstallersDirectory = PathHelper.GetGeNSISInstallerssDir();

            config.CompanyName = GConst.Default.COMPANY_NAME;
            config.Publisher = Environment.UserName;
            config.Website = GConst.Default.WEBSITE_URL;

            s_AppConfig = config;
            return config;
        }

        /// <summary>
        /// Returns the installation directory of NSIS if found, else NULL.
        /// </summary>
        /// <returns>Path to NSIS install dir, or NULL.</returns>
        public static string GetNsisInstallationDirectoryOrNull()
        {
            if (Directory.Exists(PathHelper.GetProgramFilesX64NsisDir()))
            {
                Log?.Debug("Found NSIS directory in 64 bit Program Files.");
                return PathHelper.GetProgramFilesX64NsisDir();
            }
            else if (Directory.Exists(PathHelper.GetProgramFilesX86NsisDir()))
            {
                Log?.Debug("Found NSIS directory in 32 bit Program Files (x86).");
                return PathHelper.GetProgramFilesX86NsisDir();
            }
            else if (Directory.Exists(PathHelper.GetUserProgramFilesNsisDir()))
            {
                Log?.Debug("Found NSIS in users application directory.");
                return PathHelper.GetUserProgramFilesNsisDir();
            }
            else
            {
                Log?.Warn("NSIS installation directory not found!");
                return null;
            }
        }

        /// <summary>
        /// Creates GeNSIS directories (Projects, Scripts, Installers) if they do not exist.
        /// </summary>
        public static void CreateGeNSISDirectoriesIfNotExist()
        {
            CreateGeNsisDirectory(PathHelper.GetGeNSISInstallerssDir());
            CreateGeNsisDirectory(PathHelper.GetGeNSISProjectsDir());
            CreateGeNsisDirectory(PathHelper.GetGeNSISScriptsDir());
        }

        private static void CreateGeNsisDirectory(string pPath)
        {
            if (!Directory.Exists(pPath))
            {
                Log?.Debug($"Creating dir: {pPath}...");
                Directory.CreateDirectory(pPath);
            }
        }
    }
}
