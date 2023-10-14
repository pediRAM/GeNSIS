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
            if (!m_AppData.IsService) return; // m_AppData.Service.UserType

            AddComment("Stop service if installed and running:");
            Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Stop_Service.bat\" \"{m_AppData.Service.ServiceName}\"'"); // todo: add ServiceName parameter!

            if (m_AppData.Service.UserType == Enums.EServiceUserType.SpecificUser)
                Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Install_Service.bat\" \"{m_AppData.Service.ServiceName}\" \"{m_AppData.Service.UserType}\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"{m_AppData.Service.User}\" \"{m_AppData.Service.Password}\"'");
            else
                Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Install_Service.bat\" \"{m_AppData.Service.ServiceName}\" \"{m_AppData.Service.UserType}\" \"$INSTDIR\\${{APP_EXE_NAME}}\" \"NT AUTHORITY\\{m_AppData.Service.UserType}\"'");
        }

        private void AddServiceUninstall()
        {
            if (!m_AppData.IsService) return;

            AddComment("Stop service if installed and running:");
            Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Stop_Service.bat\" \"{m_AppData.Service.ServiceName}\"'");
            Add($"ExecWait '\"$SYSDIR\\sc.exe\" delete {m_AppData.Service.ServiceName}'");
        }

        private void AddStopService()
        {
            if (!m_AppData.IsService) return;

            AddComment("Stop service if installed and running:");
            Add($"ExecWait '\"$SYSDIR\\cmd.exe\" /c \"$INSTDIR\\Stop_Service.bat\" \"{m_AppData.Service.ServiceName}\"'");
        }
    }
}
