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

using GeNSIS.Core.Helpers;

namespace GeNSIS.Core.Commands
{
    internal class AutoCreateInstallerNameCommand : ACommand
    {
        public AutoCreateInstallerNameCommand(AppDataViewModel pAppDataViewModel) : base(pAppDataViewModel) { }
        public override bool CanExecute(object parameter)
            => !string.IsNullOrWhiteSpace((string)parameter);

        public override void Execute(object parameter)
            => ExeInfoHelper.AutoNameInstallerExe(AppDataViewModel);
    }
}
