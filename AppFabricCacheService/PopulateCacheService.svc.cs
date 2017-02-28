using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Diagnostics;
//using FillAppFabricCache;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;
using Microsoft.ApplicationServer.Caching;
using AppFabricCacheService.ConfigurationService;
using System.ComponentModel.DataAnnotations;

namespace AppFabricCacheService
{
    //delegate void SendOrPostCallbackDlgt(object num);

    class PopulateStateObject
    {
        public int LoopCounter;
        public CancellationToken ct;
        //public SendOrPostCallback dlgt;
        //public IPopulateCacheCallback CallBack;
        //public SynchronizationContext Context;
        public ArrayList fingerList;
        public int maxPoolSize;
        public BlockingCollection<int> bc;
        public DataCache cache;
    }

    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PopulateCacheService : IPopulateCacheService
    {
        private static CancellationTokenSource _tokenSource;
        private static DataCache _cache;
        private static List<FillAppFabricCache> _fillCacheClassList;

        private static TimeSpan cacheTimeSpan = new TimeSpan(24, 0, 0);
        //private static DataCacheFactory _factory;
        //private static bool _terminate = false;

        //static PopulateCacheService()
        //{
        //    _factory = new DataCacheFactory();
        //    //try
        //    //{
        //    //    _cache = factory.GetCache("default");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new FaultException<string>(ex.Message);
        //    //    //throw new FaultException(ex.Message);
        //    //}
        //    //Debug.Assert(_cache == null);
        //}

        private DataCache initDataCache()
        {
            try
            {
                if (_cache == null)
                {
                    DataCacheFactory factory = new DataCacheFactory();
                    _cache = factory.GetCache("default");
                }
                return _cache;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //static public void CallDelegate(object rowcount)
        //{
        //    CallBack.RespondWithRecordNumbers((int)rowcount);
        //}

        //SendOrPostCallback dlgt = new SendOrPostCallback(CallDelegate);
        delegate void d();

        static public IPopulateCacheCallback CallBack
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IPopulateCacheCallback>();
            }
        }

        public ArrayList getFingerList()
        {
            try {
                return initDataCache().Get("fingerList") as ArrayList;
            }
            catch (Exception ex)
            {
                throw new FaultException<string>("AppFabric caching service is not available. Launch PowerShell command \"get-cachehost\" to see if it is down");
            }
        }

        public DateTime getExpirationTime()
        {
            try
            {
                return (DateTime)initDataCache().Get("cacheExpirationTime");
            }
            catch (Exception ex)
            {
                throw new FaultException<string>("AppFabric caching service is not available. Launch PowerShell command \"get-cachehost\" to see if it is down");
            }
        }

        public void Terminate()
        {
            try {
                _tokenSource.Cancel();
            } catch (Exception) { }

            //int id = Thread.CurrentThread.ManagedThreadId;
            foreach (var fillCacheClass in _fillCacheClassList)
            {
                if (fillCacheClass.cmd != null)
                    fillCacheClass.cmd.Cancel();
            }
        }

        private void dumpCache() {
            if (_cache.Get("regionNameList") != null)
            {
                ArrayList regionNameList = _cache.Get("regionNameList") as ArrayList;
                foreach (string regionName in regionNameList)
                {
                    if (regionName != null)
                        _cache.RemoveRegion(regionName);
                }
            }

            _cache.Remove("fingerList");
            _cache.Remove("regionNameList");
        }

        public void Run2(ArrayList fingerList)
        {
        }

        //public void Run(string[] args)
        public void Run(ArrayList fingerList)
        {
            //            Stopwatch st = new Stopwatch();
            //            st.Start();
            //_terminate = false;
            //int id = Thread.CurrentThread.ManagedThreadId;
            if (_fillCacheClassList == null)
                _fillCacheClassList = new List<FillAppFabricCache>();
            else
                _fillCacheClassList.Clear();

            initDataCache();

            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;

            Int32 rowcount = 0;
            //for (int i = 0; i < 2; i++)
            //{

            try
            {
                _fillCacheClassList.Add(new FillAppFabricCache());
                rowcount = _fillCacheClassList[0].rowcount();
                if (rowcount == -1)
                {
                    dumpCache();
                    CallBack.RespondWithError("The request was cancelled");
                    //CallBack.CacheOperationComplete();
                    _tokenSource.Dispose();
                    return;
                }
                //break;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                dumpCache();
                CallBack.RespondWithError(ex.Message);
                //CallBack.CacheOperationComplete();
                _tokenSource.Dispose();
                return;
                //Console.WriteLine("Time out, try again ");
            }
            catch (Exception ex)
            {
                dumpCache();
                CallBack.RespondWithError(ex.Message);
                //CallBack.CacheOperationComplete();
                _tokenSource.Dispose();
                return;
                //throw new FaultException(ex.ToString());
                //Console.WriteLine("Time out, try again ");
            }

            _fillCacheClassList.Clear();


//            }

            //            st.Stop();

            CallBack.RespondWithRecordNumbers(rowcount);
            //CallBack.RespondWithRecordNumbers(Thread.CurrentThread.ManagedThreadId);

            if (ct.IsCancellationRequested)
            {
                dumpCache();
                CallBack.RespondWithError("The request was cancelled");
                //CallBack.CacheOperationComplete();
                _tokenSource.Dispose();
                return;
            }

            //Console.WriteLine("Row count: " + rowcount);

            //int limit = 10000;
            var client = new ConfigurationServiceClient();
            int limit;
            int.TryParse(client.getAppSetting("chunkSize"), out limit);
            if (limit == 0)
            {
                dumpCache();
                CallBack.RespondWithError("Chunk size is invalid, press any key to close");
                _tokenSource.Dispose();
                return;
            }

            //int topindex = (int)(rowcount / limit + 1);
            int topindex = (int)(rowcount / limit);
            if (rowcount % limit != 0)
                topindex++;
            //topindex = 100;
            Task[] taskArray = new Task[topindex];
            //Task[] taskArray = new Task[1];
            int offset = 0;

            Stopwatch stw = new Stopwatch();
            stw.Start();

            //bool go = false;
            //if (args != null && args.Length != 0)
            //{
            //    if (Int32.TryParse(args[0], out offset))
            //    {
            //        if (offset < topindex)
            //        {
            //            offset *= limit;
            //            limit = 1000;
            //            taskArray = new Task[10];
            //            limit = 10000;
            //            taskArray = new Task[1];
            //            go = true;
            //        }

            //        //Console.WriteLine(offset);
            //    }

            //    if (!go)
            //    {
            //        CallBack.RespondWithError(" --- Wrong parameter value provided for AppFabricCache ---");
            //        //Console.WriteLine(" --- Wrong parameter value, press any key to close ---");
            //        //Console.ReadKey();
            //        return;
            //    }
            //}

            //SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            //SynchronizationContext Context = SynchronizationContext.Current;
            //SendOrPostCallback dlgt = new SendOrPostCallback(CallDelegate);


            if (_cache.Get("regionNameList") != null)
            {
                ArrayList regionNameList = _cache.Get("regionNameList") as ArrayList;
                foreach (string regionName in regionNameList)
                {
                    if (regionName != null)
                        _cache.RemoveRegion(regionName);
                }
            }

            _cache.Put("fingerList", new ArrayList(), cacheTimeSpan);
            _cache.Put("regionNameList", new ArrayList(), cacheTimeSpan);

//            _tokenSource = new CancellationTokenSource();
//            CancellationToken ct = _tokenSource.Token;

            BlockingCollection<int> bc = new BlockingCollection<int>();

            //if (true)
            //{

                //int i = 0;
                //taskArray = new Task[5];
                for (int i = 0; i < taskArray.Length; i++)
                {
                    //CallBack.RespondWithError(taskArray.Length.ToString());
                    taskArray[i] = Task.Factory.StartNew((Object obj) =>
                    {
                        ct.ThrowIfCancellationRequested();

                        PopulateStateObject state = obj as PopulateStateObject;

                        //if (state.Dlgt == null)
                        //    state.CallBack.RespondWithError("Null");
                        //else
                        //    state.CallBack.RespondWithError("Not Null");

                        var cl = new FillAppFabricCache(state.bc, null, state.fingerList, state.maxPoolSize, state.ct, state.cache);
                        _fillCacheClassList.Add(cl);

                        //var process = new FillAppFabricCache.FillAppFabricCache(state.bc, null, state.fingerList, state.maxPoolSize, state.ct, state.cache);
                        //var process = new FillAppFabricCache.FillAppFabricCache(state.Dlgt, state.Context);
                        //var process = new FillAppFabricCache.FillAppFabricCache();
                        //try
                        //{
                        //process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit - offset, Thread.CurrentThread.ManagedThreadId);
                        //process.run(state.LoopCounter * limit + 90000, state.LoopCounter * limit + limit, limit);

                        cl.run(state.LoopCounter * limit + offset, limit);
                        //cl.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit);
                        //process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit);

                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
                        //Console.WriteLine(process.run(1, 2, Thread.CurrentThread.ManagedThreadId));
                    },
                    new PopulateStateObject() { LoopCounter = i, bc = bc, fingerList = fingerList, maxPoolSize = taskArray.Length, ct = ct, cache = _cache },
                    //new StateObject() { LoopCounter = i, Dlgt = dlgt, CallBack = CallBack, Context = Context },
                    _tokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
                }

                Task.Factory.ContinueWhenAll(taskArray, tasks =>
                {
                    bc.CompleteAdding();

                    //foreach (Task<string> task in tasks)
                    //{
                    //    Console.WriteLine(task.Result);
                    //}
                });

                d d = delegate
                {
                    foreach (var item in bc.GetConsumingEnumerable())
                    {
                        CallBack.RespondWithRecordNumbers(item);
                        //Thread.Sleep(100);
                    }
                };
                d();

                try
                {
                    Task.WaitAll(taskArray);
                    _cache.Put("fingerList", fingerList, cacheTimeSpan);
                    _cache.Put("cacheExpirationTime", DateTime.Now + cacheTimeSpan, cacheTimeSpan);
                }
                catch (Exception ex)
                {
                    foreach (var t in taskArray)
                    {
                        if (t == null)
                            continue;

                        if (t.Status == TaskStatus.Faulted)
                        {
                        //if (ex is System.Data.SqlClient.SqlException)
                        //{
                        //    CallBack.RespondWithError("KUKUKU: " + ex.Message);
                        //    dumpCache();
                        //    _tokenSource.Dispose();
                        //    return;
                        //}
                            bool fault = true;
                            while ((ex is AggregateException) && (ex.InnerException != null))
                            {
                                if (ex.Message.EndsWith("Operation cancelled by user."))
                                {
                                    fault = false;
                                    break;
                                }
                                else if (ex.InnerException.GetType().Name.Equals("TaskCanceledException"))
                                {
                                    if (ex.InnerException.Message.StartsWith("A task was canceled"))
                                    {
                                        fault = false;
                                        break;
                                    }
                                }

                                ex = ex.InnerException;
                            }

                            if (fault)
                            {
                                _tokenSource.Cancel();
                                _tokenSource.Dispose();
                                dumpCache();
                                CallBack.RespondWithError(ex.Message);
                                return;
                            }

                        //ex = ex.InnerException;

                        //}

                        //if (!ex.Message.StartsWith("A task was canceled"))
                        //{
                        //    CallBack.RespondWithError("AAAAAAA: " + ex.Message);
                        //    dumpCache();
                        //    _tokenSource.Dispose();
                        //    return;
                        //}
                        }
                    }
                }
                //finally
                //{
                if (ct.IsCancellationRequested)
                {
                    dumpCache();
                    CallBack.RespondWithError("The request was cancelled");
                }

                _tokenSource.Dispose();
                //}
            //}
            //else
            //{
            //    try
            //    {

            //        //var process = new FillAppFabricCache.FillAppFabricCache(CallBack);
            //        //var process = new FillAppFabricCache.FillAppFabricCache(CallBack);
            //        //var process = new FillAppFabricCache.FillAppFabricCache(dlgt, Context);
            //        var process = new FillAppFabricCache.FillAppFabricCache(null, dlgt, fingerList, ct, _cache);
            //        //process.run(0, 0, 0);
            //        for (int i = 0; i < taskArray.Length; i++)
            //        {
            //            //process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit - offset, Thread.CurrentThread.ManagedThreadId);
            //            //process.run(state.LoopCounter * limit + 90000, state.LoopCounter * limit + limit, limit);

            //            process.run(i * limit + offset, i * limit + limit, limit);
            //        }

            //        //stw.Stop();
            //        //Console.WriteLine(" ----- Count(*) time elapsed: {0}", st.Elapsed);
            //        //Console.WriteLine(" ----- Loop time elapsed: {0}", stw.Elapsed);
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex.Flatten().Message);
            //        //throw ex.Flatten();
            //        while ((ex is AggregateException) && (ex.InnerException != null))
            //            ex = ex.InnerException;

            //        //throw new FaultException(ex.ToString());
            //        CallBack.RespondWithError(ex.ToString());
            //        return;
            //        //Console.WriteLine(ex.ToString());
            //    }
            //    finally
            //    {
            //        _tokenSource.Dispose();
            //        //                    Console.WriteLine(" ------------------ Press any key to close -----------------------");
            //    }
            //}

            stw.Stop();
            TimeSpan ts = stw.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            //ts.Milliseconds / 10);

            ArrayList list = _cache.Get("regionNameList") as ArrayList;
            if (list != null)
            {
                CallBack.RespondWithText(string.Format(" --- Time elapsed: {0}", elapsedTime));

                //ArrayList list = _cache.Get("regionNameList") as ArrayList;
                foreach (string regionName in list)
                {
                    if (regionName == null)
                    {
                        CallBack.RespondWithError(string.Format(" --- region name is null"));
                        break;
                    }
                }
            }

            CallBack.CacheOperationComplete();

            //Console.WriteLine(" ----- Count(*) time elapsed: {0}", st.Elapsed);
            //Console.WriteLine(" ----- Loop time elapsed: {0}", stw.Elapsed);
            //Console.WriteLine(" ------------------ Press any key to close -----------------------");
            //Console.ReadKey();
        }
    }
}
