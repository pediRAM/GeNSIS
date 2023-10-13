namespace GeNSIS.Core.TextGenerators
{
    using GeNSIS.Core.Extensions;
    using GeNSIS.Core.Interfaces;
    using System.IO;


    /*
     * Contains methods for adding/removing firewall rules.
     */
    public partial class NsisGenerator : ITextGenerator
    {
        #region Firewall Rules

        #region Add FW Rule
        private void AddAllFirewallRules()
        {
            string filename = Path.GetFileNameWithoutExtension(m_AppData.ExeName.Name);

            foreach (IFirewallRule fwr in m_AppData.GetFirewallRules())
            {
                AddComment(GetCommentOpenFirewallRule(fwr));
                AddFirewallRule(filename, fwr);
            }
        }

        private void AddFirewallRule(string pAppName, IFirewallRule fwr)
        {
            switch (fwr.ProtocolType)
            {
                case Enums.EProtocolType.TCP:
                Add(GetCommandAddTcpFirewallRule(pAppName, fwr));
                break;

                case Enums.EProtocolType.UDP:
                Add(GetCommandAddUdpFirewallRule(pAppName, fwr));
                break;

                case Enums.EProtocolType.Both:
                Add(GetCommandAddTcpFirewallRule(pAppName, fwr));
                Add(GetCommandAddUdpFirewallRule(pAppName, fwr));
                break;
            }
        }

        private string GetCommandAddTcpFirewallRule(string pAppName, IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallAddRule(pAppName, "TCP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallAddRule(pAppName, "TCP", fwr.Port);
        }

        private string GetCommandAddUdpFirewallRule(string pAppName, IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallAddRule(pAppName, "UDP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallAddRule(pAppName, "UDP", fwr.Port);
        }

        private string GetFirewallAddRule(string pAppName, string pProtocol, int pPortFrom, int pPortTo)
            => $"ExecWait 'netsh advfirewall firewall add rule name=\"{pAppName}: Open {pProtocol.ToUpper()} Ports {pPortFrom}-{pPortTo}\" dir=in action=allow protocol={pProtocol.ToUpper()} localport={pPortFrom}-{pPortTo}'";

        private string GetFirewallAddRule(string pAppName, string pProtocol, int pPort)
            => $"ExecWait 'netsh advfirewall firewall add rule name=\"{pAppName}: Open {pProtocol.ToUpper()} Port {pPort}\" dir=in action=allow protocol={pProtocol.ToUpper()} localport={pPort}'";

        private string GetCommentOpenFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return $"Open {fwr.ProtocolType.GetDisplayName()} ports from {fwr.Port} to {fwr.ToPort}.";
            else
                return $"Open {fwr.ProtocolType.GetDisplayName()} port {fwr.Port}.";
        }
        #endregion Add FW Rule


        #region Delete FW Rule
        private void AddDeleteAllFirewallRules()
        {
            foreach (IFirewallRule fwr in m_AppData.GetFirewallRules())
            {
                AddComment(GetCommentCloseFirewallRule(fwr));
                AddDeleteFirewallRule(fwr);
            }
        }

        private void AddDeleteFirewallRule(IFirewallRule fwr)
        {
            switch (fwr.ProtocolType)
            {
                case Enums.EProtocolType.TCP:
                Add(GetCommandDeleteTcpFirewallRule(fwr));
                break;

                case Enums.EProtocolType.UDP:
                Add(GetCommandDeleteUdpFirewallRule(fwr));
                break;

                case Enums.EProtocolType.Both:
                Add(GetCommandDeleteTcpFirewallRule(fwr));
                Add(GetCommandDeleteUdpFirewallRule(fwr));
                break;
            }
        }

        private string GetCommandDeleteTcpFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallDeleteRule("TCP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallDeleteRule("TCP", fwr.Port);
        }

        private string GetCommandDeleteUdpFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return GetFirewallDeleteRule("UDP", fwr.Port, fwr.ToPort);
            else
                return GetFirewallDeleteRule("UDP", fwr.Port);
        }

        private string GetFirewallDeleteRule(string pProtocol, int pPortFrom, int pPortTo)
            => $"ExecWait 'netsh advfirewall firewall delete rule name=\"Open {pProtocol.ToUpper()} Ports {pPortFrom}-{pPortTo}\"'";

        private string GetFirewallDeleteRule(string pProtocol, int pPort)
            => $"ExecWait 'netsh advfirewall firewall delete rule name=\"Open {pProtocol.ToUpper()} Port {pPort}\"'";

        private string GetCommentCloseFirewallRule(IFirewallRule fwr)
        {
            if (fwr.IsRange)
                return $"Close {fwr.ProtocolType.GetDisplayName()} ports from {fwr.Port} to {fwr.ToPort}.";
            else
                return $"Close {fwr.ProtocolType.GetDisplayName()} port {fwr.Port}.";
        }
        #endregion Delete FW Rule

        #endregion Firewall Rules
    }
}
