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


using GeNSIS.Core.Enums;
using GeNSIS.Core.Extensions;
using GeNSIS.Core.Helpers;

namespace GeNSIS.Core.Commands
{
    public abstract class AbstractAddFirewallRules : ACommand
    {
        public AbstractAddFirewallRules(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }

        protected abstract EProtocolType ProtocolType { get; }
        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter) => AppDataViewModel.FirewallRules.AddRange(FirewallRuleHelper.ParseFirewallRules(parameter as string, ProtocolType));
    }

    public class AddTcpFWRules : AbstractAddFirewallRules
    {
        public AddTcpFWRules(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }
        protected override EProtocolType ProtocolType => EProtocolType.TCP;
    }

    public class AddUdpFWRules : AbstractAddFirewallRules
    {
        public AddUdpFWRules(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }
        protected override EProtocolType ProtocolType => EProtocolType.UDP;
    }

    public class AddTcpUdpFWRules : AbstractAddFirewallRules
    {
        public AddTcpUdpFWRules(AppDataVM pAppDataViewModel) : base(pAppDataViewModel) { }
        protected override EProtocolType ProtocolType => EProtocolType.Both;
    }
}
