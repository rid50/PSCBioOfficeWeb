using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;

namespace WCFService
{
    [ServiceContract]
    //[ServiceKnownType(typeof(WsqImage))]
    public interface IMemoryCacheService
    {
        [OperationContract]
        bool Contains(string id);

        [OperationContract]
        void SetDirty();

        [OperationContract]
        byte[] GetRawFingerCollection(string id);

        [OperationContract]
        ArrayList GetQualityFingerCollection(string id);

        [OperationContract]
        byte[] GetPicture(string id);

        [OperationContract]
        void Put(FingerPrintDataContract fingersCollectionDataContract);
    }

    [DataContract]
    [KnownType (typeof(WsqImage))]
    public class FingerPrintDataContract
    {
        [DataMember]
        public string id = "";
        [DataMember]
        public ArrayList fingersCollection = null;
    }

}
