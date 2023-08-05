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


namespace GeNSIS.Core
{
    using GeNSIS.Core.Models;
    using System.ComponentModel;

    public class ProjectVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string m_Note = string.Empty;
        private AppDataVM m_AppData = new AppDataVM(true);

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

        public AppDataVM AppData
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
                //Note = Note,
                AppData = AppData.ToModel(),
            };
        }

        private void NotifyPropertyChanged(string pPropertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(pPropertyName)));
    }
}
