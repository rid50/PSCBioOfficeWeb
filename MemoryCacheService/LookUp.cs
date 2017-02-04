using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using Microsoft.ApplicationServer.Caching;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.Caching;

namespace MemoryCacheService
{
    public class LookUp
    {
        private ArrayList   _fingerList;
        private int         _gender;
        private int         _firstMatch;
        private byte[]      _probeTemplate;
        private MemoryCache   _cache;
        private CancellationToken _ct;

        public LookUp(ArrayList fingerList, int gender, int firstMatch, byte[] probeTemplate, MemoryCache cache, CancellationToken ct)
        {
            _fingerList     = fingerList;
            _gender         = gender;
            _firstMatch     = firstMatch;
            _probeTemplate  = probeTemplate;
            _cache          = cache;
            _ct             = ct;
        }

//        enum FingerListEnum { li, lm, lr, ll, ri, rm, rr, rl, lt, rt }

        public List<Tuple<string, int>> run(String regionName, int matchingThreshold)
        {
            //var result = new ArrayList();

            var matcher = new BioProcessor.BioProcessor(MatchingThreshold: matchingThreshold);

            //var items = _cache.Where(x => Regex.IsMatch(x.Key, "^" + regionName)).Select(x => x.Key);
            try
            {
                matcher.enrollProbeTemplate(_fingerList, _probeTemplate);

                //byte[][] buffer = new byte[10][];
                //int rowNumber = 0;
                //UInt32 retcode = 0;
                foreach (KeyValuePair<string, object> item in _cache.Where(x => Regex.IsMatch(x.Key, "^" + regionName))
                                                    .Select(x => new KeyValuePair<string, object>(x.Key, x.Value)))
                {
                    if (_gender == 1 && Regex.IsMatch(item.Key, "m") ||
                        _gender == 2 && Regex.IsMatch(item.Key, "w") ||
                        _gender == 0)
                    {
                        //throw new Exception("kuku");

                        //short numOfMatches = 0;
                        //bool matched = false;
                        //rowNumber++;
                        //continue;
                        //if (rowNumber % 1000 == 0)
                        //    Console.WriteLine("Region name: {0}, row number: {1}", regionName, rowNumber);

                        //if (_ct.IsCancellationRequested)
                        //{
                        //_ct.ThrowIfCancellationRequested();
                        //}
                        ////if (_ct.IsCancellationRequested)
                        ////{
                        ////    break;
                        ////    //_ct.ThrowIfCancellationRequested();
                        ////}
                        //int i = 0;
                        //if (Regex.Replace(item.Key, ".$", "") == "20005140")
                        //  matched = true;

                        //int k = item.Key.IndexOf("m");
                        //if (item.Key.Substring(k + 1) == "20005140")
                        //    matched = true;

                        matcher.enrollGalleryTemplate(_fingerList, item.Value as byte[][], item.Key);

                        //matched = matcher.match(_fingerList, item.Value as byte[][]);
                        //if (matched)
                        //{
                        //    int i = item.Key.IndexOf("m");
                        //    if (i == -1)
                        //        i = item.Key.IndexOf("w");

                        //    retcode = UInt32.Parse(item.Key.Substring(i + 1));
                        //    //retcode = UInt32.Parse(Regex.Replace(item.Key, ".$", ""));
                        //    break;
                        //}
                    }
                }

                return matcher.identify(_firstMatch == 1);
                ////if (ret != String.Empty)
                ////{
                ////    int i = ret.IndexOf("m");
                ////    if (i == -1)
                ////        i = ret.IndexOf("w");

                ////    retcode = UInt32.Parse(ret.Substring(i + 1));
                ////}

                //////_ct.ThrowIfCancellationRequested();
                ////return retcode;
            }
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message, ex.InnerException);
            //}
            finally
            {
                matcher.CleanBiometrics();
            }
        }
    }
}