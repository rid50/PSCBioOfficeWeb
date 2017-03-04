using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;
//using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MemoryCacheService
{
    //[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMatchingCallback))]
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMatchingService
    {
        [OperationContract]
        ArrayList getFingerList();

        //[OperationContract]
        //Task<int> Terminate();

        [OperationContract]
        void Terminate(string guid);

        //[OperationContractAttribute(AsyncPattern = true)]
        //IAsyncResult BeginTerminate(int msg, AsyncCallback callback, object asyncState);

        ////Note: There is no OperationContractAttribute for the end method.
        //void EndTerminate(IAsyncResult result);

        [OperationContract]
        bool verify(byte[] probeTemplate, byte[] galleryTemplate, int matchingThreshold);

        //[OperationContract(IsOneWay = true)]
        //Task<int> match2(ArrayList fingerList, int gender, byte[] probeTemplate);

        //[OperationContract(IsOneWay = true)]
        [OperationContract]
        [FaultContractAttribute(typeof(Exception))]
        //void enrollGalleryTemplate(List<string> fingerList);
        MatchingResult match(string guid, List<string> fingerList, int gender, int firstMatch, byte[] probeTemplate, int matchingThreshold);

        //[OperationContract]
        //void match2(ArrayList fingerList, int gender, byte[] probeTemplate, int matchingThreshold);
    }

    [DataContract]
    [Serializable]
    [KnownType(typeof(List<Tuple<string, int>>))] //required because ArrayList is used polymorphically
    public class MatchingResult
    {
        private List<Tuple<string, int>> _list;
        [DataMember]
        public List<Tuple<string, int>> Result
        {
            get { return _list; }
            set { _list = value; }
        }
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
