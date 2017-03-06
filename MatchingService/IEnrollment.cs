using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MatchingService
{
    [ServiceContract(SessionMode = SessionMode.Allowed, CallbackContract = typeof(IEnrollementCallback))]
    public interface IEnrollment
    {
        [OperationContract(IsOneWay = true)]
        void Run2(ArrayList fingerList);

        [OperationContract]
        void Run(ArrayList fingerList);

        [OperationContract]
        [FaultContractAttribute(typeof(string))]
        ArrayList GetFingerList();

        [OperationContract]
        [FaultContractAttribute(typeof(string))]
        System.DateTime GetExpirationTime();

        [OperationContract]
        [FaultContractAttribute(typeof(string))]
        bool Verify(byte[] probeTemplate, byte[] galleryTemplate, int matchingThreshold);

        [OperationContract]
        [FaultContractAttribute(typeof(string))]
        MatchingResult Identify(ArrayList fingerList, int firstMatch, byte[] probeTemplate, int matchingThreshold);

        [OperationContract]
        int Terminate();
    }

    public interface IEnrollementCallback
    {
        [OperationContract(IsOneWay = true)]
        void RespondWithRecordNumbers(int num);

        [OperationContract(IsOneWay = true)]
        void RespondWithText(string str, bool append = false);

        [OperationContract(IsOneWay = true)]
        void RespondWithError(string str);

        [OperationContract(IsOneWay = true)]
        void CacheOperationComplete();
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
}
