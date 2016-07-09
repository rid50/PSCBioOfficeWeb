using System.ServiceModel;
using System.Collections;

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
        void processEnrolledData(byte[][] serializedWSQArray, out ArrayList fingersCollection);
        //ArrayList processEnrolledData(byte[][] serializedWSQArray);
    }
}
