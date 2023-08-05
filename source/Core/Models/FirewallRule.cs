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
using GeNSIS.Core.ViewModels;

namespace GeNSIS.Core.Models
{
    public class FirewallRule : IFirewallRule
    {
        public FirewallRule() { }
        public FirewallRule(IFirewallRule pFirewallRule) : this() => UpdateValues(pFirewallRule);

        public EProtocolType ProtocolType { get; set; } = EProtocolType.TCP;
        public bool IsRange { get; set; }
        public int IP { get; set; }
        public int ToIP { get; set; }

        public void UpdateValues(IFirewallRule pFirewallRule)
        {
            ProtocolType = pFirewallRule.ProtocolType;
            IsRange      = pFirewallRule.IsRange;
            IP           = pFirewallRule.IP;
            ToIP         = pFirewallRule.ToIP;
        }

        public FirewallRuleVM ToViewModel()
        {
            return new FirewallRuleVM
            {
                ProtocolType = ProtocolType,
                IsRange      = IsRange,
                IP           = IP,
                ToIP         = ToIP,
            };
        }
    }
}
