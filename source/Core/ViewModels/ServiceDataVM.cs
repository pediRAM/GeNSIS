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

namespace GeNSIS.Core.ViewModels
{
    using GeNSIS.Core.Enums;
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.Models;
    using System;
    using System.ComponentModel;


    public class ServiceDataVM : IServiceData, INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private EServiceStartType m_StartType;
        private string m_ServiceName;
        private string m_DisplayName;
        private string m_User;
        private string m_Password;
        private string m_Dependencies;
        public ServiceDataVM() { }

        public ServiceDataVM(IServiceData pServiceData) : this()
            => UpdateValues(pServiceData);

        public string ServiceName
        {
            get { return m_ServiceName; }
            set
            {
                if (value == m_ServiceName) return;
                m_ServiceName = value;
                NotifyPropertyChanged(nameof(ServiceName));
            }
        }

        public string DisplayName
        {
            get { return m_DisplayName; }
            set
            {
                if (value == m_DisplayName) return;
                m_DisplayName = value;
                NotifyPropertyChanged(nameof(DisplayName));
            }
        }

        public EServiceStartType StartType
        {
            get { return m_StartType; }
            set
            {
                if (value == m_StartType) return;
                m_StartType = value;
                NotifyPropertyChanged(nameof(StartType));
            }
        }

        public string User
        {
            get { return m_User; }
            set
            {
                if (value == m_User) return;
                m_User = value;
                NotifyPropertyChanged(nameof(User));
            }
        }

        public string Password
        {
            get { return m_Password; }
            set
            {
                if (value == m_Password) return;
                m_Password = value;
                NotifyPropertyChanged(nameof(Password));
            }
        }

        public string Dependencies
        {
            get { return m_Dependencies; }
            set
            {
                if (value == m_Dependencies) return;
                m_Dependencies = value;
                NotifyPropertyChanged(nameof(Dependencies));
            }
        }

        public object Clone()
        {
            // todo: check and review Clone() function!
            return new ServiceDataVM
            {
                ServiceName = ServiceName,
                DisplayName = DisplayName,
                StartType = StartType,
                User = User,
                Password = Password,
                Dependencies = Dependencies,
            };
        }

        public ServiceData ToModel(IServiceData pServiceData)
        {
            return new ServiceData
            {
                ServiceName = pServiceData.ServiceName,
                DisplayName = pServiceData.DisplayName,
                StartType = pServiceData.StartType,
                User = pServiceData.User,
                Password = pServiceData.Password,
                Dependencies = pServiceData.Dependencies,
            };
        }

        public void UpdateValues(IServiceData pServiceData)
        {
            ServiceName = pServiceData.ServiceName;
            DisplayName = pServiceData.DisplayName;
            StartType = pServiceData.StartType;
            User = pServiceData.User;
            Password = pServiceData.Password;
            Dependencies = pServiceData.Dependencies;
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}
