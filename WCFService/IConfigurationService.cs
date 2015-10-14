using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFService
{
    [ServiceContract]
    public interface IConfigurationService
    {
        [OperationContract]
        string getAppSetting(string key);

        [OperationContract]
        string getConnectionString(string name);
    }
}
