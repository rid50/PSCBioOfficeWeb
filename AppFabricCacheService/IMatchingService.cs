using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;

namespace AppFabricCacheService
{
    [ServiceContract]
    public interface IMatchingService
    {
        [OperationContract]
        ArrayList getFingerList();

        [OperationContract]
        void Terminate();

        [OperationContract]
        UInt32 match(ArrayList arrOfFingers, byte[] template);
    }
}
