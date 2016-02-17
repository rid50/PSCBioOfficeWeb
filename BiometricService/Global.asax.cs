using System;
using Neurotec.Licensing;

namespace BiometricService
{
    public class Global : System.Web.HttpApplication
    {
        const string Components = "Biometrics.FingerExtractionFast,Biometrics.FingerMatchingFast,Images.WSQ";
        //const string Components = "Images.WSQ";

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                foreach (string component in Components.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    NLicense.ObtainComponents("/local", "5000", component);
                }
            }
            catch (Exception ex)
            {
                throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message, System.ServiceModel.FaultCode.CreateSenderFaultCode("a1", "b1"));
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            throw new System.ServiceModel.FaultException(exc.Message);

            //Response.Write(exc.Message);
            //Server.ClearError();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            NLicense.ReleaseComponents(Components);
            //releaseFingersLicences(licensesMain, licensesBss);
        }
    }
}