using System.ServiceProcess;
using System.ServiceModel;

namespace PSCWindowsService
{
    class WindowsService : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public WindowsService()
        {
            // Name the Windows Service
            ServiceName = "PSCWindowsService";

        }

        public static void Main()
        {
            ServiceBase.Run(new WindowsService());
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(CommandServices));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
