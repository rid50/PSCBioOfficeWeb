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
        public short maxPoolSize;
        public BlockingCollection<int> bc;
        public DataCache cache;
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PopulateCacheService : IPopulateCacheService
    {
        private static CancellationTokenSource _tokenSource;
        private static DataCache _cache;
        //private static bool _terminate = false;

        static PopulateCacheService()
        {
            DataCacheFactory factory = new DataCacheFactory();
            _cache = factory.GetCache("default");
            //Debug.Assert(_cache == null);
        }

        static public void CallDelegate(object rowcount)
        {
            CallBack.RespondWithRecordNumbers((int)rowcount);
        }

        SendOrPostCallback dlgt = new SendOrPostCallback(CallDelegate);
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
            return _cache.Get("fingerList") as ArrayList;
        }

        public void Terminate()
        {
            _tokenSource.Cancel();
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


        //public void Run(string[] args)
        public void Run(ArrayList fingerList)
        {
//            Stopwatch st = new Stopwatch();
//            st.Start();
            //_terminate = false;
            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;

            Int32 rowcount = 0;
            //for (int i = 0; i < 2; i++)
            //{
            try
            {
                rowcount = FillAppFabricCache.FillAppFabricCache.rowcount();
                //break;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                dumpCache();
                CallBack.RespondWithError(ex.ToString());
                //CallBack.CacheOperationComplete();
                _tokenSource.Dispose();
                return;
                //Console.WriteLine("Time out, try again ");
            }
            catch (Exception ex)
            {
                dumpCache();
                CallBack.RespondWithError(ex.ToString());
                //CallBack.CacheOperationComplete();
                _tokenSource.Dispose();
                return;
                //throw new FaultException(ex.ToString());
                //Console.WriteLine("Time out, try again ");
            }
//            }

//            st.Stop();

            CallBack.RespondWithRecordNumbers(rowcount);
            //CallBack.RespondWithRecordNumbers(Thread.CurrentThread.ManagedThreadId);

            if (ct.IsCancellationRequested)
            {
                dumpCache();
                CallBack.RespondWithText("The request was cancelled");
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

            int topindex = (int)(rowcount / limit + 1);
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

            _cache.Put("fingerList", new ArrayList(), new TimeSpan(24, 0, 0));
            _cache.Put("regionNameList", new ArrayList(), new TimeSpan(24,0,0));

//            _tokenSource = new CancellationTokenSource();
//            CancellationToken ct = _tokenSource.Token;

            BlockingCollection<int> bc = new BlockingCollection<int>();

            //if (true)
            //{

                for (int i = 0; i < taskArray.Length; i++)
                {
                    //CallBack.RespondWithError(taskArray.Length.ToString());
                    taskArray[i] = Task.Factory.StartNew((Object obj) =>
                    {
                        PopulateStateObject state = obj as PopulateStateObject;

                        //if (state.Dlgt == null)
                        //    state.CallBack.RespondWithError("Null");
                        //else
                        //    state.CallBack.RespondWithError("Not Null");

                        var process = new FillAppFabricCache.FillAppFabricCache(state.bc, null, state.fingerList, state.maxPoolSize, state.ct, state.cache);
                        //var process = new FillAppFabricCache.FillAppFabricCache(state.Dlgt, state.Context);
                        //var process = new FillAppFabricCache.FillAppFabricCache();
                        //try
                        //{
                        //process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit - offset, Thread.CurrentThread.ManagedThreadId);
                        //process.run(state.LoopCounter * limit + 90000, state.LoopCounter * limit + limit, limit);

                        process.run(state.LoopCounter * limit + offset, state.LoopCounter * limit + limit, limit);

                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine(ex.Message);
                        //}
                        //Console.WriteLine(process.run(1, 2, Thread.CurrentThread.ManagedThreadId));
                    },
                    new PopulateStateObject() { LoopCounter = i, bc = bc, fingerList = fingerList, maxPoolSize = (short)taskArray.Length, ct = ct, cache = _cache },
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
                    }
                };
                d();

                try
                {
                    Task.WaitAll(taskArray);
                    _cache.Put("fingerList", fingerList, new TimeSpan(24, 0, 0));
                }
                catch (Exception ex)
                {
                    foreach (var t in taskArray)
                    {
                        if (t.Status == TaskStatus.Faulted)
                        {
                            while ((ex is AggregateException) && (ex.InnerException != null))
                                ex = ex.InnerException;

                            dumpCache();
                            CallBack.RespondWithError(ex.Message);
                            //CallBack.CacheOperationComplete();
                            return;
                            //throw new FaultException(ex.Message);
                        }
                    }
                }
                finally
                {
                    if (ct.IsCancellationRequested)
                    {
                        dumpCache();
                        CallBack.RespondWithText("The request was cancelled");
                    }

                    _tokenSource.Dispose();
                }
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

            if (_cache.Get("regionNameList") != null)
                CallBack.RespondWithText(string.Format(" --- Time elapsed: {0}", elapsedTime));

            CallBack.CacheOperationComplete();

            //Console.WriteLine(" ----- Count(*) time elapsed: {0}", st.Elapsed);
            //Console.WriteLine(" ----- Loop time elapsed: {0}", stw.Elapsed);
            //Console.WriteLine(" ------------------ Press any key to close -----------------------");
            //Console.ReadKey();
        }
    }
}
