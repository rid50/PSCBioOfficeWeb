using System.ServiceModel;

namespace BiometricService
{
    [ServiceContract]
    public interface IWSQImageService
    {
        [OperationContract]
        void SaveWSQImage(int id, byte[] buffer);
    }
}
