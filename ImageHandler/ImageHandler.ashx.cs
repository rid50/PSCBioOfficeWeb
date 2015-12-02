using System;
using System.Collections.Generic;
using System.Web;
//using System.Configuration;
using System.Collections;
using System.Web.SessionState;
//using DataService;
//using BioProcessor;
using ImageHandler.CacheService;
using System.Web.Script.Serialization;
//using ImageHandler;
//using Microsoft.ApplicationServer.Caching;

//using System.Drawing;
//using System.Drawing.Imaging;
//using gx;

namespace WebHandlers
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class ImageHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return true; } }

        private static readonly System.Object lockThis = new System.Object();

        //private static IList<string> licensesMain = null;
        //private static IList<string> licensesBss = null;

        //System.IO.StreamWriter file = null;
/*
        public ImageHandler() : base()
        {
            
            //System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("\\BioProcessorLog.txt"), "ctor");

            if (licensesMain == null)
            {
            //    file = new System.IO.StreamWriter(HttpContext.Current.Server.MapPath("\\BioProcessorLog.txt"), true);
            //    file.WriteLine("wsq: ctor");
            //    file.Flush();

                initFingersLicences(HttpContext.Current);
            }
        }

        ~ImageHandler()
        {
            //throw new Exception("destructor");

            if (licensesMain != null)
            {
                releaseFingersLicences();
                licensesMain = null;
            }
            //file.WriteLine("wsq: dtor");
            //file.Flush();
            //file.Close();
        }
 */       
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "application/xop+xml";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);

            if (context.Request.QueryString["getSessionState"] != null)
            {
                //object ses = context.Application["myerror"];
                if (context.Application["myerror"] != null)
                {
                    context.Response.Write(context.Application["myerror"]);
                    //context.Response.End();
                    //context.Application.Remove("myerror");
                    if (context.Request.QueryString["clearError"] != null)
                        context.Application["myerror"] = null;
                }
                return;
            }

            var cache = new MemoryCacheServiceClient();

            if (context.Request.QueryString["clearSession"] != null)
            {
                try
                {
                    cache.SetDirty();
                }
                catch (System.ServiceModel.FaultException ex)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    context.Response.Write(serializer.Serialize(ex.Message));
                }
                catch (Exception ex)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    context.Response.Write(serializer.Serialize(ex.Message));
                }
                //MemoryCache.Default.Remove("fingersCollection");
                //MemoryCache.Default.Remove("id");

                //context.Application.Remove("fingersCollection");
                //context.Application.Remove("id");
                return;
            }
            //context.Response.Cache.SetAllowResponseInBrowserHistory(false);

            //System.Object lockThis = new System.Object();

            //initFingersLicences(context);

            //_context = context;
            //_fingersCollectionList = new Dictionary<int, byte[]>();
//            IList<string> licensesMain = null;
//            IList<string> licensesBss = null;
            string id = context.Request.QueryString["id"];
            try {
                Int32.Parse(id);
            } catch (Exception) {
                context.Application["myerror"] = "Please enter a valid ID";
                throw new Exception("Please enter a valid ID");
            }

            string wsqQuery = context.Request.QueryString["wsq"];

            byte[] buffer = new byte[0];

//            var cache = new MemoryCacheServiceClient();

/*
            //file.WriteLine("wsq: " + wsqQuery);
            //file.Flush();

            DataSource ds = null;

            //if (System.Configuration.ConfigurationManager.AppSettings["Enroll"] == "db")
            if (getAppSetting("Enroll") == "db")
                ds = new DatabaseService();
            else
            {
                context.Response.BinaryWrite(new byte[1]);
                context.Response.End();
                return;
            }
*/
            if (wsqQuery == null)
            {
                try
                {
                    buffer = cache.GetPicture(id);
                    //buffer = ds.GetImage(IMAGE_TYPE.picture, Convert.ToInt32(id));
                    context.Response.BinaryWrite(buffer);
                    //context.Response.End();
                }
                catch (Exception ex)
                {
                    context.Application["myerror"] = ex.Message;
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                lock (lockThis)
                {
                    Dictionary<string, int> dict = new Dictionary<string, int>();
                    dict.Add("li", 0);
                    dict.Add("lm", 1);
                    dict.Add("lr", 2);
                    dict.Add("ll", 3);
                    dict.Add("ri", 4);
                    dict.Add("rm", 5);
                    dict.Add("rr", 6);
                    dict.Add("rl", 7);
                    dict.Add("lt", 8);
                    dict.Add("rt", 9);

                    ArrayList fingersCollection = null;
                    //MemoryStream ms = null;
                    try
                    {
                        fingersCollection = cache.GetQualityFingerCollection(id);

//                        if (context.Application["fingersCollection"] == null || id != context.Application["id"] as String)
                        //if (HttpRuntime.Cache["fingersCollection"] == null || id != HttpRuntime.Cache["id"] as String)


                        //DataCache cache = CacheUtil.GetCache();
                        //ArrayList cfc = cache.Get("fingersCollection") as ArrayList;
                        //string cId = cache.Get("id") as String;

                        //if (MemoryCache.Default["fingersCollection"] == null || id != MemoryCache.Default["id"] as String)
                        //if (cfc == null || id != cId)
/*
                        var cache = new MemoryCacheServiceClient();
                        //if (!cache.Contains(id))
                        if (!cache.IsDirty(id))
                        {
                            cache.GetImage(IMAGE_TYPE.wsq, Convert.ToInt32(id));

//                            if (context.Application["fingersCollection"] != null)
//                            if (HttpRuntime.Cache["fingersCollection"] != null)
//                            if (MemoryCache.Default["fingersCollection"] != null)
                            //if (cfc != null)
                            //{
                                //context.Session.Remove("FingersLicences");
//                                context.Application.Remove("fingersCollection");
//                                context.Application.Remove("id");
//                                HttpRuntime.Cache.Remove("fingersCollection");
//                                HttpRuntime.Cache.Remove("id");
                                //MemoryCache.Default.Remove("fingersCollection");
                                //MemoryCache.Default.Remove("id");
                              //  cache.Remove("fingersCollection");
                              //  cache.Remove("id");
                                //MemoryCache.Default.Remove("fingersCollection");
                                //MemoryCache.Default.Remove("id");
                                //releaseFingersLicences();
                                //initFingersLicences(context);
                            //}

                            //initFingersLicences(context, out licensesMain, out licensesBss);

                            buffer = ds.GetImage(IMAGE_TYPE.wsq, Convert.ToInt32(id));

                            var bioProcessor = new BioProcessor.BioProcessor();
                            bioProcessor.processEnrolledData(buffer, out fingersCollection);

                            //ms = new MemoryStream(buffer);

                            // Construct a BinaryFormatter and use it to deserialize the data to the stream.
                            //BinaryFormatter formatter = new BinaryFormatter();
                            //formatter.Binder = new GenericBinder<WsqImage>();
                            //fingersCollection = formatter.Deserialize(ms) as ArrayList;
//                            context.Application["id"] = id;
//                            context.Application["fingersCollection"] = fingersCollection;
//                            HttpRuntime.Cache["id"] = id;
//                            HttpRuntime.Cache["fingersCollection"] = fingersCollection;
//                            cache.Add("fingersCollection", fingersCollection);
//                            cache.Add("id", id);
                            //cache.Put("id", id);
                            //cache.Put("fingersCollection", fingersCollection);
                            var dataContract = new FingerPrintDataContract();
                            dataContract.id = id;
                            dataContract.fingersCollection = fingersCollection;
                            cache.Put(dataContract);
                            //MemoryCache.Default["id"] = id;
                            //MemoryCache.Default["fingersCollection"] = fingersCollection;
                            //throw new Exception(fingersCollection.Count.ToString());
                            //releaseFingersLicences(licensesMain, licensesBss);

                        }
                        else
                            fingersCollection = cache.GetFingerCollection();
//                        fingersCollection = MemoryCache.Default["fingersCollection"] as ArrayList;
//                        fingersCollection = HttpRuntime.Cache["fingersCollection"] as ArrayList;
//                        fingersCollection = context.Application["fingersCollection"] as ArrayList;


                        //throw new Exception((fingersCollection == null).ToString());
                        //throw new Exception("type: " + fingersCollection[dict[wsqQuery]].GetType().ToString());

                        //throw new Exception("type: " + fingersCollection[dict[wsqQuery]].ToString());
                        //byte[] buffer2 = new byte[0];
                        //image.ToByteArray(ImageFormat.Bmp);
                        //buffer = fingersCollection[dict[wsqQuery]] as byte[];
                        //throw new Exception("type: " + fingersCollection[dict[wsqQuery]].ToString());
                        //throw new Exception("type: " + buffer2.GetType().ToString());

                        //WsqImage wsqImage = fingersCollection[dict[wsqQuery]] as WsqImage;
                        //buffer = ConvertWSQToBmp(wsqImage);
*/
                        if (fingersCollection != null)
                            context.Response.BinaryWrite(fingersCollection[dict[wsqQuery]] as byte[]);
                        else
                            throw new Exception("Finger collection is null");
                        //context.Response.Flush();
                    }
                    catch (Exception ex)
                    {
                        //context.Response.Write("<script language=javascript>alert('ERROR');</script>");
                        //context.Response.Write(String.Format("<script>$('#error_box').val({0});</script>", ex.ToString()));
                        //context.Response.Flush();
                        //context.Response.Status = "kuku";
                        //context.Response.StatusDescription = "kuku2";
                        //context.Response.SubStatusCode = 333;

                        //context.Response.StatusCode = 404;
                        //context.Response.SuppressContent = true;
                        //HttpContext.Current.ApplicationInstance.CompleteRequest();
                        context.Application["myerror"] = ex.Message;
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        //DisposeWSQImage();

                        //if (ms != null)
                          //  ms.Close();
                    }
                }
            }
        }
/*
        public class GenericBinder<T> : System.Runtime.Serialization.SerializationBinder
        {
            /// <summary>
            /// Resolve type
            /// </summary>
            /// <param name="assemblyName">eg. App_Code.y4xkvcpq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null</param>
            /// <param name="typeName">eg. String</param>
            /// <returns>Type for the deserializer to use</returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                // We're going to ignore the assembly name, and assume it's in the same assembly 
                // that <T> is defined (it's either T or a field/return type within T anyway)

                string[] typeInfo = typeName.Split('.');
                bool isSystem = (typeInfo[0].ToString() == "System");
                string className = typeInfo[typeInfo.Length - 1];

                // noop is the default, returns what was passed in
                Type toReturn = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));

                if (!isSystem && (toReturn == null))
                {   // don't bother if system, or if the GetType worked already (must be OK, surely?)
                    System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(typeof(T));
                    string assembly = a.FullName.Split(',')[0];   //FullName example: "App_Code.y4xkvcpq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
                    if (a == null)
                    {
                        throw new ArgumentException("Assembly for type '" + typeof(T).Name.ToString() + "' could not be loaded.");
                    }
                    else
                    {
                        Type newtype = a.GetType(assembly + "." + className);
                        if (newtype == null)
                        {
                            throw new ArgumentException("Type '" + typeName + "' could not be loaded from assembly '" + assembly + "'.");
                        }
                        else
                        {
                            toReturn = newtype;
                        }
                    }
                }
                return toReturn;
            }
        }

        internal static string getAppSetting(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            //throw new Exception(setting);

            // If we didn't find setting, try to load it from current dll's config file
            if (string.IsNullOrEmpty(setting))
            {
                var filename = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var configuration = ConfigurationManager.OpenExeConfiguration(filename);
                if (configuration != null)
                    setting = configuration.AppSettings.Settings[key].Value;
            }

            return setting;
        }
*/

/*
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