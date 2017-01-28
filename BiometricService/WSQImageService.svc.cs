//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization.Formatters.Binary;
//using Neurotec.Biometrics.Client;
//using Neurotec.Biometrics;
//using System.IO;
using System.Collections;
//using Neurotec.Images;
using BiometricService.ConfigurationService;
using DAO;
//using WsqSerializationBinder;

namespace BiometricService
{
    public class WSQImageService : IWSQImageService
    {
        //public void DeserializeWSQArray(byte[][] serializedWSQArray, out ArrayList fingersCollection)
        //public void processRawData(byte[][] serializedWSQArray, out ArrayList fingersCollection)
        //{
        //    var bioProcessor = new BioProcessor.BioProcessor();
        //    try
        //    {
        //        //bioProcessor.DeserializeWSQArray(serializedWSQArray[0], out fingersCollection);
        //        bioProcessor.processRawData(serializedWSQArray, out fingersCollection);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw new System.Exception(ex.Message);
        //    }
        //}

        //public ArrayList processEnrolledData(byte[][] serializedWSQArray)
        public void processEnrolledData(byte[][] serializedWSQArray, out ArrayList fingersCollection)
        {
            //ArrayList fingersCollection;
            var bioProcessor = new BioProcessor.BioProcessor();
            try {
                bioProcessor.processEnrolledData(serializedWSQArray, out fingersCollection);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            //return fingersCollection;
        }

        public void SaveWSQImage(int id, byte[] buffer)
        {
            var bioProcessor = new BioProcessor.BioProcessor();
            Dictionary<string, byte[]> templates = bioProcessor.GetTemplatesFromWSQImage(id, buffer,  null);

            var client = new ConfigurationServiceClient();

            Dictionary<string, string> settings = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in client.AppSettings())
            {
                settings.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in client.ConnectionStrings())
            {
                settings.Add(kvp.Key, kvp.Value);
            }

            var db = new Database(settings);
            db.SaveWSQTemplate(id, templates);
        }
    }
}
