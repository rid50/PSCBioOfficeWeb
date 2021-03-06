﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Runtime.InteropServices;

namespace BiometricService
{
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class MatchingService : IMatchingService
    {
        //[DllImport("Lookup.dll", EntryPoint = "match", CallingConvention = CallingConvention.StdCall)]
        //public static extern UInt32 matchTemplate(
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
        //    string[] fingerList, int fingerListSize,
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
        //    byte[] template,
        //    UInt32 size, System.Text.StringBuilder errorMessage, int messageSize);

        [DllImport("Lookup.dll", EntryPoint = "fillCache", CallingConvention = CallingConvention.StdCall)]
        public static extern void fillCacheOnly(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            string[] fingerList, int fingerListSize,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            string[] appSettings, CallBackDelegate callback);

        [DllImport("Lookup.dll", EntryPoint = "match", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 matchTemplate(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            string[] fingerList, int fingerListSize,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            byte[] probeTemplate,
            UInt32 probeTemplatSsize,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            string[] appSettings,
            System.Text.StringBuilder errorMessage, int messageSize);

        [DllImport("Lookup.dll", EntryPoint = "terminateMatchingService", CallingConvention = CallingConvention.StdCall)]
        public static extern void terminate();

        //[DllImport("Lookup.dll", CharSet = CharSet.Auto)]
        //public static extern void SetCallBack(CallBackDelegate callback);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct CallBackStruct
        {
            public short code;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string text;
            //public System.Text.StringBuilder text;
        }

        //public delegate void DelegateNotify(int notifyCode, ref NotifyStruct notifyInfo);
        //public delegate void DelegateNotify(int notifyCode);
        //public delegate void CallBackDelegate(int callBackParam);

        public delegate void CallBackDelegate(ref CallBackStruct callBackParam);

        //public void OnCallback(int x, ref NotifyStruct notifyInfo)
        public void OnCallback(ref CallBackStruct callBackParam)
        {
            if (callBackParam.code == 0)
            {
                CallBack.CacheOperationComplete();
            }
            else if (callBackParam.code == 1)
            {
                int result;
                int.TryParse(callBackParam.text, out result);
                CallBack.RespondWithRecordNumbers(result);
            }
            else if (callBackParam.code == 2)
                CallBack.RespondWithText(callBackParam.text);
            else if (callBackParam.code == 3)
                CallBack.RespondWithError(callBackParam.text);
        }

        //public void setCallBack(CallBackDelegate callback)
        //{
        //    SetCallBack(callback);
        //}

        static public IMatchingServiceCallback CallBack
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IMatchingServiceCallback>();
            }
        }

        public void fillCache2(string[] fingerList, int fingerListSize, string[] appSettings)
        {
        }

        public void fillCache(string[] fingerList, int fingerListSize, string[] appSettings)
        {
            //DelegateNotify d = new DelegateNotify(OnCallback);
            //CallBackDelegate d = new CallBackDelegate(OnCallback);

            //SetCallBack(new CallBackDelegate(OnCallback));
            //SetNotifyCallBack(CallBack as MulticastDelegate);
            fillCacheOnly(fingerList, fingerListSize, appSettings, new CallBackDelegate(OnCallback));
        }

        public UInt32 match(string[] fingerList, int fingerListSize, byte[] probeTemplate, UInt32 probeTemplateSize, string[] appSettings, ref System.Text.StringBuilder errorMessage, int messageSize)
        {
            return matchTemplate(fingerList, fingerListSize, probeTemplate, probeTemplateSize, appSettings, errorMessage, messageSize);
        }

        public void terminateMatchingService()
        {
            terminate();
        }
    }
}
