using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;

namespace MemoryCacheService
{
    [ServiceContract]
    public interface IMatchingService
    {
        [OperationContract]
        ArrayList getFingerList();

        [OperationContract]
        void Terminate();

        [OperationContract]
        bool verify(byte[] probeTemplate, byte[] galleryTemplate);

        [OperationContract]
        UInt32 match(ArrayList fingerList, int gender, byte[] probeTemplate);
    }
}
