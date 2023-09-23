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

namespace GeNSIS.Core.Models
{
    using GeNSIS.Core.Interfaces;
    using GeNSIS.Core.ViewModels;
    using System;


    public class ServiceData : IServiceData, ICloneable
    {
        public ServiceData() { }

        public ServiceData(IServiceData pServiceData) : this()
            => UpdateValues(pServiceData);

        public string ServiceName { get; set; }

        public string DisplayName { get; set; }

        public bool IsAutoStart { get; set; } = true;

        public string User { get; set; }

        public string Password { get; set; }

        public string Dependencies { get; set; }

        public object Clone()
        {
            // todo: check and review Clone() function!
            return new ServiceData
            {
                ServiceName = ServiceName,
                DisplayName = DisplayName,
                IsAutoStart = IsAutoStart,
                User = User,
                Password = Password,
                Dependencies = Dependencies,
            };
        }

        public ServiceDataVM ToViewModel(IServiceData pServiceData)
        {
            return new ServiceDataVM
            {
                ServiceName = pServiceData.ServiceName,
                DisplayName = pServiceData.DisplayName,
                IsAutoStart = pServiceData.IsAutoStart,
                User = pServiceData.User,
                Password = pServiceData.Password,
                Dependencies = pServiceData.Dependencies,
            };
        }

        public void UpdateValues(IServiceData pServiceData)
        {
            ServiceName = pServiceData.ServiceName;
            DisplayName = pServiceData.DisplayName;
            IsAutoStart = pServiceData.IsAutoStart;
            User = pServiceData.User;
            Password = pServiceData.Password;
            Dependencies = pServiceData.Dependencies;
        }

    }
}
