using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using BioProcessor;

namespace biooffice
{
    public class Global : System.Web.HttpApplication
    {

        //protected void Application_Start(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //context.Server.MapPath("/");
        //        //throw new Exception(context.Server.MapPath("/"));
        //        //throw new Exception(HttpRuntime.AppDomainAppPath);
        //        //throw new Exception(System.Web.Hosting.HostingEnvironment.MapPath(HttpRuntime.AppDomainAppVirtualPath));
        //        //throw new Exception(System.Web.Hosting.HostingEnvironment.MapPath(HttpRuntime.AppDomainAppPath));

        //        //Helpers.ObtainLicenses(licensesMain, context.Server.MapPath(""));

        //        Helpers.ObtainLicenses();

        //        //Helpers.ObtainLicenses(licensesMain, context.Server.MapPath("/"));

        //        //try
        //        //{
        //        //    Helpers.ObtainLicenses(licensesBss, context.Server.MapPath(""));
        //        //    //Helpers.ObtainLicenses(licensesBss, context.Server.MapPath("/"));
        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    //Console.WriteLine(ex.ToString());
        //        //    throw new Exception("Error FingersBSS: " + ex.Message);
        //        //}

        //        //Application.EnableVisualStyles();
        //        //Application.SetCompatibleTextRenderingDefault(false);
        //        //Application.Run(new Form1());
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw new Exception("kuku");
        //        //throw new System.ServiceModel.FaultException("KUKU", 
        //        throw new System.ServiceModel.FaultException("Error FingersExtractor, FingersMatcher: " + ex.Message);
        //    }
        //    //finally
        //    //{
        //    //    //Helpers.ReleaseLicenses(licensesMain);
        //    //    //Helpers.ReleaseLicenses(licensesBss);
        //    //}
        //}




/*
        IList<string> licensesMain = null;
        IList<string> licensesBss = null;

        protected void Application_Start(object sender, EventArgs e)
        {
            initFingersLicences(HttpContext.Current, out licensesMain, out licensesBss);
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

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            releaseFingersLicences(licensesMain, licensesBss);
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

                Helpers.ObtainLicenses(licensesMain, context.Server.MapPath("/"));

                try
                {
                    Helpers.ObtainLicenses(licensesBss, context.Server.MapPath("/"));
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    throw new Exception("Error FingersBSS: " + ex.Message);
                }

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                throw new Exception("Error FingersExtractor, FingersMatcher: " + ex.Message);
            }
            finally
            {
                //Helpers.ReleaseLicenses(licensesMain);
                //Helpers.ReleaseLicenses(licensesBss);
            }
        }

        private void releaseFingersLicences(IList<string> licensesMain, IList<string> licensesBss)
        {
            Helpers.ReleaseLicenses(licensesMain);
            Helpers.ReleaseLicenses(licensesBss);
        }
*/
    }
}