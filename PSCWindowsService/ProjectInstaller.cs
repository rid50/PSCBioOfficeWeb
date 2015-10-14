using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace PSCWindowsService
{
    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.StartType = ServiceStartMode.Automatic;
            service.ServiceName = "PSCWindowsService";
            service.Description = "PSC WCF Service that control Fingerprint scanner operations";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
