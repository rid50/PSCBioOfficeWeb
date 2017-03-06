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
        public List<string> fingerList;
        public int          gender;
        //public int          firstMatch;
        //public byte[]       probeTemplate;
        //public int          matchingThreshold;
        public MemoryCache  cache;
        public String       regionName;
        public CancellationToken ct;
        public BioProcessor.BioProcessor matcher;
    }

    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MatchingService : IMatchingService
    {
        private static BioProcessor.BioProcessor _matcher = null;
        private CancellationTokenSource _tokenSource;
        private static MemoryCache _cache;
        private static ConcurrentDictionary<string, CancellationTokenSource> _cd = null;

        private static int sGender= -1;
        private static List<string> sFingerList = new List<string>();
        //private static DataCacheFactory _factory;

        static MatchingService()
        {
            _matcher = new BioProcessor.BioProcessor();

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

        public void Terminate(string guid)
        {
            //return 66;
            //string id = OperationContext.Current.SessionId;
            //var ic = this;

            //int k = 55;
            try
            {
                _tokenSource = _cd[guid];

                if (_tokenSource != null)
                {
                    //k = 33;
                    _tokenSource.Cancel();
                    //_tokenSource = null;
                }
            }
            catch (Exception) { //k = 77;
            }

            //return 0;
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

        public bool verify(byte[] probeTemplate, byte[] galleryTemplate, int matchingThreshold)
        {
            var matcher = new BioProcessor.BioProcessor(MatchingThreshold: matchingThreshold);
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
        //public void enrollGalleryTemplate(List<string> fingerList)
        public MatchingResult match(string guid, List<string> fingerList, int gender, int firstMatch, byte[] probeTemplate, int matchingThreshold)
        {
            //string id = OperationContext.Current.SessionId;
            MatchingResult matchingResult = new MatchingResult();

            List<Tuple<string, int>> list = new List<Tuple<string, int>>();

            if (_matcher.EnrolTask != null && sGender == gender && sFingerList.Except(fingerList).ToList().Count == 0)
            {
                _matcher.enrollProbeTemplate(new ArrayList(fingerList), probeTemplate);
                list = _matcher.identify(firstMatch == 1, matchingThreshold);
                matchingResult.Result = list.OrderByDescending(x => x.Item2).ToList();
                return matchingResult;
            }

            sGender = gender;
            sFingerList = fingerList;

            _tokenSource = new CancellationTokenSource();
            CancellationToken ct = _tokenSource.Token;
            //_cd.TryAdd(guid, _tokenSource);

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
            //List<Task<UInt32>> tasks = new List<Task<UInt32>>();

            //Task<UInt32>[] taskArray = new Task<UInt32>[2];



            //Task.Run(() =>
            //{
            //string regionName = "0";
            //for(int k = 0; k < 2; k++)

            //string regionName = "0";
            //taskArray = new Task<UInt32>[1];
            //int i = 0;

            //regionNameList.Clear();
            //regionNameList.Add("0");
            //regionNameList.Add("0");

            _matcher.DisposeEnrolmentTask();

            List<Task<List<Tuple<string, int>>>> tasks = new List<Task<List<Tuple<string, int>>>>();

            //_matcher = new BioProcessor.BioProcessor(MatchingThreshold: matchingThreshold);

            //try
            //{
                //Task<List<Tuple<string, int>>> task = null;
            foreach (string regionName in regionNameList)
            {
                //if (regionName == null)
                //  continue;

                if (ct.IsCancellationRequested)
                {
                    break;
                    //ct.ThrowIfCancellationRequested();
                }

                //taskArray[i++] = Task.Factory.StartNew((Object obj) =>
                tasks.Add(Task.Factory.StartNew((Object obj) =>
                //task = Task.Factory.StartNew((Object obj) =>
                {
                    //if (ct.IsCancellationRequested)
                    //{
                    //ct.ThrowIfCancellationRequested();
                    //}

                    //if (ct.IsCancellationRequested)
                    //    return list;

                    MatchStateObject state = obj as MatchStateObject;

                    var process = new LookUp(state.matcher, new ArrayList(state.fingerList), state.gender, state.cache, state.ct);

                    //retcode = process.run(state.regionName);

                    List<Tuple<string, int>> list2 = process.run(state.regionName);
                    //if (arrayList.Count != 0 && state.firstMatch == 1 && !_tokenSource.IsCancellationRequested)
                    //{
                    //    _tokenSource.Cancel();
                    //}

                    return list2;
                },
                //new MatchStateObject() { matcher = _matcher, fingerList = fingerList, gender = gender, firstMatch = firstMatch, probeTemplate = probeTemplate, matchingThreshold = matchingThreshold, cache = _cache, regionName = regionName, ct = ct },
                new MatchStateObject() { matcher = _matcher, fingerList = fingerList, gender = gender, cache = _cache, regionName = regionName, ct = ct },
                //_tokenSource.Token,
                ct,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default));
            }

            //Task task = Task.WhenAll(tasks.ToArray());
            //Task task = Task.WhenAll(tasks.ToArray().Where(t => t != null));
            try
            {
                Task.WaitAll(tasks.ToArray());
                //task.Wait();

                //if (task.Status == TaskStatus.RanToCompletion && ((List<Tuple<string, int>>)(task.Result)).Count != 0)
                //{
                //    list.AddRange((List<Tuple<string, int>>)(task.Result));
                //    //break;
                //}
                //}

                //_matcher.enrollProbeTemplate(new ArrayList(fingerList), probeTemplate);
                list.Add(new Tuple<string, int>("", 0));
                list = _matcher.identify(new ArrayList(fingerList), probeTemplate, firstMatch == 1, matchingThreshold);

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

                //foreach (var t in tasks)
                //{
                //    if (t.Status == TaskStatus.RanToCompletion && ((List<Tuple<string, int>>)(t.Result)).Count != 0)
                //    {
                //        list.AddRange((List<Tuple<string, int>>)(t.Result));
                //        //break;
                //    }
                //}

                //return retcode;
            }
            catch (Exception ex)
            {
                foreach (var t in tasks)
                {
                    if (t == null)
                        continue;

                    if (t.Status == TaskStatus.RanToCompletion && ((List<Tuple<string, int>>)(t.Result)).Count != 0)
                    {
                        list = (List<Tuple<string, int>>)(t.Result);
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
                    //_cd.TryRemove(guid, out _tokenSource);
                    _tokenSource = null;
                }

                //if (ct.IsCancellationRequested)
                //{
                //    throw new FaultException("The request was cancelled");
                //}
            }
            //_tokenSource.Dispose();
            //}

            //CallBack.RespondWithText(retcode.ToString());
            //CallBack.MatchingComplete();
            matchingResult.Result = list.OrderByDescending(x => x.Item2).ToList();
            return matchingResult;
            //});

            //return;
        }
    }
}
