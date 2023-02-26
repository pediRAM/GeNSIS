/***************************************************************************************
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


using System.Windows.Controls;

namespace GeNSIS.Core.Commands
{
    class RemoveSelectedFileCommand : ACommand
    {
        public RemoveSelectedFileCommand(AppDataViewModel pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
        {
            var listBox = (ListBox)parameter;
            return (listBox != null && listBox.SelectedItem != null);
        }

        public override void Execute(object parameter)
        {
            var listBox = (ListBox)parameter;
            AppDataViewModel.Files.Remove(listBox.SelectedItem as string);
        }
    }
}