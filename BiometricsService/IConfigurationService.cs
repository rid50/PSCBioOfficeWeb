using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;

namespace BiometricsService
{
    [ServiceContract]
    public interface IConfigurationService
    {
        //[OperationContract]   does not work for a property, have to set it on get, set methods
        //System.Collections.Specialized.NameValueCollection AppSetting;

        //[OperationContract]
        //System.Configuration.ConnectionStringSettingsCollection ConnectionString;

        //System.Collections.Specialized.NameValueCollection AppSetting { [OperationContract()] get; }
        [OperationContract]
        Dictionary<string, string> AppSettings();

        [OperationContract]
        Dictionary<string, string> ConnectionStrings();
        
        [OperationContract]
        string getAppSetting(string key);

        [OperationContract]
        string getConnectionString(string name);

    }
}
