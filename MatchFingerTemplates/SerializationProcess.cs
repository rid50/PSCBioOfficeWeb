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
using Neurotec.Biometrics.Client;

namespace MatchFingerTemplates
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
        static public bool terminateProcess = false;

        private NBiometricClient _biometricClient;

        //private static readonly System.Object lockThis = new System.Object();

        //NFExtractor extractor;

        //static SerializationProcess()
        //{
        //    conn = new SqlConnection(getConnectionString());
        //    conn.Open();
        //    conn2 = new SqlConnection(getConnectionString());
        //    conn2.Open();
        //}

        NSubject subject = null;

        public SerializationProcess(string ProbeTemplate)
        {
            subject = NSubject.FromFile(ProbeTemplate);
            //subject = null;
            //int i = 0;
        }

        //string probeTemplate;
        //public string ProbeTemplate
        //{
        //    get { return probeTemplate; }
        //    set {
        //        probeTemplate = value;

        //    }
        //}

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
                cmd.CommandText = "SELECT count(*) FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL";
                reader = cmd.ExecuteReader();
                reader.Read();
                return reader.GetInt32(0);
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
    
        public int run(int from, int to, int count)
        {
            string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
            string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];
            string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            int AppId = 0;
            int rowNumber = 0;

            _biometricClient = new NBiometricClient { UseDeviceManager = true, BiometricTypes = NBiometricType.Finger };
            _biometricClient.Initialize();

            Stopwatch sw = new Stopwatch();

            NSubject subject2 = null;
            int score = 0;
            int numberOfMatches = 0;
            NBiometricStatus status;
            int retcode = 0;

            byte[][] buffer = new byte[2][];

            try
            {
                var connStr = getConnectionString();
                conn = new SqlConnection(connStr);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = String.Format("SELECT AppID, ri, rr FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (terminateProcess)
                        break;

                    //rowNumber++;
                    //Console.WriteLine("{0}", rowNumber + from);

                    numberOfMatches = 0;

                    if (!reader.IsDBNull(0))
                    {
                        AppId = reader.GetInt32(0);
                        //Console.WriteLine("{0}", AppId);

                        if (!reader.IsDBNull(1) && ((byte[])reader["ri"]).Length > 1)
                        {
                            buffer[0] = (byte[])reader["ri"];

                            subject2 = NSubject.FromMemory(buffer[0]);

                            //_biometricClient.MatchingWithDetails = true;

                            //sw.Start();

                            status = _biometricClient.Verify(subject, subject2);

                            //sw.Stop();
                            //Console.WriteLine(" ------------------------- Time elapsed: {0}", sw.Elapsed);

                            if (status == NBiometricStatus.Ok)
                            {
                                score = subject.MatchingResults[0].Score;
                                numberOfMatches++;
                            }

                            subject2.Clear();
                        }

                        if (!reader.IsDBNull(2) && ((byte[])reader["rr"]).Length > 1)
                        {
                            buffer[1] = (byte[])reader["rr"];

                            subject2 = NSubject.FromMemory(buffer[1]);

                            _biometricClient.MatchingWithDetails = true;
                            status = _biometricClient.Verify(subject, subject2);

                            if (status == NBiometricStatus.Ok)
                            {
                                score = subject.MatchingResults[0].Score;
                                numberOfMatches++;
                            }

                            subject2.Clear();
                        }
                    }

                    if (numberOfMatches == 2)
                    {
                        //Console.WriteLine(" ----- AppID: {0}", AppId);
                        retcode = AppId;
                        break;
                    }
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

            return retcode;
        }

        static private String getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }
    }
}
