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
using GeNSIS.Core.ViewModels;
using System;
using System.Collections.Generic;

namespace GeNSIS.Core.Helpers
{
    public static class FirewallRuleHelper
    {
        public static List<FirewallRuleVM> ParseFirewallRules(string pRulesString, EProtocolType pPrototolType)
        {
            if (string.IsNullOrWhiteSpace(pRulesString)) 
                return null;

            var firewallRules = new List<FirewallRuleVM>();
            string[] tokens = pRulesString.Split(new char[] { ' ', ';', ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach(var t in tokens)
                if (TryParseToken(pPrototolType, t, out var firewallRule))
                    firewallRules.Add(firewallRule);

            return firewallRules;
        }

        public static bool TryParseToken(EProtocolType pProtocolType, string pToken, out FirewallRuleVM pFirewallRule)
        {
            pFirewallRule = null;

            if (string.IsNullOrWhiteSpace(pToken)) 
                return false;

            // IP Range?
            if (pToken.Contains('-'))
            {
                string[] tokens = pToken.Split(new char[] { '-' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 2) 
                    return false;

                return TryParseRange(pProtocolType, tokens[0], tokens[1], out pFirewallRule);
            }
            else
            {
                if (int.TryParse(pToken, out int pResult))
                {
                    pFirewallRule = new FirewallRuleVM { ProtocolType = pProtocolType, Port = pResult };
                    return true;
                }
                return false;
            }
        }

        public static bool TryParseRange(EProtocolType pProtocolType, string pFromIP, string pToIP, out FirewallRuleVM pFirewallRule)
        {
            pFirewallRule = null;

            if (int.TryParse(pFromIP, out int fromIP) && int.TryParse(pToIP, out int toIP))
            {
                pFirewallRule = new FirewallRuleVM
                {
                    ProtocolType = pProtocolType,
                    IsRange = fromIP != toIP,
                    Port = Math.Min(fromIP, toIP),
                    ToPort = Math.Max(fromIP, toIP)                        
                };
                return true;
            }

            return false;
        }
    }
}
