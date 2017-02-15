using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;

namespace BiometricService
{
    [ServiceContract]
    public interface IWSQImageService
    {
        [OperationContract]
        void SaveWSQImage(int id, byte[] buffer);

        //[OperationContract]
        //void processRawData(byte[][] serializedWSQArray, out ArrayList fingersCollection);
        ////void DeserializeWSQArray(byte[][] serializedWSQArray, out ArrayList fingersCollection);

        [OperationContract]
        [FaultContractAttribute(typeof(System.ComponentModel.DataAnnotations.ValidationException))]
        void processEnrolledData(byte[][] serializedWSQArray, out ArrayList fingersCollection);

        [OperationContract]
        Dictionary<string, byte[]> GetTemplatesFromWSQImage(int id, byte[] buffer, bool[] processAsTemplate);
        //ArrayList processEnrolledData(byte[][] serializedWSQArray);
    }
}
