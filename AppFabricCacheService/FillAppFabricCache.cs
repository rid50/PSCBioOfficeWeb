﻿using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Microsoft.ApplicationServer.Caching;
using AppFabricCacheService.ConfigurationService;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using System.Text;

namespace FillAppFabricCache
{
    class FillAppFabricCache
    {
        enum FingerListEnum { li, lm, lr, ll, ri, rm, rr, rl, lt, rt }

        //private static DataCacheFactory _factory = null;
        private static CancellationToken _ct;
        private static DataCache _cache;
        private static SendOrPostCallback _callback;
        //private static SynchronizationContext _context = null;
        private ArrayList _fingerList;
        private short _maxPoolSize;
        private static BlockingCollection<int> _bc;

        //static FillAppFabricCache()
        //{
        //    DataCacheFactory factory = new DataCacheFactory();
        //    _cache = factory.GetCache("default");
        //    //Debug.Assert(_cache == null);
        //}

        //public FillAppFabricCache(AppFabricCacheService.IPopulateCacheCallback callback)
        //public FillAppFabricCache(SendOrPostCallback callback, SynchronizationContext context)
        public FillAppFabricCache(BlockingCollection<int> bc, SendOrPostCallback callback, ArrayList fingerList, short maxPoolSize, CancellationToken ct, DataCache cache)
        {
            _bc         = bc;
            _callback   = callback;
            _fingerList = fingerList;
            _maxPoolSize = maxPoolSize;
            _ct         = ct;
            _cache      = cache;
            //_context = context;
        }

        //public AppFabricCacheService.IPopulateCacheCallback CallBack
        //{
        //    get
        //    {
        //        return System.ServiceModel.OperationContext.Current.GetCallbackChannel<AppFabricCacheService.IPopulateCacheCallback>();
        //    }
        //}

        //public void CallDelegate(object rowcount)
        //{
        //    //throw new Exception(Thread.CurrentThread.ManagedThreadId.ToString());
        //    _callback(1000);
        //    //if (CallBack == null)
        //    //    throw new Exception("kuku");
        //    //else
        //    //    throw new Exception("ukuk");

        //    //CallBack.RespondWithRecordNumbers(rowcount);
        //}

        public static Int32 rowcount()
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                var client = new ConfigurationServiceClient();
                conn = new SqlConnection(client.getConnectionString("ConnectionString"));

                //conn = new SqlConnection(getConnectionString());
                conn.Open();
                cmd = new SqlCommand();
                cmd.CommandTimeout = 300;
                cmd.Connection = conn;
                //cmd.CommandText = "SELECT count(*) FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL";
                //cmd.CommandText = "SELECT count(*) FROM Egy_T_FingerPrint";
                cmd.CommandText = "SELECT count(*) FROM " + client.getAppSetting("dbFingerTable");
                reader = cmd.ExecuteReader();
                reader.Read();
                int result = reader.GetInt32(0);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                try
                {
                    if (reader != null)
                        reader.Close();

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    
        //public void run(int from, int to, int count, int threadId)
        //public void run(int from, int to)
        public void run(int from, int to, int count)
        {
            //string fingerFields = "ri,rm,rr,rl";
            //string fingerFields = "li,lm,lr,ll,ri,rm,rr,rl,lt,rt";
            
            var sb = new StringBuilder();
            foreach (string finger in _fingerList)
            {
                sb.Append(finger + ",");
            }

            string fingerFields = sb.ToString();
            fingerFields = fingerFields.Remove(fingerFields.Length - 1);
            string[] fingerFieldsArray = fingerFields.Split(new char[] { ',' });

            //string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
            //string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];
            //string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];

            var client = new ConfigurationServiceClient();

            string dbFingerTable = client.getAppSetting("dbFingerTable");
            string dbIdColumn = client.getAppSetting("dbIdColumn");

            //return;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            //byte[] buffer = new byte[0];
            byte[][] buffer = new byte[10][];
            int id = 0;
            int rowNumber = 0;

            Stopwatch sw = new Stopwatch();
            //Stopwatch stwd = new Stopwatch();
            //Stopwatch stws = new Stopwatch();
            //stw.Start();
            //stwd.Start();
            //stws.Start();

            //foreach (string region in _cache.GetSystemRegions())
            //{
            //    foreach (var kvp in _cache.GetObjectsInRegion(region))
            //    {
            //        Console.WriteLine("data item ('{0}','{1}') in region {2} of cache {3}", kvp.Key, kvp.Value.ToString(), region, "default");
            //    }
            //}
            //return;            

            //if (_cache.Get("fingerList") == null)
            //    _cache.Add("fingerList", _fingerList);

            string regionName = from.ToString();
            _cache.RemoveRegion(regionName);
            _cache.CreateRegion(regionName);

            //ArrayList regionNameList;

            //if (_cache.Get("regionNameList") == null)
            //{
            //    regionNameList = new ArrayList();
            //    _cache.Add("regionNameList", regionNameList);
            //}

            ArrayList regionNameList = _cache.Get("regionNameList") as ArrayList;
            regionNameList.Add(regionName);
            _cache.Put("regionNameList", regionNameList, new TimeSpan(24, 0, 0));

            try
            {
                //conn = buildConnectionString();
                //conn = new SqlConnection(client.getConnectionString("ConnectionString"));

                var connStr = client.getConnectionString("ConnectionString");
                connStr += String.Format(";Max Pool Size={0}", _maxPoolSize);
                conn = new SqlConnection(connStr);

                //var connStr = getConnectionString();
                //conn = new SqlConnection(connStr);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;

                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE AppID = 20095420";

                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE datalength(" + dbFingerColumn + ") IS NOT NULL";
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM (SELECT ROW_NUMBER() OVER(ORDER BY AppID) AS row, AppID, AppWsq FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL) r WHERE row > {0} and row <= {1}", from, to);
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = String.Format("SELECT AppID," + fingerFields + " FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);


                //cmd.CommandText = "SELECT " + dbIdColumn + "," + fingerFields + " FROM " + dbFingerTable;
                cmd.CommandText = String.Format("SELECT " + dbIdColumn + "," + fingerFields + " FROM " + dbFingerTable + " WITH (NOLOCK) ORDER BY " + dbIdColumn + " ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = String.Format("SELECT AppID, " + fingerFields + " FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                
                
                //cmd.CommandText = String.Format("SELECT " + fingerFields + " FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = "SELECT AppID, AppWsq FROM Egy_T_FingerPrint WHERE AppID = 20095423";

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (_ct.IsCancellationRequested)
                    {
                        _ct.ThrowIfCancellationRequested();
                    }

                    id = (int)reader[dbIdColumn];

                    rowNumber++;

                    if (rowNumber % 1000 == 0)
                    {
                        if (_bc != null)
                            _bc.Add(1000);
                        else
                            _callback(1000);

                        //this.Invoke((Action<AppFabricCacheService.IPopulateCacheCallback>)((callback) =>
                        //{
                        //    callback.RespondWithRecordNumbers(1000);
                        //}), _callback);
                        //throw new Exception(_callback.GetType().ToString());

                        //string conn_name = "foo";
                        //uiContext.Post(new SendOrPostCallback((o) =>
                        //{
                        //    updateConnStatus(conn_name, true);
                        //}), null);

                        //_context.Post(_callback, 1000);


                        //var task = Task.Factory.StartNew(() =>
                        //{
                        //    //int ii = 5 + 5;
                        //    //throw new Exception("aaaaaaaaaaa: " + Thread.CurrentThread.ManagedThreadId);
                        //    CallDelegate(1000);
                        //}, CancellationToken.None, TaskCreationOptions.None, _context);

                        //task.Wait();

                        //_context.Send(state => { AppFabricCacheService.PopulateCacheService.CallBack.RespondWithRecordNumbers((int)state); }, state: 1000);
                        //_context.Send(state => { AppFabricCacheService.PopulateCacheService.CallDelegate(state); }, state: 1000);

                        //_context.Post(CallDelegate, 1000);
                        //_callback(1000);

                        //CallBack.RespondWithRecordNumbers(1000);
                    }
                    //Console.WriteLine("{0}", rowNumber + from);
                    //Console.WriteLine("ID = {0}", id);
                    //if (id == 20000007)
                    //    id = id;


                    //if (!(reader.IsDBNull(1) && reader.IsDBNull(2) && reader.IsDBNull(3) && reader.IsDBNull(4) && reader.IsDBNull(5)
                    //      && reader.IsDBNull(6) && reader.IsDBNull(7) && reader.IsDBNull(8) && reader.IsDBNull(9) && reader.IsDBNull(10)
                    //     )
                    //   )
                    //if (!reader.IsDBNull(1))
//                    {
                        //                        id = (int)reader[dbIdColumn];
                        bool approved = false, confirmed = false;
                        int i = 0;
                        foreach (string finger in fingerFieldsArray)
                        {
                            FingerListEnum f = (FingerListEnum)Enum.Parse(typeof(FingerListEnum), finger);
                            if (!reader.IsDBNull(i + 1) && ((byte[])reader[finger]).Length > 1)
                            {
                                if (!approved)
                                    approved = true;
                                else if (approved && !confirmed)
                                    confirmed = true;

                                buffer[(int)f] = (byte[])reader[finger];
                            }
                            else
                                buffer[(int)f] = new byte[0];
                            
                            i++;
                        }

                        if (confirmed)
                            _cache.Add(id.ToString(), buffer, new TimeSpan(24, 0, 0), regionName);
//                    }
                    //else
                    //{
                    //    Console.WriteLine("NULL {0}", id);
                    //}
                }


                //int k = 0;
                //foreach (var cacheItem in _cache.GetObjectsInRegion(regionName))
                //{
                //    k++;
                //    //Console.WriteLine("Key={0} -- Value ={1}", cacheItem.Key, cacheItem.Value);
                //}

                //Console.WriteLine("{0}", rowNumber + from);
                //Console.WriteLine("Objects in Cache = {0}", k);

            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                         _ct.ThrowIfCancellationRequested();
                    else
                        throw new Exception(e.Message);
                }
            }
            finally
            {
                try
                {
                    if (reader != null)
                        reader.Close();

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        static private String getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }
    }
}
