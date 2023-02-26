﻿/***************************************************************************************
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


using System.IO;
using System.Windows.Controls;

namespace GeNSIS.Core.Commands
{
    class SetLicenseFileCommand : ACommand
    {
        public SetLicenseFileCommand(AppDataViewModel pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
        {
            var listBox = (ListBox)parameter;
            return (listBox != null && listBox.SelectedItem != null && Path.GetExtension(listBox.SelectedItem as string).Equals(".txt", System.StringComparison.OrdinalIgnoreCase));
        }

        public override void Execute(object parameter)
        {
            AppDataViewModel.License = ((ListBox)parameter).SelectedItem as string;
        }
    }
}

