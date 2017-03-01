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
//using Microsoft.ApplicationServer.Caching;
using MemoryCacheService.ConfigurationService;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Caching;

namespace MemoryCacheService
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
        public MemoryCache cache;
    }

    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class PopulateCacheService : IPopulateCacheService
    {
        private static CancellationTokenSource _tokenSource;
        private static MemoryCache _cache;
        private static List<FillMemoryCache> _fillCacheClassList;

        //private static DateTimeOffset cacheTimeSpan = new DateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)).AddDays(1);
        private static DateTimeOffset cacheTimeSpan = new DateTimeOffset(DateTime.Now).AddDays(1);
        //private static TimeSpan cacheTimeSpan = new TimeSpan(24, 0, 0);
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

        private MemoryCache initDataCache()
        {
            try
            {
                if (_cache == null)
                {
                    _cache = MemoryCache.Default;
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
            //var list = new ArrayList();
            //list.Add("li");
            //list.Add("lm");

            //return list;
            //throw new FaultException(new FaultReason("kuku"));

            try {
                return initDataCache().Get("fingerList") as ArrayList;
            }
            catch (Exception ex)
            {
                throw new FaultException(new FaultReason(ex.Message));
            }
        }

        public DateTime getExpirationTime()
        {
            try
            {
                Object obj = initDataCache().Get("cacheExpirationTime");
                if (obj != null)
                    return ((DateTimeOffset)obj).DateTime;
                else
                    return new DateTime(0);
            }
            catch (Exception ex)
            {
                throw new FaultException(new FaultReason(ex.Message));
            }
        }

        public int Terminate()
        {
            //_tokenSource.Cancel();

            int k = 55;
            try
            {
                if (_tokenSource != null)
                {
                    k = 33;
                    _tokenSource.Cancel();
                    //_tokenSource = null;
                }
            }
            catch (Exception) { k = 77; 
            }

            //int id = Thread.CurrentThread.ManagedThreadId;
            foreach (var fillCacheClass in _fillCacheClassList)
            {
                if (fillCacheClass.cmd != null)
                    fillCacheClass.cmd.Cancel();
            }

            return k;
        }

        private void dumpCache() {
            if (_cache.Get("regionNameList") != null)
            {
                ArrayList regionNameList = _cache.Get("regionNameList") as ArrayList;
                foreach (string regionName in regionNameList)
                {
                    if (regionName != null)
                        _cache.Remove(regionName);
                        //_cache.RemoveRegion(regionName);

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
                _fillCacheClassList = new List<FillMemoryCache>();
            else
                _fillCacheClassList.Clear();

            initDataCache();

            //_tokenSource = new CancellationTokenSource();
            //CancellationToken ct = _tokenSource.Token;

            Int32 rowcount = 0;
            //for (int i = 0; i < 2; i++)
            //{

            try
            {
                _fillCacheClassList.Add(new FillMemoryCache());
                rowcount = _fillCacheClassList[0].rowcount();
                if (rowcount == -1)
                {
                    dumpCache();
                    CallBack.RespondWithError("The request was cancelled");
                    //CallBack.CacheOperationComplete();
                    //_tokenSource.Dispose();
                    //_tokenSource = null;
                    return;
                }
                //break;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                dumpCache();
                CallBack.RespondWithError(ex.Message);
                //CallBack.CacheOperationComplete();
                //_tokenSource.Dispose();
                //_tokenSource = null;
                return;
                //Console.WriteLine("Time out, try again ");
            }
            catch (Exception ex)
            {
                dumpCache();
                CallBack.RespondWithError(ex.Message);
                //CallBack.CacheOperationComplete();
                //_tokenSource.Dispose();
                //_tokenSource = null;
                return;
                //throw new FaultException(ex.ToString());
                //Console.WriteLine("Time out, try again ");
            }

            _fillCacheClassList.Clear();


//            }

            //            st.Stop();

            CallBack.RespondWithRecordNumbers(rowcount);
            //CallBack.RespondWithRecordNumbers(Thread.CurrentThread.ManagedThreadId);

            //_tokenSource = new CancellationTokenSource();
            //CancellationToken ct = _tokenSource.Token;

            //if (ct.IsCancellationRequested)
            //{
            //    dumpCache();
            //    CallBack.RespondWithError("The request was cancelled");
            //    //CallBack.CacheOperationComplete();
            //    _tokenSource.Dispose();
            //    _tokenSource = null;
            //    return;
            //}

            //Console.WriteLine("Row count: " + rowcount);

            //int limit = 10000;
            var client = new ConfigurationServiceClient();
            int limit;
            int.TryParse(client.getAppSetting("chunkSize"), out limit);
            if (limit == 0)
            {
                dumpCache();
                CallBack.RespondWithError("Chunk size is invalid, press any key to close");
                //_tokenSource.Dispose();
                //_tokenSource = null;
                return;
            }

            //int topindex = (int)(rowcount / limit + 1);
            int topindex = (int)(rowcount / limit);
            if (rowcount % limit != 0)
                topindex++;
            //topindex = 100;
            Task[] tasks = new Task[topindex];
            //Task[] taskArray = new Task[1];
            int offset = 0;


            //int diff = rowcount % Environment.ProcessorCount;
            //if (diff != 0)
            //    rowcount += Environment.ProcessorCount - diff;

            //limit = rowcount / Environment.ProcessorCount;
            //Task[] tasks = new Task[Environment.ProcessorCount];

            //limit = rowcount;
            //Task[] tasks = new Task[1];

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

            var items = MemoryCache.Default.Select(x => x.Key);
            foreach (var key in items)
                MemoryCache.Default.Remove(key);

/*
            if (_cache.Get("regionNameList") != null)
            {
                ArrayList regionNameList = _cache.Get("regionNameList") as ArrayList;
                foreach (string regionName in regionNameList)
                {
                    if (regionName != null)
                        _cache.Remove(regionName);
                        //_cache.RemoveRegion(regionName);
                }
            }
*/
            _cache.Set("fingerList", new ArrayList(), cacheTimeSpan);
            _cache.Set("regionNameList", new ArrayList(), cacheTimeSpan);
            //_cache.Put("fingerList", new ArrayList(), cacheTimeSpan);
            //_cache.Put("regionNameList", new ArrayList(), cacheTimeSpan);

            //            _tokenSource = new CancellationTokenSource();
            //            CancellationToken ct = _tokenSource.Token;
            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;

            BlockingCollection<int> bc = new BlockingCollection<int>();

            //if (true)
            //{

                //int i = 0;
                //taskArray = new Task[1];
            for (int i = 0; i < tasks.Length; i++)
            {
                //CallBack.RespondWithError(taskArray.Length.ToString());
                tasks[i] = Task.Factory.StartNew((Object obj) =>
                {
                    //ct.ThrowIfCancellationRequested();
                    if (ct.IsCancellationRequested)
                        return;

                    PopulateStateObject state = obj as PopulateStateObject;

                    //if (state.Dlgt == null)
                    //    state.CallBack.RespondWithError("Null");
                    //else
                    //    state.CallBack.RespondWithError("Not Null");

                    var cl = new FillMemoryCache(state.bc, null, state.fingerList, state.maxPoolSize, state.ct, state.cache);
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
                new PopulateStateObject() { LoopCounter = i, bc = bc, fingerList = fingerList, maxPoolSize = tasks.Length, ct = ct, cache = _cache },
                //new StateObject() { LoopCounter = i, Dlgt = dlgt, CallBack = CallBack, Context = Context },
                ct,
                //_tokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            }

            Task.Factory.ContinueWhenAll(tasks, task =>
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
                    if (!ct.IsCancellationRequested)
                    {
                        CallBack.RespondWithRecordNumbers(item);
                    }
                    else
                    {
                        bc.CompleteAdding();
                        break;
                    }
                    ////Thread.Yield();
                }
            };

            d();
            //d.BeginInvoke(null, null);

            Task ta = Task.WhenAll(tasks);

            //            d();

            try
            {
                ta.Wait();
                //Action myAction = () =>
                //{
                //    Task.WaitAll(tasks, ct);
                //};


                //IAsyncResult result = myAction.BeginInvoke(null, null);
                //Thread.Yield();
                //Thread.Sleep(100);

                //d();



                ////Task.WaitAll(tasks);

                //myAction.EndInvoke(result);

                //int k = taskArray.Length;

                //for (int i = 0; i < taskArray.Length; i++)
                //{
                //    if (taskArray[i].IsCompleted) k--;
                //    if (k == 0)
                //        break;

                //    d();
                //}


                _cache.Set("fingerList", fingerList, cacheTimeSpan);
                _cache.Set("cacheExpirationTime", cacheTimeSpan, cacheTimeSpan);
            }
            catch (Exception ex)
            {
                foreach (var t in tasks)
                {
                    if (t == null)
                        continue;

                    if (t.Status == TaskStatus.Faulted || t.Status == TaskStatus.Canceled)
                    {
                        //if (ex is System.Data.SqlClient.SqlException)
                        //{
                        //    CallBack.RespondWithError("KUKUKU: " + ex.Message);
                        //    dumpCache();
                        //    _tokenSource.Dispose();
                        //    return;
                        //}
                        //bool fault = true;
                        while ((ex is AggregateException) && (ex.InnerException != null))
                        {
                            //if (ex.Message.EndsWith("Operation cancelled by user."))
                            //{
                            //    //fault = false;
                            //    break;
                            //}
                            //else if (ex.InnerException.GetType().Name.Equals("TaskCanceledException"))
                            //{
                            //    if (ex.InnerException.Message.StartsWith("A task was canceled"))
                            //    {
                            //        //fault = false;
                            //        break;
                            //    }
                            //}

                            ex = ex.InnerException;
                        }

//                        if (fault)
                        {
                            _tokenSource.Cancel();
                            //_tokenSource.Dispose();
                            //_tokenSource = null;
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
            finally
            {
                if (ct.IsCancellationRequested)
                {
                    dumpCache();
                    CallBack.RespondWithError("The request was cancelled");
                }

                _tokenSource.Dispose();
                _tokenSource = null;

            }


            //_tokenSource.Dispose();
            //_tokenSource = null;

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
