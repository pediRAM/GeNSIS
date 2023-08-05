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


using GeNSIS.Core.Enums;
using GeNSIS.Core.Interfaces;
using GeNSIS.Core.Models;
using System.ComponentModel;

namespace GeNSIS.Core.ViewModels
{
    public class FirewallRuleVM : IFirewallRule, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool m_IsRange;
        private EProtocolType m_ProtocolType = EProtocolType.TCP;
        private int m_IP;
        private int m_ToIP;

        public FirewallRuleVM() { }

        public FirewallRuleVM(IFirewallRule pFirewallRule) : this()
            => UpdateValues(pFirewallRule);

        public EProtocolType ProtocolType
        {
            get { return m_ProtocolType; }
            set
            {
                if (value == m_ProtocolType) return;
                m_ProtocolType = value;
                NotifyPropertyChanged(nameof(ProtocolType));
            }
        }

        public bool IsRange
        {
            get { return m_IsRange; }
            set
            {
                if (value == m_IsRange) return;
                m_IsRange = value;
                NotifyPropertyChanged(nameof(IsRange));
            }
        }

        public int IP
        {
            get { return m_IP; }
            set
            {
                if (value == m_IP) return;
                m_IP = value;
                NotifyPropertyChanged(nameof(IP));
            }
        }

        public int ToIP
        {
            get { return m_ToIP; }
            set
            {
                if (value == m_ToIP) return;
                m_ToIP = value;
                NotifyPropertyChanged(nameof(ToIP));
            }
        }

        public void UpdateValues(IFirewallRule pFirewallRule)
        {
            ProtocolType = pFirewallRule.ProtocolType;
            IsRange = pFirewallRule.IsRange;
            IP = pFirewallRule.IP;
            ToIP = pFirewallRule.ToIP;
        }

        public FirewallRule ToModel(IFirewallRule pFirewallRule)
        {
            return new FirewallRule
            {
                ProtocolType = pFirewallRule.ProtocolType,
                IsRange = pFirewallRule.IsRange,
                IP = pFirewallRule.IP,
                ToIP = pFirewallRule.ToIP,
            };
        }

        public override string ToString()
        {
            if (IsRange) 
                return $"{ProtocolType}:{IP}-{ToIP}";

            return $"{ProtocolType}:{IP}";
        }

        private void NotifyPropertyChanged(string pPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}
