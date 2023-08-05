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


using System;

namespace GeNSIS.Core.Commands
{
    public class SetIconFileCommand : ACommand
    {
        public SetIconFileCommand(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
            => parameter != null && ((string)parameter).EndsWith(".ico", StringComparison.InvariantCultureIgnoreCase);

        public override void Execute(object parameter)
        {
            AppDataViewModel.InstallerIcon = (string)parameter;
        }
    }
}
