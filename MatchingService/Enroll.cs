﻿using MatchingService.ConfigurationService;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace MatchingService
{
    public class Enroll
    {
        private static System.Object theLock = new System.Object();

        enum FingerListEnum { li, lm, lr, ll, ri, rm, rr, rl, lt, rt }

        //private static DataCacheFactory _factory = null;
        private CancellationToken _ct;
        //private static MemoryCache _cache;
        private static SendOrPostCallback _callback;
        //private static SynchronizationContext _context = null;
        private ArrayList _fingerList;
        private static int _maxPoolSize;
        private static BlockingCollection<int> _bc;
        private BioProcessor.BioProcessor _matcher;

        //static FillAppFabricCache()
        //{
        //    DataCacheFactory factory = new DataCacheFactory();
        //    _cache = factory.GetCache("default");
        //    //Debug.Assert(_cache == null);
        //}

        //public FillAppFabricCache(AppFabricCacheService.IPopulateCacheCallback callback)
        //public FillAppFabricCache(SendOrPostCallback callback, SynchronizationContext context)
        public Enroll() { }

        public Enroll(BlockingCollection<int> bc, SendOrPostCallback callback, ArrayList fingerList, int maxPoolSize, CancellationToken ct, BioProcessor.BioProcessor matcher)
        {
            _bc = bc;
            _callback = callback;
            _fingerList = fingerList;
            _maxPoolSize = maxPoolSize;
            _ct = ct;
            _matcher = matcher;
            //_cache = cache;
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

        private SqlCommand _command;

        public SqlCommand cmd
        {
            get { return _command; }
            set { _command = value; }
        }


        public Int32 RowCount()
        {
            SqlConnection conn = null;
            //SqlCommand cmd = null;
            SqlDataReader reader = null;
            int result = -1;
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
                if (reader.HasRows)
                {
                    reader.Read();
                    result = reader.GetInt32(0);
                }

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
        //public void run(int from, int to, int count)
        public void run(int from, int count)
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
            string dbPictureTable = client.getAppSetting("dbPictureTable");
            string dbGenderColumn = client.getAppSetting("dbGenderColumn");

            //return;

            //byte[] buffer = new byte[0];
            //byte[][] buffer = new byte[10][];
            byte[][] buffer;
            int id = 0; bool gender;
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
            //_cache.Remove(regionName);
            //_cache.RemoveRegion(regionName);
            //_cache.CreateRegion(regionName);

            //ArrayList regionNameList;

            //if (_cache.Get("regionNameList") == null)
            //{
            //    regionNameList = new ArrayList();
            //    _cache.Add("regionNameList", regionNameList);
            //}
            //lock (theLock)
            //{
            //    ArrayList regionNameList = _cache.Get("regionNameList") as ArrayList;
            //    regionNameList.Add(regionName);
            //    _cache.Set("regionNameList", regionNameList, new DateTimeOffset(DateTime.Now).AddDays(1));
            //    //_cache.Put("regionNameList", regionNameList, new TimeSpan(24, 0, 0));
            //}
            //try
            //{
            //conn = buildConnectionString();
            //conn = new SqlConnection(client.getConnectionString("ConnectionString"));

            var connectionString = client.getConnectionString("ConnectionString");
            connectionString += String.Format(";Connect Timeout=0;Pooling=true;Min Pool Size=1;Max Pool Size={0}", _maxPoolSize);
            //conn = new SqlConnection(connectionString);

            //conn = new SqlConnection(connectionString);

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            //using (SqlConnection conn = new SqlConnection(connectionString))
            try
            {
                //var connStr = getConnectionString();
                //conn = new SqlConnection(connStr);
                conn = new SqlConnection(connectionString);
                conn.Open();
                cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                cmd.Connection = conn;

                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE AppID = 20095420";

                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE datalength(" + dbFingerColumn + ") IS NOT NULL";
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM (SELECT ROW_NUMBER() OVER(ORDER BY AppID) AS row, AppID, AppWsq FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL) r WHERE row > {0} and row <= {1}", from, to);
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = String.Format("SELECT AppID," + fingerFields + " FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);


                //cmd.CommandText = "SELECT " + dbIdColumn + "," + fingerFields + " FROM " + dbFingerTable;
                //cmd.CommandText = String.Format("SELECT " + dbIdColumn + "," + fingerFields + " FROM " + dbFingerTable + " WITH (NOLOCK) ORDER BY " + dbIdColumn + " ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = String.Format("SELECT A." + dbIdColumn + ", B." + dbGenderColumn + ", " + fingerFields + " FROM " + dbFingerTable + " As A WITH (NOLOCK) INNER JOIN " + dbPictureTable  + " ORDER BY " + dbIdColumn + " ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);

                cmd.CommandText = String.Format("SELECT A.{0} As Id, B.{1} As Gender, {2} FROM {3} As A WITH(NOLOCK) INNER JOIN {4} As B ON A.{0} = B.{0} ORDER BY A.{0} ASC OFFSET {5} ROWS FETCH NEXT {6} ROWS ONLY ",
                                                        dbIdColumn, dbGenderColumn, fingerFields, dbFingerTable, dbPictureTable, from, count);
                //cmd.CommandText = String.Format("SELECT AppID, " + fingerFields + " FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);


                //cmd.CommandText = String.Format("SELECT " + fingerFields + " FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
                //cmd.CommandText = "SELECT AppID, AppWsq FROM Egy_T_FingerPrint WHERE AppID = 20095423";

                //using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //if (_ct.IsCancellationRequested)
                            //{
                            //    break;
                            //}
                            //Thread.Sleep(100);
                            _ct.ThrowIfCancellationRequested();

                            //id = (int)reader[dbIdColumn];
                            id = (int)reader["Id"];
                            gender = (bool)reader["Gender"];
                            //int k = 0;
                            //if (id == 20005140)
                            //    k = 1;

                            rowNumber++;

                            if (rowNumber % 100 == 0)
                            {
                                if (_bc != null)
                                    _bc.Add(100);
                                else
                                    _callback(100);


                            }

                            buffer = new byte[10][];
                            bool approved = false, confirmed = false;   // this two variables asure that no less than 2 finger templates will be saved in memry cache
                            int i = 2; // 0 - Id column, 1 - Gender column, 2 is the first finger column we are interested in
                            foreach (string finger in fingerFieldsArray)
                            {
                                FingerListEnum f = (FingerListEnum)Enum.Parse(typeof(FingerListEnum), finger);
                                if (!reader.IsDBNull(i) && ((byte[])reader[finger]).Length > 1)
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

                            //if (id.ToString() == "20005140")
                            //{
                            //    int k = 0;

                            //}
                            if (confirmed)
                                _matcher.enrollGalleryTemplate(_fingerList, buffer, id.ToString());
                                //_cache.Set(regionName + (gender ? "m" : "w") + id.ToString(), buffer, new DateTimeOffset(DateTime.Now).AddDays(1));

                            //_cache.Add(id.ToString() + (gender ? "m" : "w"), buffer, new DateTimeOffset(DateTime.Now, new TimeSpan(24, 0, 0)), regionName);

                            //byte[][] buffer2 = new byte[10][];
                            //buffer2 = _cache.Get(regionName + (gender ? "m" : "w") + id.ToString()) as byte[][];

                            //                    }
                            //else
                            //{
                            //    Console.WriteLine("NULL {0}", id);
                            //}
                        }

                        //if (reader != null)
                        //{
                        //cmd.Cancel();
                        //cmd = null;
                        //reader.Close();
                        //}
                    }
                }

                //if (conn.State == ConnectionState.Open)
                //{
                //conn.Close();
                //}
            }
            finally
            {
                //try
                //{
                if (cmd != null)
                    cmd.Cancel();

                if (reader != null)
                    reader.Close();

                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    //conn = null;
                    //conn.Dispose();
                    //SqlConnection.ClearPool(conn);
                }
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(ex.Message);
                //}
            }

            //if (_ct.IsCancellationRequested)
            //    return;
            //_ct.ThrowIfCancellationRequested();
        }

        //static private String getConnectionString()
        //{
        //    return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        //}
    }
}