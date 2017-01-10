using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;
//using System.Threading;
using System.Threading.Tasks;

namespace MemoryCacheService
{
    //[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMatchingCallback))]
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IMatchingService
    {
        [OperationContract]
        ArrayList getFingerList();

        //[OperationContract]
        //Task<int> Terminate();

        [OperationContract]
        int Terminate();

        //[OperationContractAttribute(AsyncPattern = true)]
        //IAsyncResult BeginTerminate(int msg, AsyncCallback callback, object asyncState);

        ////Note: There is no OperationContractAttribute for the end method.
        //void EndTerminate(IAsyncResult result);

        [OperationContract]
        bool verify(byte[] probeTemplate, byte[] galleryTemplate);

        //[OperationContract(IsOneWay = true)]
        //Task<int> match2(ArrayList fingerList, int gender, byte[] probeTemplate);

        //[OperationContract(IsOneWay = true)]
        [OperationContract]
        [FaultContractAttribute(typeof(Exception))]
        UInt32 match(ArrayList fingerList, int gender, byte[] probeTemplate);

        //[OperationContract]
        //void match2(ArrayList fingerList, int gender, byte[] probeTemplate);
    }

    //public interface IMatchingCallback
    //{
    //    [OperationContract(IsOneWay = true)]
    //    void RespondWithText(string str);

    //    [OperationContract(IsOneWay = true)]
    //    void RespondWithError(string str);

    //    [OperationContract(IsOneWay = true)]
    //    void MatchingComplete();
    //}

    //class CompletedAsyncResult<T> : IAsyncResult
    //{
    //    T data;

    //    public CompletedAsyncResult(T data)
    //    { this.data = data; }

    //    public T Data
    //    { get { return data; } }

    //    #region IAsyncResult Members
    //    public object AsyncState
    //    { get { return (object)data; } }

    //    public WaitHandle AsyncWaitHandle
    //    { get { throw new Exception("The method or operation is not implemented."); } }

    //    public bool CompletedSynchronously
    //    { get { return true; } }

    //    public bool IsCompleted
    //    { get { return true; } }
    //    #endregion
    //}
}
