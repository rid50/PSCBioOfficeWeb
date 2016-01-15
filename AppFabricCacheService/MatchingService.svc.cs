using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;
using Microsoft.ApplicationServer.Caching;
using System.Threading.Tasks;
using System.Threading;

namespace AppFabricCacheService
{
    class MatchStateObject
    {
        public ArrayList fingerList;
        public byte[] template;
        public DataCache cache;
        public String regionName;
        public CancellationToken ct;
    }

    public class MatchingService : IMatchingService
    {
        private static DataCache _cache = null;

        static MatchingService()
        {
            DataCacheFactory factory = new DataCacheFactory();
            _cache = factory.GetCache("default");
            //Debug.Assert(_cache == null);
        }

        public ArrayList getFingerList()
        {
            return _cache.Get("fingerList") as ArrayList;
        }

        public UInt32 match(ArrayList fingerList, byte[] template)
        {
            ArrayList regionNameList;

            if (_cache.Get("regionNameList") != null)
            {
                regionNameList = _cache.Get("regionNameList") as ArrayList;
            }
            else
            {
                throw new FaultException("Cache is empty");
            }

            Task<UInt32>[] taskArray = new Task<UInt32>[regionNameList.Count];
            //Task<UInt32>[] taskArray = new Task<UInt32>[2];

            int i = 0; UInt32 retcode = 0;

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            //string regionName = "0";
            //for(int k = 0; k < 2; k++)
            foreach (string regionName in regionNameList)
            {
                taskArray[i++] = Task.Factory.StartNew((Object obj) =>
                {
                    ct.ThrowIfCancellationRequested();

                    MatchStateObject state = obj as MatchStateObject;

                    var process = new LookUp(state.fingerList, state.template, state.cache, state.ct);

                    retcode = process.run(state.regionName);

                    if (retcode != 0)
                        tokenSource.Cancel();

                    return retcode;
                },
                new MatchStateObject() { fingerList = fingerList, template = template, cache = _cache, regionName = regionName, ct = ct },
                tokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            }

            try
            {
                Task.WaitAll(taskArray);
                foreach (var t in taskArray)
                {
                    if (t.Status == TaskStatus.RanToCompletion && (UInt32)t.Result != 0)
                        return (UInt32)t.Result;
                }

                return 0;
                //Console.WriteLine(" ----- Time elapsed: {0}", stw.Elapsed);
            }
            catch (Exception ex)
            {
                foreach (var t in taskArray)
                {
                    if (t.Status == TaskStatus.RanToCompletion && (UInt32)t.Result != 0)
                        return (UInt32)t.Result;
                    else if (t.Status == TaskStatus.Faulted)
                    {
                        while ((ex is AggregateException) && (ex.InnerException != null))
                            ex = ex.InnerException;
                        throw new Exception(ex.Message);
                    }

                    //Console.WriteLine("{0,10} {1,20} {2,14}",
                    //                  t.Id, t.Status,
                    //                  t.Status != TaskStatus.Canceled ? t.Status != TaskStatus.Faulted ? t.Result.ToString("N0") : "n/a" : "falted");
                }
            }

            return 0;
        }
    }
}
