using System;
using System.ServiceModel;

namespace BiometricService
{

    [ServiceContract]
    public interface IMatchingService
    {
        [OperationContract]
        UInt32 match(string[] arrOfFingers, int arrOfFingersSize, byte[] template, UInt32 size, string[] appSettings, ref System.Text.StringBuilder errorMessage, int messageSize);

        [OperationContract]
        void terminateMatchingService();
    }
}
