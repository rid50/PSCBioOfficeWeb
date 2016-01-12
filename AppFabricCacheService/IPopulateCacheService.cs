using System.ServiceModel;

namespace AppFabricCacheService
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IPopulateCacheCallback))]
    public interface IPopulateCacheService
    {
        [OperationContract(IsOneWay=true)]
        void Run(string[] args);
    }

    public interface IPopulateCacheCallback
    {
        [OperationContract(IsOneWay = true)]
        void RespondWithRecordNumbers(int num);

        [OperationContract(IsOneWay = true)]
        void RespondWithResult(string str);

        [OperationContract(IsOneWay = true)]
        void RespondWithError(string str);

        [OperationContract(IsOneWay = true)]
        void CacheComplete();
    }
}
