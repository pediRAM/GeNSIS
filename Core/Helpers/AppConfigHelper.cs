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
using System.IO;
using System.Text.Json;

namespace GeNSIS.Core.Helpers
{
    internal static class AppConfigHelper
    {
        #region Constants
        public const string FILENAME_CONFIG = "config.json";
        public const string DIR_NSIS = "\\NSIS";
        public const string DIR_GENSIS_PROJECTS = "\\GeNSIS\\Projects";
        public const string DIR_GENSIS_SCRIPTS  = "\\GeNSIS\\NSIS-Scripts";
        public const string DEF_COMPANY_NAME = "ACME";
        public const string DEF_WEBSITE_URL = "https://example.com/";
        #endregion Constants

        public static bool AppConfigFileExists()
            => File.Exists(FILENAME_CONFIG);

        public static AppConfig ReadConfigFile()
        {
            var jsonString = File.ReadAllText(FILENAME_CONFIG, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize<AppConfig>(jsonString);
        }

        public static void WriteConfigFile(AppConfig pAppConfig)
        {
            string jsonString = JsonSerializer.Serialize<AppConfig>(pAppConfig, new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(FILENAME_CONFIG, jsonString, System.Text.Encoding.UTF8);
        }

        public static AppConfig CreateConfig()
        {
            var config = new AppConfig();
            var x64Dir = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles)    + DIR_NSIS;
            var x86Dir = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86) + DIR_NSIS;
            var usrDir = Environment.GetFolderPath(Environment.SpecialFolder.Programs)         + DIR_NSIS;
            var prjDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)      + DIR_GENSIS_PROJECTS;
            var scrDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)      + DIR_GENSIS_SCRIPTS;

            // Find and set NSIS directory.
            if (Directory.Exists(x64Dir))
                config.NsisInstallationDirectory = x64Dir;
            else if (Directory.Exists(x86Dir))
                config.NsisInstallationDirectory = x86Dir;
            else if (Directory.Exists(usrDir))
                config.NsisInstallationDirectory = usrDir;

            // Create GeNSIS projects directory, if not exists!
            if (!Directory.Exists(prjDir))
                Directory.CreateDirectory(prjDir);
            config.GeNSISProjectsDirectory = prjDir;

            // Create GeNSIS nsis-script-output directory, if not exists!
            if (!Directory.Exists(scrDir))
                Directory.CreateDirectory(scrDir);
            config.ScriptsDirectory = scrDir;

            config.CompanyName = "ACME";
            config.Publisher = Environment.UserName;
            return config;
        }
    }
}
