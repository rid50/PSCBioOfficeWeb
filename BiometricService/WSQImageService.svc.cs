using System.Collections.Generic;
using System.Collections;
using BiometricService.ConfigurationService;
using DAO;
//using WsqSerializationBinder;

namespace BiometricService
{
    public class WSQImageService : IWSQImageService
    {
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

        public Dictionary<string, byte[]> GetTemplatesFromWSQImage(int id, byte[] buffer, bool[] processAsTemplate)
        {
            var bioProcessor = new BioProcessor.BioProcessor();
            Dictionary<string, byte[]> templates = bioProcessor.GetTemplatesFromWSQImage(id, buffer, processAsTemplate);
            return templates;
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
