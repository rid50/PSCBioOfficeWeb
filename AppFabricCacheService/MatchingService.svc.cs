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
        public ArrayList    fingerList;
        public byte[]       probeTemplate;
        public DataCache    cache;
        public String       regionName;
        public CancellationToken ct;
    }

    public class MatchingService : IMatchingService
    {
        private static CancellationTokenSource _tokenSource;
        private static DataCache _cache;
        //private static DataCacheFactory _factory;

        static MatchingService()
        {
            DataCacheFactory factory = new DataCacheFactory();
            _cache = factory.GetCache("default");
            //Debug.Assert(_cache == null);
        }

        public ArrayList getFingerList()
        {
            //_cache = _factory.GetCache("default");
            return _cache.Get("fingerList") as ArrayList;
        }

        public void Terminate()
        {
            _tokenSource.Cancel();
        }

        public UInt32 match(ArrayList fingerList, byte[] probeTemplate)
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

            UInt32 retcode = 0;

            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;

            //string regionName = "0";
            //for(int k = 0; k < 2; k++)
            int i = 0;
            foreach (string regionName in regionNameList)
            {
                taskArray[i++] = Task.Factory.StartNew((Object obj) =>
                {
                    if (ct.IsCancellationRequested)
                    {
                        ct.ThrowIfCancellationRequested();
                    }

                    MatchStateObject state = obj as MatchStateObject;

                    var process = new LookUp(state.fingerList, state.probeTemplate, state.cache, state.ct);

                    retcode = process.run(state.regionName);

                    if (retcode != 0)
                        _tokenSource.Cancel();

                    return retcode;
                },
                new MatchStateObject() { fingerList = fingerList, probeTemplate = probeTemplate, cache = _cache, regionName = regionName, ct = ct },
                _tokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            }

            try
            {
                Task.WaitAll(taskArray);
                foreach (var t in taskArray)
                {
                    if (t.Status == TaskStatus.RanToCompletion && (UInt32)t.Result != 0)
                    {
                        retcode = (UInt32)t.Result;
                        break;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {

                foreach (var t in taskArray)
                {
                    if (t.Status == TaskStatus.RanToCompletion && (int)t.Result != 0)
                    {
                        retcode = (UInt32)t.Result;
                        break;
                    }
                    else if (t.Status == TaskStatus.Faulted)
                    {
                        while ((ex is AggregateException) && (ex.InnerException != null))
                            ex = ex.InnerException;
                        throw new Exception(ex.Message);
                    }
                }
            }
            finally
            {
                //if (ct.IsCancellationRequested)
                //{
                //    throw new Exception("The request was cancelled");
                //}

                _tokenSource.Dispose();
            }

            return retcode;
        }
    }
}
