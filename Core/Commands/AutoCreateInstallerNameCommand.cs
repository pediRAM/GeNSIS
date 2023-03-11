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

namespace GeNSIS.Core.Commands
{
    internal class AutoCreateInstallerNameCommand : ACommand
    {
        public AutoCreateInstallerNameCommand(AppDataViewModel pAppDataViewModel) : base(pAppDataViewModel) { }
        public override bool CanExecute(object parameter)
            => parameter != null;

        public override void Execute(object parameter)
        {
            var d = AppDataViewModel;
            if (string.IsNullOrWhiteSpace(d.ExeName)) return;
            string build = string.IsNullOrWhiteSpace(d.AppBuild) ? null : $"_{d.AppBuild.Trim()}";
            string arch = d.Is64BitApplication ? "x64" : "x32";
            d.InstallerFileName = $"Setup_{d.AppName}_{d.AppVersion}{build}_{arch}.exe";
        }
    }
}
