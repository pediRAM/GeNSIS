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


namespace GeNSIS.Core
{
    using GeNSIS.Core.Commands;
    using GeNSIS.Core.Models;
    using System.ComponentModel;
    using System.Windows.Input;

    public class ProjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Note = string.Empty;
        private AppDataViewModel m_AppData = new AppDataViewModel(true);

        public string Version { get; set; } = AsmConst.MODEL_VERSION;

        public string Note
        {
            get { return m_Note; }
            set
            {
                if (value == m_Note) return;
                m_Note = value;
                NotifyPropertyChanged(nameof(Note));
            }
        }

        public AppDataViewModel AppData
        {
            get { return m_AppData; }
            set
            {
                if (value == m_AppData) return;
                m_AppData = value;
                NotifyPropertyChanged(nameof(AppData));
            }
        }

        public Project ToModel()
        {
            return new Project
            {
                Version = Version,
                Note = Note,
                AppData = AppData.ToModel(),
            };
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
    }
}
