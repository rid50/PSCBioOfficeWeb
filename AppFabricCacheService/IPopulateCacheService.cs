using System.ServiceModel;
using System.Collections;

namespace AppFabricCacheService
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IPopulateCacheCallback))]
    public interface IPopulateCacheService
    {
        [OperationContract(IsOneWay = true)]
        void Run(ArrayList fingerList);

        [OperationContract]
        void Run2(ArrayList fingerList);

        [OperationContract]
        [FaultContractAttribute(typeof(System.ComponentModel.DataAnnotations.ValidationException))]
        ArrayList getFingerList();

        [OperationContract]
        System.DateTime getExpirationTime();

        [OperationContract]
        void Terminate();
    }

    public interface IPopulateCacheCallback
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
