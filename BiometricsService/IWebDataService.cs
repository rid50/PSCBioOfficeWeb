using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DAO;

namespace BiometricsService
{
    [ServiceContract]
    public interface IWebDataService
    {
        [OperationContract]
        byte[] GetImage(IMAGE_TYPE imageType, int id);

        [OperationContract]
        void SendImage(IMAGE_TYPE imageType, int id, ref byte[] buffer);
    }
}
