using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DAO;

namespace WCFService
{
    [ServiceContract]
    public interface IDbDataService
    {
        [OperationContract]
        byte[] GetImage(IMAGE_TYPE imageType, int id);

        [OperationContract]
        void SendImage(IMAGE_TYPE imageType, int id, ref byte[] buffer);
    }

    [DataContract]
    public class JsonResult
    {
#pragma warning disable 0649    //warning CS0649: Field 'DataSourceServices.JsonResult.result' is never assigned to, and will always have its default value null
        [DataMember(Name = "result", IsRequired = false)]
        public string result;
        [DataMember(Name = "picture", IsRequired = false)]
        public string picture;
        [DataMember(Name = "wsq", IsRequired = false)]
        public string wsq;
#pragma warning restore 0649
    }
}
