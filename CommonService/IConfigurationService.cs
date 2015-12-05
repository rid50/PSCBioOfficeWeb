using System.Collections.Generic;
using System.ServiceModel;
using System.Configuration;

namespace CommonService
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
        IConfigurationService GetConfigurationManager();

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
