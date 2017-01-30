using System;
using Neurotec.Licensing;

namespace BiometricService
{
    public class Global : System.Web.HttpApplication
    {
        //const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatching,Devices.FingerScanners,Images.WSQ";
        const string Components = "Biometrics.FingerExtraction,Biometrics.FingerMatchingFast,Images.WSQ";

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                //System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath("App_Data/log.txt"), true);
                foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    //sw.WriteLine("bio: " + component);
                    if (!NLicense.IsComponentActivated(component))
                    {
                        //sw.WriteLine("bio2: " + component);
                        if (!NLicense.ObtainComponents("/local", "5000", component))
                            if (component.Equals("Biometrics.FingerMatchingFast"))
                                NLicense.ObtainComponents("/local", "5000", "Biometrics.FingerMatching");
                    }
                }

                //sw.WriteLine("bio:=======================================================");
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