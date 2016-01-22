using System;
using System.ServiceModel;

namespace BiometricService
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMatchingServiceCallback))]
    public interface IMatchingService
    {
        [OperationContract(IsOneWay = true)]
        void fillCache2(string[] fingerList, int fingerListSize, string[] appSettings);

        [OperationContract]
        void fillCache(string[] fingerList, int fingerListSize, string[] appSettings);

        [OperationContract]
        UInt32 match(string[] fingerList, int fingerListSize, byte[] probeTemplate, UInt32 probeTemplateSize, string[] appSettings, ref System.Text.StringBuilder errorMessage, int messageSize);

        [OperationContract]
        void terminateMatchingService();
    }

    public interface IMatchingServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RespondWithRecordNumbers(int num);

        [OperationContract(IsOneWay = true)]
        void RespondWithText(string str);

        [OperationContract(IsOneWay = true)]
        void RespondWithError(string str);

        [OperationContract(IsOneWay = true)]
        void CacheOperationComplete();
    }
}
