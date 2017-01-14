using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;
//using Microsoft.ApplicationServer.Caching;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Caching;
using System.ServiceModel.Activation;
using System.Collections.Concurrent;

namespace MemoryCacheService
{
    class MatchStateObject
    {
        public ArrayList    fingerList;
        public int          gender;
        public byte[]       probeTemplate;
        public MemoryCache  cache;
        public String       regionName;
        public CancellationToken ct;
    }

    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MatchingService : IMatchingService
    {
        private CancellationTokenSource _tokenSource;
        private static MemoryCache _cache;
        private static ConcurrentDictionary<string, CancellationTokenSource> _cd = null;
        //private static DataCacheFactory _factory;

        static MatchingService()
        {
            //DataCacheFactory factory = new DataCacheFactory();
            //_cache = factory.GetCache("default");
            _cache = MemoryCache.Default;
            //Debug.Assert(_cache == null);
            int initialCapacity = 101;

            // The higher the concurrencyLevel, the higher the theoretical number of operations
            // that could be performed concurrently on the ConcurrentDictionary.  However, global
            // operations like resizing the dictionary take longer as the concurrencyLevel rises. 
            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;

            // Construct the dictionary with the desired concurrencyLevel and initialCapacity
            _cd = new ConcurrentDictionary<string, CancellationTokenSource>(concurrencyLevel, initialCapacity);
        }

        //public IMatchingCallback CallBack
        //{
        //    get
        //    {
        //        return OperationContext.Current.GetCallbackChannel<IMatchingCallback>();
        //    }
        //}

        public ArrayList getFingerList()
        {
            string id = OperationContext.Current.SessionId;
            //_cache = _factory.GetCache("default");
            return _cache.Get("fingerList") as ArrayList;
        }

        public int Terminate(string guid)
        {
            //return 66;
            string id = OperationContext.Current.SessionId;
            var ic = this;

            int k = 55;
            try
            {
                _tokenSource = _cd[guid];

                if (_tokenSource != null)
                {
                    k = 33;
                    _tokenSource.Cancel();
                    //_tokenSource = null;
                }
            }
            catch (Exception) { k = 77; }

            return k;
        }

        //public async Task<int> Terminate()
        //{
        //    return await Task.Factory.StartNew(() =>
        //    {
        //        int k = 55;
        //        try
        //        {
        //            if (_tokenSource != null)
        //            {
        //                k = 33;
        //                _tokenSource.Cancel();
        //            }
        //        }
        //        catch (Exception) { k = 77; }

        //        return k;

        //    });
        //}

        //private int TerminateOperation(object state)
        //{
        //    try
        //    {
        //        if (_tokenSource != null)
        //            _tokenSource.Cancel();
        //    }
        //    catch (Exception) { }

        //    return 0;
        //}

        //public IAsyncResult BeginTerminate(int msg, AsyncCallback callback, object asyncState)
        //{
        //    var task = Task<int>.Factory.StartNew(TerminateOperation, asyncState);

        //    //try
        //    //{
        //    //    if (_tokenSource != null)
        //    //        _tokenSource.Cancel();
        //    //}
        //    //catch (Exception) { }

        //    return task.ContinueWith(res => callback(task));
        //    //return new CompletedAsyncResult<string>("");
        //    //throw new NotImplementedException();
        //}

        //public void EndTerminate(IAsyncResult r)
        //{
        //    return;
        //    //throw new NotImplementedException();
        //}

        //public IAsyncResult BeginTeminate(string msg, AsyncCallback callback, object asyncState)
        //{
        //    try
        //    {
        //        if (_tokenSource != null)
        //            _tokenSource.Cancel();
        //    }
        //    catch (Exception) { }

        //    return new CompletedAsyncResult<string>("");
        //}

        //public void EndTeminate(IAsyncResult r)
        //{
        //    throw new NotImplementedException();
        //    //CompletedAsyncResult<string> result = r as CompletedAsyncResult<string>;
        //    //return result.Data;
        //}

        public bool verify(byte[] probeTemplate, byte[] galleryTemplate)
        {
            var matcher = new BioProcessor.BioProcessor();
            return matcher.verify(probeTemplate, galleryTemplate);
        }

        //public async Task<int> match2(ArrayList fingerList, int gender, byte[] probeTemplate)
        //{
        //    return;
        //}

        //public void match2(ArrayList fingerList, int gender, byte[] probeTemplate)
        //{
        //    return;
        //}
        //[OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.BeforeCall)]
        public UInt32 match(string guid, ArrayList fingerList, int gender, byte[] probeTemplate)
        {

            string id = OperationContext.Current.SessionId;

            UInt32 retcode = 0;

            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;
            _cd.TryAdd(guid, _tokenSource);

            //Task ta = Task.Factory.StartNew(() =>
            //{

            //    //source.Cancel();

            //    //var delay = Task.Run(async () => {
            //    //    await Task.Delay(5000);
            //    //    int k = 0;
            //    //});

            //    Task.Delay(10000).Wait();

            //    //if (ct.IsCancellationRequested)
            //    //  throw new Exception("kuku");

            //    ct.ThrowIfCancellationRequested();

            //    //Console.WriteLine("MainTask {0} Thread={1}", i, Thread.CurrentThread.ManagedThreadId);
            //    //Console.WriteLine("Thread={0}", Thread.CurrentThread.ManagedThreadId);
            //    //String str = new SyncTasks().HelloAsync(i.ToString());

            //    //Console.WriteLine(str);
            //    //return 0;

            //}, _tokenSource.Token);

            //try
            //{
            //    ta.Wait();
            //}
            //catch (Exception) { }

            //if (_tokenSource != null)
            //{
            //    _tokenSource.Cancel();
            //    _tokenSource = null;
            //}

            //return retcode = 20005140;

            ArrayList regionNameList = new ArrayList();

            //_tokenSource = tokenSource;

            if (_cache.Get("regionNameList") != null)
            {
                regionNameList = _cache.Get("regionNameList") as ArrayList;
            }
            else
            {
                //CallBack.RespondWithError("Cache is empty");
                //return;
                throw new FaultException("Cache is empty");
            }

            //Task<UInt32>[] tasks = new Task<UInt32>[regionNameList.Count];
            List<Task<UInt32>> tasks = new List<Task<UInt32>>();
            //Task<UInt32>[] taskArray = new Task<UInt32>[2];



            //Task.Run(() =>
            //{
                //string regionName = "0";
                //for(int k = 0; k < 2; k++)

                //string regionName = "0";
                //taskArray = new Task<UInt32>[1];
                //int i = 0;
            foreach (string regionName in regionNameList)
            {
                //if (regionName == null)
                //  continue;

                //taskArray[i++] = Task.Factory.StartNew((Object obj) =>
                tasks.Add(Task.Factory.StartNew((Object obj) =>
                {
                    //if (ct.IsCancellationRequested)
                    //{
                    //ct.ThrowIfCancellationRequested();
                    //}

                    if (ct.IsCancellationRequested)
                        return retcode;

                    MatchStateObject state = obj as MatchStateObject;

                    var process = new LookUp(state.fingerList, state.gender, state.probeTemplate, state.cache, state.ct);

                    //retcode = process.run(state.regionName);

                    retcode = process.run(state.regionName);
                    if (retcode != 0 && !_tokenSource.IsCancellationRequested)
                    {
                        _tokenSource.Cancel();
                    }

                    return retcode;
                },
                new MatchStateObject() { fingerList = fingerList, gender = gender, probeTemplate = probeTemplate, cache = _cache, regionName = regionName, ct = ct },
                //_tokenSource.Token,
                ct,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default));
            }

            Task task = Task.WhenAll(tasks.ToArray());
            //Task task = Task.WhenAll(tasks.ToArray().Where(t => t != null));
            try
            {
                task.Wait();
                //Task.WaitAll(taskArray);
                //await task;

                //Action myAction = () =>
                //{
                //    Task.WaitAll(taskArray);
                //    //Task.WhenAll(taskArray);
                //};

                //IAsyncResult result = myAction.BeginInvoke(null, null);

                //// Poll while simulating work.
                ////while (!result.IsCompleted)
                ////{
                ////    Task.Delay(100);

                ////    //Task.Yield();
                ////    //Thread.Yield();
                ////}

                //myAction.EndInvoke(result);

                foreach (var t in tasks)
                {
                    if (t.Status == TaskStatus.RanToCompletion && (UInt32)t.Result != 0)
                    {
                        retcode = (UInt32)t.Result;
                        break;
                    }
                }

                //return retcode;
            }
            catch (Exception ex)
            {
                foreach (var t in tasks)
                {
                    if (t == null)
                        continue;

                    if (t.Status == TaskStatus.RanToCompletion && (int)t.Result != 0)
                    {
                        retcode = (UInt32)t.Result;
                        break;
                    }
                    else if (t.Status == TaskStatus.Faulted || t.Status == TaskStatus.Running)
                    {
                        bool fault = true;
                        if (ex.Message.Equals("The operation was canceled."))
                        {
                            fault = true;
                            //continue;
                        }

                        //while ((ex is AggregateException) && (ex.InnerException != null))
                        while (ex.InnerException != null)
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

                            //ex = ex.InnerException;

                            if (ex.Message.Equals("The operation was canceled."))
                            {
                                fault = true;
                                //break;
                            }
                        }

                        if (fault)
                        {
                            if (_tokenSource != null)
                                _tokenSource.Cancel();
                            //_tokenSource.Dispose();
                            //CallBack.RespondWithError(ex.Message);
                            //return;
                            throw new FaultException(ex.Message);
                        }
                    }
                }
            }
            finally
            {
                if (_tokenSource != null)
                {
                    _tokenSource.Dispose();
                    _cd.TryRemove(guid, out _tokenSource);
                    _tokenSource = null;
                }
                //if (ct.IsCancellationRequested)
                //{
                //    throw new Exception("The request was cancelled");
            }

            //_tokenSource.Dispose();
            //}

            //CallBack.RespondWithText(retcode.ToString());
            //CallBack.MatchingComplete();
            return retcode;
            //});

            //return;
        }
    }
}
