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

namespace GeNSIS.Core
{
    public class Validator
    {
        #region Constants
        public const string ERR_EMPTY_APPNAME = "Name of application cannot be empty!";
        public const string HINT_APPNAME = "Put the filename of executable without '.exe', like: 'MyEditor' (from 'MyEditor.exe').";

        public const string ERR_EMPTY_EXENAME = "Name of executable cannot be empty!";
        public const string HINT_EXENAME = "Example for ExeName: MyEditr.exe";

        public const string ERR_EMPTY_ASSOCIATEDEXTENSION = "Extension cannot illegal characters!";
        public const string HINT_ASSOCIATEDEXTENSION = "Leave blank (for no association), or short string containing a-z and 0-9 characters.";

        public const string ERR_EMPTY_APPVERSION = "AppVersion cannot be null, empty or contain illegal characters!";
        public const string HINT_APPVERSION = "Example for AppVersion: 1.0, 1.2.3 or 1.2.3.4";


        public const string ERR_EMPTY_PUBLISHER = "Publisher cannot be empty!";
        public const string HINT_PUBLISHER = "Put something like 'John DOE', 'ACME LTD' or 'XYZ'";

        public const string ERR_EMPTY_FILES = "Files cannot be empty!";
        public const string HINT_FILES = "Add at least one *.exe file.";

        #endregion Constants

        public bool IsValid(AppData p, out ValidationError pError)
        {
            pError = null;

            if (string.IsNullOrWhiteSpace(p.AppName)) { pError = new ValidationError(nameof(AppData.AppName), ERR_EMPTY_APPNAME, HINT_APPNAME); return false; }
            if (string.IsNullOrWhiteSpace(p.ExeName)) { pError = new ValidationError(nameof(AppData.ExeName), ERR_EMPTY_EXENAME, HINT_EXENAME); return false; }
            if (string.IsNullOrWhiteSpace(p.AssociatedExtension)) { pError = new ValidationError(nameof(AppData.AssociatedExtension), ERR_EMPTY_ASSOCIATEDEXTENSION, HINT_ASSOCIATEDEXTENSION); return false; }
            if (string.IsNullOrWhiteSpace(p.AppVersion)) { pError = new ValidationError(nameof(AppData.AppVersion), ERR_EMPTY_APPVERSION, HINT_APPVERSION); return false; }
            /*
            if (string.IsNullOrWhiteSpace(p.AppBuild) { pError = new ValidationError(nameof(AppData.AppBuild), ERR_EMPTY_APPBUILD, HINT_APPBUILD); return false; }
            if (string.IsNullOrWhiteSpace(p.AppIcon) { pError = new ValidationError(nameof(AppData.AppIcon), ERR_EMPTY_APPICON, HINT_APPICON); return false; }
            if (string.IsNullOrWhiteSpace(p.Company) { pError = new ValidationError(nameof(AppData.Company), ERR_EMPTY_COMPANY, HINT_COMPANY); return false; }
            if (string.IsNullOrWhiteSpace(p.License) { pError = new ValidationError(nameof(AppData.License), ERR_EMPTY_LICENSE, HINT_LICENSE); return false; }
            if (string.IsNullOrWhiteSpace(p.Url) { pError = new ValidationError(nameof(AppData.Url), ERR_EMPTY_URL, HINT_URL); return false; }
            */
            if (string.IsNullOrWhiteSpace(p.Publisher)) { pError = new ValidationError(nameof(AppData.Publisher), ERR_EMPTY_PUBLISHER, HINT_PUBLISHER); return false; }


            
            if (p.Files.Count == 0) { pError = new ValidationError(nameof(AppData.Files), ERR_EMPTY_FILES, HINT_FILES); return false; }
            return true;

        }
    }
}
