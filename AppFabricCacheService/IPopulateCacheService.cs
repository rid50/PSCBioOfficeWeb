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
        void Callback(string str);
    }
}
