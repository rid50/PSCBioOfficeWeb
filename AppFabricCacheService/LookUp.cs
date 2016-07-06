using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationServer.Caching;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

namespace AppFabricCacheService
{
    public class LookUp
    {
        private ArrayList   _fingerList;
        private int         _gender;
        private byte[]      _probeTemplate;
        private DataCache   _cache;
        private CancellationToken _ct;

        public LookUp(ArrayList fingerList, int gender, byte[] probeTemplate, DataCache cache, CancellationToken ct)
        {
            _fingerList     = fingerList;
            _gender         = gender;
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
                //int rowNumber = 0;
                UInt32 retcode = 0;
                foreach (KeyValuePair<string, object> item in _cache.GetObjectsInRegion(regionName))
                {
                    if (_gender == 1 && Regex.IsMatch(item.Key, "m$") ||
                        _gender == 2 && Regex.IsMatch(item.Key, "w$") ||
                        _gender == 0)
                    {

                        //short numOfMatches = 0;
                        bool matched = false;
                        //rowNumber++;
                        //continue;
                        //if (rowNumber % 1000 == 0)
                        //    Console.WriteLine("Region name: {0}, row number: {1}", regionName, rowNumber);

                        //if (_ct.IsCancellationRequested)
                        //{
                        _ct.ThrowIfCancellationRequested();
                        //}

                        //int i = 0;
                        //if (Regex.Replace(item.Key, ".$", "") == "20005140")
                            //matched = true;

                        matched = matcher.match(_fingerList, item.Value as byte[][]);
                        if (matched)
                        {
                            retcode = UInt32.Parse(Regex.Replace(item.Key, ".$", ""));
                            break;
                        }
                    }
                }

                return retcode;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                matcher.CleanBiometrics();
            }
        }
    }
}