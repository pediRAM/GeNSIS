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


using GeNSIS.Core.ViewModels;

namespace GeNSIS.Core.Commands
{
    public class RemoveSelectedFileCommand : ACommand
    {
        public RemoveSelectedFileCommand(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
            => parameter != null;

        public override void Execute(object parameter)
        {
            var fsi = parameter as FileSystemItemVM;
            AppDataViewModel.Files.Remove(fsi);

            if (AppDataViewModel.License != null && AppDataViewModel.License.Path == fsi.Path) AppDataViewModel.License = null;
            if (AppDataViewModel.ExeName != null && AppDataViewModel.ExeName.Path == fsi.Path) AppDataViewModel.ExeName = null;
            if (AppDataViewModel.InstallerIcon == fsi.Path) AppDataViewModel.InstallerIcon = null;
        }
    }
}