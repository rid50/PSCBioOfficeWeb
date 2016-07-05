using System;
using Neurotec.Licensing;

namespace AppFabricCacheService
{
    public class Global : System.Web.HttpApplication
    {
        //const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ";
        const string Components = "Biometrics.FingerMatchingFast";

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("App_Data/log.txt"), true);
                foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //System.Diagnostics.EventLog.WriteEntry("BiometricService", "bio: " + component);
                    //sw.WriteLine("app: " + component);
                    if (!NLicense.IsComponentActivated(component))
                    {
                        //System.Diagnostics.EventLog.WriteEntry("BiometricService", "bio2: " + component);
                        //sw.WriteLine("app2: " + component);
                        NLicense.ObtainComponents("/local", "5000", component);
                    }
                }

                //System.Diagnostics.EventLog.WriteEntry("BiometricService", "bio:=======================================================");

                //sw.WriteLine("app:=======================================================");
                //sw.Close();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message, System.ServiceModel.FaultCode.CreateSenderFaultCode("a1", "b1"));
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            throw new System.ServiceModel.FaultException(exc.Message);

            //Response.Write(exc.Message);
            //Server.ClearError();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            NLicense.ReleaseComponents(Components);
            //releaseFingersLicences(licensesMain, licensesBss);
        }
    }
}