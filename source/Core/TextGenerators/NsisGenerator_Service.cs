namespace GeNSIS.Core.TextGenerators
{
    using GeNSIS.Core.Interfaces;


    public partial class NsisGenerator : ITextGenerator
    {
        /* High Priority = 1
        1 sc create MyServiceName binPath= "C:\Path\To\Service.exe" obj= "NT AUTHORITY\LocalSystem" type= own
        2 sc create MyServiceName binPath= "C:\Path\To\Service.exe" obj= "NT AUTHORITY\NetworkService" type= own
        3 sc create MyServiceName binPath= "C:\Path\To\Service.exe" obj= "NT AUTHORITY\LocalService" type= own

        sc create MyServiceName binPath= "C:\Path\To\Service.exe" obj= ".\john.doe" type= own password= YourPasswordHere
        sc create MyServiceName binPath= "C:\Path\To\Service.exe" obj= "xyz\john.doe" type= own password= YourPasswordHere
        */

        private void AddServiceInstall()
        {
            if (!m_AppData.IsService) return;

            AddComment("Stop service if installed and running:");
            Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Stop_Service.bat\"'"); // todo: add ServiceName parameter!
            Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Install_Service.bat\"'"); // todo: add service-data paramerters!
            AddStripline();
            Add();
        }

        private void AddServiceUninstall()
        {
            if (!m_AppData.IsService) return;

            AddComment("Stop service if installed and running:");
            Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Stop_Service.bat\"'"); // todo: add ServiceName parameter!
            Add($"ExecWait '\"$SYSDIR\\sc.exe\" delete {m_AppData.Service.ServiceName}'");
            AddStripline();
            Add();
        }
    }
}
