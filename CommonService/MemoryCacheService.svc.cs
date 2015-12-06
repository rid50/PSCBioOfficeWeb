using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Runtime.Caching;
using System.Collections;
using DAO;
using System.Configuration;
using CommonService.WSQImageService;
//using DataService.ConfigurationService;


namespace CommonService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MemoryCacheService : IMemoryCacheService
    {
        //static public ConfigurationService configurationServiceClient;
        static public Dictionary<string, string> _settings = new Dictionary<string, string>();
        ArrayList _fingersCollection = null;

        static MemoryCacheService()
        {
            //System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(@"c:\temp\debug.log"));

            //BasicHttpBinding basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            //basicHttpbinding.Name = "BasicHttpBinding_IConfigurationService";
            //basicHttpbinding.MessageEncoding = WSMessageEncoding.Mtom;
            //basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            //basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            //EndpointAddress endpointAddress = new EndpointAddress("http://biooffice/WcfSiteConfigurationService/ConfigurationService.svc");
            //configurationServiceClient = new ConfigurationServiceClient(basicHttpbinding, endpointAddress);

            //System.Diagnostics.Debug.WriteLine("kuku", "DEBUG2::");
            //System.Diagnostics.Debug.Close();

            //configurationServiceClient = new ConfigurationService();
            //configurationServiceClient = new ConfigurationService();
            //System.Diagnostics.Debug.WriteLine("kuku2", "DEBUG2::");
            //System.Diagnostics.Debug.Close();
            var client = new ConfigurationService();

            foreach (KeyValuePair<string, string> kvp in client.AppSettings())
            {
                _settings.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in client.ConnectionStrings())
            {
                _settings.Add(kvp.Key, kvp.Value);
            }
        }

        private bool IsDirty(string id)
        {
            return MemoryCache.Default["fingersCollection"] == null || 
                    MemoryCache.Default["dirty"] == null ||
                    "true" == MemoryCache.Default["dirty"] as string ||
                    id != MemoryCache.Default["id"] as String;
        }

        public void SetDirty()
        {
            //throw new FaultException<String>("MyFault");

            try
            {
                MemoryCache.Default["dirty"] = "true";
            }
            catch (Exception ex)
            {
                throw new FaultException<String>(ex.Message);
            }
        }

        public byte[][] GetImage(IMAGE_TYPE imageType, int id)
        {
            byte[][] buffer = new byte[11][]; 
            //byte[] buffer = new byte[1];
            //ArrayList fingersCollection = null;

            //if (getAppSetting("Enroll") == "db")
            if (_settings["enroll"] == "db")
            {
                DataSource ds = null;

                //if (getAppSetting("provider") == "directDb")
                    //ds = new Database();
                //else if (getAppSetting("provider") == "directWebService")
                //    ds = new CloudDatabase();


                ds = new DAO.Database(_settings);

                if (imageType == IMAGE_TYPE.wsq)
                {
                    buffer = ds.GetImage(IMAGE_TYPE.wsq, Convert.ToInt32(id));

                    //var bioProcessor = new BioProcessor.BioProcessor();
                    //bioProcessor.processEnrolledData(buffer, out fingersCollection);

                }
                else if (imageType == IMAGE_TYPE.picture)
                {
                    buffer = ds.GetImage(IMAGE_TYPE.picture, Convert.ToInt32(id));
                }
            }

            return buffer;

        }

        public bool Contains(string id)
        {
            if (MemoryCache.Default["fingersCollection"] == null || id != MemoryCache.Default["id"] as String)
            {
                if (MemoryCache.Default["fingersCollection"] != null)
                {
                    MemoryCache.Default.Remove("fingersCollection");
                    MemoryCache.Default.Remove("id");
                }
                return false;
            }
            else
                return true;
        }

        public byte[] GetRawFingerCollection(string id)
        {
            byte[] buffer = new byte[1];
            //ArrayList fingersCollection = null;

            //if (getAppSetting("Enroll") == "db")
            if (_settings["enroll"] == "db")
            {
                var ds = new Database(_settings);
                buffer = ds.GetImage(IMAGE_TYPE.wsq, Convert.ToInt32(id))[0];
                var biometricService = new WSQImageServiceClient();
                _fingersCollection = biometricService.DeserializeWSQArray(buffer);

                //var bioProcessor = new BioProcessor.BioProcessor();
                //bioProcessor.DeserializeWSQArray(buffer, out fingersCollection);
            }

            return buffer;
        }

        public ArrayList GetQualityFingerCollection(string id)
        {
            if (IsDirty(id))
            {
                //byte[] buffer = new byte[1];
                byte[][] buffer = new byte[11][]; 

                //ArrayList fingersCollection = null;

                //if (getAppSetting("Enroll") == "db")
                if (_settings["enroll"] == "db")
                {
                    DataSource ds = null;
                    //if (getAppSetting("provider") == "directDb")
                    ds = new Database(_settings);
                    //else if (getAppSetting("provider") == "directWebService")
                    //    ds = new CloudDatabase();

                    //if (getAppSetting("fingerTemplates") == "yes")
                        //buffer = ds.GetImage(IMAGE_TYPE.fingerTemplates, Convert.ToInt32(id));
                    //else
                    buffer = ds.GetImage(IMAGE_TYPE.wsq, Convert.ToInt32(id));

                    var biometricService = new WSQImageServiceClient();
                    _fingersCollection = biometricService.processEnrolledData(buffer);

                    //var bioProcessor = new BioProcessor.BioProcessor();
                    //bioProcessor.processEnrolledData(buffer, out fingersCollection);
                    MemoryCache.Default["id"] = id;
                    MemoryCache.Default["fingersCollection"] = _fingersCollection;
                    MemoryCache.Default["dirty"] = "false";
                }

                return _fingersCollection;
            }
            else
                return MemoryCache.Default["fingersCollection"] as ArrayList;
        }

        public byte[] GetPicture(string id)
        {
            //if (getAppSetting("Enroll") == "db")
            if (_settings["enroll"] == "db")
            {
                var ds = new Database(_settings);
                return ds.GetImage(IMAGE_TYPE.picture, Convert.ToInt32(id))[0];
            }
            else
            {
                return new byte[0];
            }
        }

        public void Put(FingerPrintDataContract fingersCollectionDataContract)
        {
            MemoryCache.Default["id"] = fingersCollectionDataContract.id;
            MemoryCache.Default["fingersCollection"] = fingersCollectionDataContract.fingersCollection;
            MemoryCache.Default["dirty"] = "false";
        }

        //public string getAppSetting(string key)
        //{

        //    //System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(@"c:\temp\debug.log"));
        //    //System.Diagnostics.Debug.WriteLine("setting", "DEBUG2::");
        //    //System.Diagnostics.Debug.Close();

        //    //throw new FaultException(key);

        //    //ConfigurationServiceClient configurationServiceClient = new ConfigurationService.ConfigurationServiceClient();

        //    var setting = configurationServiceClient.getAppSetting(key);
        //    //if (string.IsNullOrEmpty(setting))


        //    //var setting = ConfigurationManager.AppSettings[key];
        //    ////throw new Exception(setting);

        //    //// If we didn't find setting, try to load it from current dll's config file
        //    //if (string.IsNullOrEmpty(setting))
        //    //{
        //    //    var filename = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //    //    var configuration = ConfigurationManager.OpenExeConfiguration(filename);
        //    //    if (configuration != null)
        //    //        setting = configuration.AppSettings.Settings[key].Value;
        //    //}
        //    //string setting = "aa";
        //    return setting;
        //}
    }
}
