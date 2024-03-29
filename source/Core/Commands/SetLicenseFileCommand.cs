﻿/*
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
using System;
using System.IO;

namespace GeNSIS.Core.Commands
{
    public class SetLicenseFileCommand : ACommand
    {
        public SetLicenseFileCommand(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CanExecute called with parameter: {parameter}");
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            if (parameter == null) 
                return false;

            var fsi = (parameter as FileSystemItemVM);
            if (fsi.FSType != Enums.EFileSystemType.File)
                return false;

            var fileExtension = Path.GetExtension(fsi.Name);
            
            return 
                fileExtension.Equals(".rtf", System.StringComparison.OrdinalIgnoreCase) ||
                fileExtension.Equals(".txt", System.StringComparison.OrdinalIgnoreCase);
        }

        public override void Execute(object parameter)
        {
            AppDataViewModel.License = (FileSystemItemVM)parameter;
        }
    }
}

