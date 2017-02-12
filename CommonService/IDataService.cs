using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CommonService
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        byte[] GetWSQImages(string id);

        [OperationContract]
        void SetWSQImages(int id, ref byte[] buffer);

        [OperationContract]
        void SaveWSQInDatabase(int id, byte[] buffer, bool[] processAsTemplate);
    }
}
