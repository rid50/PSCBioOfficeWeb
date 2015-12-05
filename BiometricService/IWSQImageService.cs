using System.ServiceModel;
using System.Collections;

namespace BiometricService
{
    [ServiceContract]
    public interface IWSQImageService
    {
        [OperationContract]
        void SaveWSQImage(int id, byte[] buffer);

        [OperationContract]
        void DeserializeWSQArray(byte[] serializedWSQArray, out ArrayList fingersCollection);

        [OperationContract]
        //ArrayList processEnrolledData(byte[][] serializedWSQArray);
        void processEnrolledData(byte[][] serializedWSQArray, out ArrayList fingersCollection);
    }
}
