using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data.Services;
using System.Web.Routing;
using System.Runtime.Caching;
using BioProcessor;

namespace WCFService
{
    public class Global : System.Web.HttpApplication
    {
        //IList<string> licensesMain = null;
        //IList<string> licensesBss = null;

        protected void Application_Start(object sender, EventArgs e)
        {
            //String _path = String.Concat(System.Environment.GetEnvironmentVariable("PATH"), ";",
            //                             System.AppDomain.CurrentDomain.RelativeSearchPath);
            //System.Environment.SetEnvironmentVariable("PATH", _path, EnvironmentVariableTarget.Process);
            //String _path = System.Environment.GetEnvironmentVariable("PATH");
            try
            {
                //context.Server.MapPath("/");
                //throw new Exception(context.Server.MapPath("/"));
                //throw new Exception(HttpRuntime.AppDomainAppPath);
                //throw new Exception(System.Web.Hosting.HostingEnvironment.MapPath(HttpRuntime.AppDomainAppVirtualPath));
                //throw new Exception(System.Web.Hosting.HostingEnvironment.MapPath(HttpRuntime.AppDomainAppPath));

                //Helpers.ObtainLicenses(licensesMain, context.Server.MapPath(""));

                Helpers.ObtainLicenses();

                //Helpers.ObtainLicenses(licensesMain, context.Server.MapPath("/"));

                //try
                //{
                //    Helpers.ObtainLicenses(licensesBss, context.Server.MapPath(""));
                //    //Helpers.ObtainLicenses(licensesBss, context.Server.MapPath("/"));
                //}
                //catch (Exception ex)
                //{
                //    //Console.WriteLine(ex.ToString());
                //    throw new Exception("Error FingersBSS: " + ex.Message);
                //}

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                //throw new Exception("kuku");
                //throw new System.ServiceModel.FaultException("KUKU", 
                throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message, System.ServiceModel.FaultCode.CreateSenderFaultCode("a1", "b1" ));
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
            Helpers.ReleaseLicenses();
            //releaseFingersLicences(licensesMain, licensesBss);
        }

        private void initFingersLicences(HttpContext context, out IList<string> licensesMain, out IList<string> licensesBss)
        {
            //IList<string> licensesMain = new List<string>(new string[] { "FingersExtractor", "FingersMatcher" });
            //IList<string> licensesBss = new List<string>(new string[] { "FingersBSS" });
            licensesMain = new List<string>(new string[] { "FingersExtractor", "FingersMatcher" });
            licensesBss = new List<string>(new string[] { "FingersBSS" });

            try
            {
                //context.Server.MapPath("/");
                //throw new Exception(context.Server.MapPath("/"));
                //throw new Exception(HttpRuntime.AppDomainAppPath);
                //throw new Exception(System.Web.Hosting.HostingEnvironment.MapPath(HttpRuntime.AppDomainAppVirtualPath));
                //throw new Exception(System.Web.Hosting.HostingEnvironment.MapPath(HttpRuntime.AppDomainAppPath));

                //Helpers.ObtainLicenses(licensesMain, context.Server.MapPath(""));

                Helpers.ObtainLicenses();

                //Helpers.ObtainLicenses(licensesMain, context.Server.MapPath("/"));

                //try
                //{
                //    Helpers.ObtainLicenses(licensesBss, context.Server.MapPath(""));
                //    //Helpers.ObtainLicenses(licensesBss, context.Server.MapPath("/"));
                //}
                //catch (Exception ex)
                //{
                //    //Console.WriteLine(ex.ToString());
                //    throw new Exception("Error FingersBSS: " + ex.Message);
                //}

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                //throw new Exception("kuku");
                //throw new System.ServiceModel.FaultException("KUKU", 
                throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message);
            }
            //finally
            //{
            //    //Helpers.ReleaseLicenses(licensesMain);
            //    //Helpers.ReleaseLicenses(licensesBss);
            //}
        }

        private void releaseFingersLicences(IList<string> licensesMain, IList<string> licensesBss)
        {
            Helpers.ReleaseLicenses();
            //Helpers.ReleaseLicenses(licensesBss);
        }

    }
}