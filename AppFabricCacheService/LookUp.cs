using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationServer.Caching;
using System.Collections;
using System.Threading;

namespace AppFabricCacheService
{
    public class LookUp
    {
        private ArrayList   _fingerList;
        private byte[]      _probeTemplate;
        private DataCache   _cache;
        private CancellationToken _ct;

        public LookUp(ArrayList fingerList, byte[] probeTemplate, DataCache cache, CancellationToken ct)
        {
            _fingerList     = fingerList;
            _probeTemplate  = probeTemplate;
            _cache          = cache;
            _ct             = ct;
        }

//        enum FingerListEnum { li, lm, lr, ll, ri, rm, rr, rl, lt, rt }

        public UInt32 run(String regionName)
        {
            var matcher = new BioProcessor.BioProcessor();

            try {
                matcher.enrollProbeTemplate(_fingerList, _probeTemplate);

                byte[][] buffer = new byte[10][];
                int rowNumber = 0;
                UInt32 retcode = 0;
                foreach (KeyValuePair<string, object> item in _cache.GetObjectsInRegion(regionName))
                {
                    //short numOfMatches = 0;
                    bool matched = false;
                    rowNumber++;
                    //continue;
                    //if (rowNumber % 1000 == 0)
                    //    Console.WriteLine("Region name: {0}, row number: {1}", regionName, rowNumber);

                    //if (_ct.IsCancellationRequested)
                    //{
                    _ct.ThrowIfCancellationRequested();
                    //}

                    //int i = 0;
                    //if (item.Key == "20005140")
                    //    i = 0;

                    matched = matcher.match(_fingerList, item.Value as byte[][]);

                    //if (item.Key == "123")
                    //if( false)
                    //{
                    //    buffer = item.Value as byte[][];
                    //for (int i = 0; i < buffer.Length; i++)
                    //foreach (string finger in _fingerList)
                    //{
                    //    FingerListEnum f = (FingerListEnum)Enum.Parse(typeof(FingerListEnum), finger);
                    //    if (buffer[(int)f] != null && (buffer[(int)f]).Length != 0)
                    //    {
                    //        matched = matcher.match(buffer[(int)f]);
                    //        if (matched)
                    //        {
                    //            numOfMatches++;
                    //        }
                    //    }



                    //}
                    //if (_fingerList.Count == numOfMatches)

                    if (matched)
                    {
                        retcode = UInt32.Parse(item.Key);
                        break;
                    }
                }

                return retcode;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}