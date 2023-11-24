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


namespace GeNSIS.Core
{
    internal static class GConst
    {
        public const int MAX_LAST_FILES = 10;

        public const string FLAG_PATH = "Resources/Icons/Flags";
        
        public static class Nsis
        {
            public const string INSTALLER_ICON = @"\Contrib\Graphics\Icons\nsis3-install.ico";
            public const string UNINSTALLER_ICON = @"\Contrib\Graphics\Icons\nsis3-uninstall.ico";
            public const string INSTALLER_WIZARD_IMG = @"\Contrib\Graphics\Wizard\win.bmp";
            public const string UNINSTALLER_WIZARD_IMG = @"\Contrib\Graphics\Wizard\win.bmp";
            
            public const string SUBDIR = "\\NSIS";
            public const string SUBDIR_NSIS_ICONS = "\\Contrib\\Graphics\\Icons";
            public const string SUBDIR_NSIS_HEADER_IMAGES = "\\Contrib\\Graphics\\Header";
            public const string SUBDIR_NSIS_WIZARD_IMAGES = "\\Contrib\\Graphics\\Wizard";

            public const string BASE_DIR = @"BASE_DIR";

        }

        public static class GeNSIS
        {
            public const string SUBDIR = "\\GeNSIS";
            public const string SUBDIR_PROJECTS   = $"{SUBDIR}\\Projects";
            public const string SUBDIR_SCRIPTS    = $"{SUBDIR}\\Scripts";
            public const string SUBDIR_INSTALLERS = $"{SUBDIR}\\Installers";
            public const string SUBDIR_DESIGNS    = $"{SUBDIR}\\Designs";
            public const string SUBDIR_LANG       = $"{SUBDIR}\\Translations";

            public const string FILENAME_CONFIG = "config.json";
        }

        public static class Default
        {
            public const string EXTERNAL_EDITOR = "notepad.exe";
            public const string COMPANY_NAME = "ACME";
            public const string WEBSITE_URL = "https://example.com/";
            public const string INSTALLER_FILENAME = @"Setup ${APP_NAME} ${APP_VERSION} ${APP_BUILD}.exe";// ${APP_MACHINE_TYPE}_${APP_ARCH}.exe";

            public const int EDITOR_FONT_SIZE = 12;
        }

        public static class Editor
        {
            public const int MIN_FONT_SIZE = 8;
            public const int MAX_FONT_SIZE = 64;
        }
    }
}
