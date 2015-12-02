using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Runtime.InteropServices;

namespace BiometricService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MatchingService : IMatchingService
    {
        [DllImport("Lookup.dll", EntryPoint = "match", CallingConvention = CallingConvention.StdCall)]
        //public static extern UInt32 matchTemplate(
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
        //    string[] arrOfFingers, int arrOffingersSize,
        //    [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
        //    byte[] template,
        //    UInt32 size, System.Text.StringBuilder errorMessage, int messageSize);

        public static extern UInt32 matchTemplate(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            string[] arrOfFingers, int arrOffingersSize,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            byte[] template,
            UInt32 size,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 1)]
            string[] appSettings,
            System.Text.StringBuilder errorMessage, int messageSize);

        [DllImport("Lookup.dll", EntryPoint = "terminateMatchingService", CallingConvention = CallingConvention.StdCall)]
        public static extern void terminate();

        public UInt32 match(string[] arrOfFingers, int arrOfFingersSize, byte[] template, UInt32 size, string[] appSettings, ref System.Text.StringBuilder errorMessage, int messageSize)
        {
            return matchTemplate(arrOfFingers, arrOfFingersSize, template, size, appSettings, errorMessage, messageSize);
        }

        public void terminateMatchingService()
        {
            terminate();
        }
    }
}
