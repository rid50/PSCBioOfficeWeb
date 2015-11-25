using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DAO
{
    public enum IMAGE_TYPE
    {
        picture = 0,
        wsq = 1
        //fingerTemplates = 2
    }

    public abstract class DataSource
    {
        public abstract byte[][] GetImage(IMAGE_TYPE imageType, System.Int32 id);
        public abstract void SendImage(IMAGE_TYPE imageType, int id, ref byte[] buffer);
        //public abstract Dictionary<int, byte[]> GetRecordSet(Dictionary<int, byte[]> list);
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
