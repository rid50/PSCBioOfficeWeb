using Neurotec.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

namespace BiometricService
{
    public class Global : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            const string Components = "Biometrics.FingerExtractionFast,Biometrics.FingerMatching,Images.WSQ";
            try
            {


                if (!System.Diagnostics.EventLog.SourceExists("BiometricService"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("BiometricService", "Application");
                }


                //System.IO.StreamWriter sw = System.IO.File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath("App_Data/log2.txt"));
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath("App_Data/log.txt"), true);

                foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //System.Diagnostics.EventLog.WriteEntry("BiometricService", "bio: " + component);
                    //sw.WriteLine("bio: " + component);
                    if (!NLicense.IsComponentActivated(component))
                    {
                        //System.Diagnostics.EventLog.WriteEntry("BiometricService", "bio2: " + component);
                        //sw.WriteLine("bio2: " + component);
                        NLicense.ObtainComponents("/local", "5000", component);
                    }
                }

                System.Diagnostics.EventLog.WriteEntry("BiometricService", "bio:=======================================================");

                //sw.WriteLine("bio:=======================================================");
                //sw.Close();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message, System.ServiceModel.FaultCode.CreateSenderFaultCode("a1", "b1"));
            }

            ServiceHost host = new ServiceHost(serviceType, baseAddresses);
            return host;
        }
    }
}