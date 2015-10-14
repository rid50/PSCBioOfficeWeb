using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Neurotec.Images;
using Neurotec.Biometrics;

namespace SplitFingerTemplates
{
    //public static class MyConnection
    //{
    //    private static SqlConnection conn = null;
    //    private static SqlConnection conn2 = null;

    //    static public SqlConnection Connection
    //    {
    //        //conn = new SqlConnection(getConnectionString());
    //        //conn.Open();
    //        get
    //        {
    //            return conn;
    //        }
    //    }

    //    static public SqlConnection Connection2
    //    {
    //        //conn = new SqlConnection(getConnectionString());
    //        //conn.Open();
    //        get
    //        {
    //            return conn2;
    //        }
    //    }

    //    static MyConnection()
    //    {
    //        try {
    //            conn = new SqlConnection(getConnectionString());
    //            conn.Open();
    //            conn2 = new SqlConnection(getConnectionString());
    //            conn2.Open();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception(ex.Message);
    //        }
    //    }

    //    //~Connection2()
    //    //{
    //    //    if (conn != null && conn.State == ConnectionState.Open)
    //    //    {
    //    //        conn.Close();
    //    //        conn = null;
    //    //    }

    //    //    //if (conn2 != null && conn2.State == ConnectionState.Open)
    //    //    //{
    //    //    //    conn2.Close();
    //    //    //    conn2 = null;
    //    //    //}
    //    //}

    //    static String getConnectionString()
    //    {
    //        return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
    //    }   
    //}

    class SerializationProcess
    {
        //private static readonly System.Object lockThis = new System.Object();

        NFExtractor extractor;

        //static SerializationProcess()
        //{
        //    conn = new SqlConnection(getConnectionString());
        //    conn.Open();
        //    conn2 = new SqlConnection(getConnectionString());
        //    conn2.Open();
        //}

        public SerializationProcess()
        {
            extractor = new NFExtractor();
        }

        //~SerializationProcess()
        //{
        //    if (conn != null && conn.State == ConnectionState.Open)
        //    {
        //        conn.Close();
        //        conn = null;
        //    }

        //    if (conn2 != null && conn2.State == ConnectionState.Open)
        //    {
        //        conn2.Close();
        //        conn2 = null;
        //    }
        //}

        public static Int32 rowcount()
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                //conn = buildConnectionString();
                conn = new SqlConnection(getConnectionString());
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = String.Format("SELECT count(*) FROM Egy_T_FingerPrint");

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //return (long)reader[0];
                    return reader.GetInt32(0);
                }
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

            return 0;

        }
    
        //public void run(int from, int to, int count, int threadId)
        //public void run(int from, int to)
        public void run(int from, int to, int count)
        {
            string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
            string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];
            string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];

            //return;

            SqlConnection conn = null;
            SqlConnection conn2 = null;
            SqlCommand cmd = null;
            SqlCommand cmd2 = null;
            SqlDataReader reader = null;

            //List<WsqImage> fingersCollection = null;
            ArrayList fingersCollection = null;
            //ArrayList arr = new ArrayList(10);
            MemoryStream[] ms = new MemoryStream[11];
            //MemoryStream ms;
            byte[] buffer = new byte[0];
            int id = 0;
            int rowNumber = 0;

            StringBuilder sb = new StringBuilder();

            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "li");
            dict.Add(1, "lm");
            dict.Add(2, "lr");
            dict.Add(3, "ll");
            dict.Add(4, "ri");
            dict.Add(5, "rm");
            dict.Add(6, "rr");
            dict.Add(7, "rl");
            dict.Add(8, "lt");
            dict.Add(9, "rt");

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new WsqSerializationBinder.MyBinder<WsqImage>();
            //formatter.Binder = new WsqSerializationBinder.GenericBinder<WsqImage>();

            Stopwatch stw = new Stopwatch();
            //Stopwatch stwd = new Stopwatch();
            //Stopwatch stws = new Stopwatch();
            stw.Start();
            //stwd.Start();
            //stws.Start();

            try
            {
                //conn = buildConnectionString();
                var connStr = getConnectionString();
                conn = new SqlConnection(connStr);
                conn.Open();
                conn2 = new SqlConnection(connStr);
                conn2.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;

                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE AppID = 20095420";

                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE datalength(" + dbFingerColumn + ") IS NOT NULL";
                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM (SELECT ROW_NUMBER() OVER(ORDER BY AppID) AS row, AppID, AppWsq FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL) r WHERE row > {0} and row <= {1}", from, to);
                cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    rowNumber++;
                    Console.WriteLine("{0}", rowNumber + from);

                    if (!reader.IsDBNull(1))
                    {
                        id = (int)reader[dbIdColumn];
                        buffer = (byte[])reader[dbFingerColumn];
                        ms[0] = new MemoryStream(buffer);
                        
                        try
                        {
                            //stwd.Restart();
                            fingersCollection = formatter.Deserialize(ms[0]) as ArrayList;
                            //Console.WriteLine("Deserialize ArrayList, Time elapsed: {0}, AppId: {1}", stwd.Elapsed, id);
                        }
                        //catch (Exception ex) { throw new Exception(ex.ToString()); }
                        catch (Exception) { continue; }
                        finally { ms[0].Close(); }

                        if (cmd2 != null)
                        {
                            cmd2.Dispose();
                            cmd2 = null;
                        }

                        if (sb.Length != 0)
                            sb.Clear();

                        //stws.Restart();
                        String indx = "";
                        NImage nImage = null;
                        NFRecord template = null;

                        for (int i = 0; i < fingersCollection.Count; i++)
                        {
                            if (fingersCollection[i] != null)
                            {
                                try
                                {
                                    ms[i + 1] = new MemoryStream((fingersCollection[i] as WsqImage).Content);
                                    nImage = NImageFormat.Wsq.LoadImage(ms[i + 1]);
                                }
                                catch (Exception ex) {
                                    throw new Exception(string.Format("Error creating image retrieved from database {0}", ex.Message)); 
                                }
                                finally
                                {
                                    if (ms[i + 1] != null)
                                    {
                                        ms[i + 1].Close();
                                        ms[i + 1] = null;
                                    }
                                }

                                float horzResolution = nImage.HorzResolution;
                                float vertResolution = nImage.VertResolution;
                                if (horzResolution < 250) horzResolution = 500;
                                if (vertResolution < 250) vertResolution = 500;

                                NGrayscaleImage grayImage = (NGrayscaleImage)NImage.FromImage(NPixelFormat.Grayscale, 0, horzResolution, vertResolution, nImage);

                                //continue;
                                try
                                {
                                    NfeExtractionStatus extractionStatus;
                                    //lock (lockThis)
                                    //{
                                        //template = Data.NFExtractor.Extract(grayImage, NFPosition.Unknown, NFImpressionType.LiveScanPlain, out extractionStatus);
                                        //NFExtractor extractor = new NFExtractor();
                                        template = extractor.Extract(grayImage, NFPosition.Unknown, NFImpressionType.LiveScanPlain, out extractionStatus);
                                        if (extractionStatus != NfeExtractionStatus.TemplateCreated)
                                        {
                                            throw new Exception(extractionStatus.ToString());
                                        }
                                    //}
                                }
                                catch (Exception)
                                {
                                    if (template != null)
                                    {
                                        template.Dispose();
                                        template = null;
                                    }
                                    //continue;
                                    //throw new Exception(string.Format("Extraction error: {0}", ex.Message)); 
                                }
                                finally
                                {
                                    if (nImage != null)
                                    {
                                        nImage.Dispose();
                                        nImage = null;
                                    }

                                    if (grayImage != null)
                                    {
                                        grayImage.Dispose();
                                        grayImage = null;
                                    }
                                }

                                indx = "@" + dict[i];

                                if (sb.Length == 0)
                                {
                                    cmd2 = new SqlCommand();
                                    cmd2.Connection = conn2;

                                    sb.Append("update {0} with (serializable) SET ");
                                }
                                else
                                    sb.Append(",");

                                //ms[i + 1] = new MemoryStream();
                                //formatter.Serialize(ms[i + 1], template);
                                
                                sb.Append(dict[i] + "=" + indx);
                                cmd2.Parameters.Add(indx, SqlDbType.VarBinary);
                                if (template == null)
                                    cmd2.Parameters[indx].Value = new byte[0];
                                else
                                    cmd2.Parameters[indx].Value = template.Save();
                                //cmd2.Parameters[indx].Value = ms[i + 1].ToArray();

                                if (template != null)
                                {
                                    template.Dispose();
                                    template = null;
                                }

                                //bt[i] = ms[i + 1].ToArray();
                                ////arr.Add(ms[i + 1].ToArray());

                            }
                        }

                        if (sb.Length != 0)
                        {
                            sb.Append(" where {1} = @id");
                            cmd2.CommandText = String.Format(sb.ToString(), dbFingerTable, dbIdColumn);
                            cmd2.Parameters.Add("@id", SqlDbType.Int);
                            cmd2.Parameters["@id"].Value = id;

                            //conn2 = new SqlConnection(getConnectionString());
                            //conn2.Open();
                            //cmd2.Connection = MyConnection.Connection2;
                            cmd2.ExecuteNonQuery();

                            //cmd2.CommandText = String.Format(@"update {0} with (serializable) SET li = @li
                            //    where {1} = @id", dbFingerTable, dbIdColumn);
                        }

/*
                        cmd2.Parameters.Add("@id", SqlDbType.Int);
                        cmd2.Parameters["@id"].Value = id;

                        cmd2.Parameters.Add("@li", SqlDbType.VarBinary);
                        cmd2.Parameters["@li"].Value = arr[0];
                        //cmd.Parameters.Add("@lm", SqlDbType.VarBinary);
                        //cmd.Parameters["@lm"].Value = arr[1];

                        cmd2.ExecuteNonQuery();
*/                        
                        //TimeSpan ts = stws.Elapsed;
                        //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00000000}",
                        //            ts.Hours, ts.Minutes, ts.Seconds,
                        //            ts.Milliseconds);

                        //Console.WriteLine("Serialize WsqImage, Time elapsed: {0}", elapsedTime);

                        //Console.WriteLine("Serialize WsqImage, Time elapsed: {0}", stws.Elapsed);
                        //Console.WriteLine("TaskId: {0}, Serialize WsqImage, Time elapsed: {1}", threadId, stws.ElapsedMilliseconds);
                        //Console.WriteLine("AppId: {0}", id);
                        
                        //arr.Clear();
                        if (fingersCollection != null)
                        {
                            fingersCollection.Clear();
                            fingersCollection = null;
                        }

                        for (int i = 0; i < 11; i++)
                        {
                            if (ms[i] != null)
                            {
                                ms[i].Close();
                                ms[i] = null;
                            }
                        }

                        //if (id % 10 == 0)
                        //{
                        //    //Console.WriteLine(id);
                        //    Console.WriteLine("Number of Records Retrieved: {0}, Time elapsed: {1}", numRecordsRetrieved, stw.Elapsed);
                        //    //Console.WriteLine("Thread Id: {3], Number of Records Retrieved: {0}, Time elapsed: {1}", numRecordsRetrieved, stw.Elapsed, threadId);
                        //    //stw.Restart();
                        //}
                    }
                }

                //Console.WriteLine("From: {0}, To: {1}, Number of Records Retrieved: {2}", from, to, numRecordsRetrieved);
                //Console.WriteLine("Thread Id: {0}, Number of Records Retrieved: {1}, Time elapsed: {2}", threadId, numRecordsRetrieved, stw.Elapsed);

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

                    if (conn2 != null && conn2.State == ConnectionState.Open)
                    {
                        conn2.Close();
                        conn2 = null;
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
