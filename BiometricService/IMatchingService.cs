using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;

namespace BiometricService
{
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //public struct CallBackStruct
    //{
    //    public short code;
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    //    public string text;
    //    //public System.Text.StringBuilder text;
    //}

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMatchingServiceCallback))]
    public interface IMatchingService
    {
        //[OperationContract]
        //void setCallBack(CallBackDelegate callback);

        [OperationContract(IsOneWay = true)]
        void fillCache(string[] fingerList, int fingerListSize, string[] appSettings);

        [OperationContract]
        void fillCache2(string[] fingerList, int fingerListSize, string[] appSettings);

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
