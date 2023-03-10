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
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GeNSIS.Core.Commands
{
    public class AutoRetrieveExeDataCommand : ACommand
    {
        public AutoRetrieveExeDataCommand(AppDataViewModel pAppDataViewModel) : base(pAppDataViewModel) { }

        public override bool CanExecute(object parameter)
            => !string.IsNullOrEmpty(AppDataViewModel.ExeName);

        public override void Execute(object parameter)
        {
            try
            {
                var d = AppDataViewModel;                
                var info = ExeInfoHelper.GetExeData(d.ExeName);
                d.Is64BitApplication = info.IsX64;
                d.AppName = Path.GetFileNameWithoutExtension(d.ExeName);

                if (info.Version.EndsWith(".0.0"))
                    d.AppVersion = info.Version.Substring(0, info.Version.Length - 4);
                else if (info.Version.EndsWith(".0"))
                    d.AppVersion = info.Version.Substring(0, info.Version.Length - 2);
                else
                    d.AppVersion = info.Version;

                var fvi = FileVersionInfo.GetVersionInfo(d.ExeName);
                if (!string.IsNullOrWhiteSpace(fvi.CompanyName))
                {
                    d.Company = fvi.CompanyName;
                    d.Publisher = fvi.CompanyName;
                }
            }
            catch(Exception ex)
            {
                _ = Dispatcher.CurrentDispatcher.InvokeAsync(new Action(() => { MessageBox.Show(ex.ToString(), "Error!"); }));
            }
        }
    }
}
