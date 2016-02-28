//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization.Formatters.Binary;
//using Neurotec.Biometrics.Client;
//using Neurotec.Biometrics;
//using System.IO;
using System.Collections;
//using Neurotec.Images;
using BiometricService.ConfigurationService;
using DAO;
//using WsqSerializationBinder;

namespace BiometricService
{
    public class WSQImageService : IWSQImageService
    {
        public void DeserializeWSQArray(byte[] serializedWSQArray, out ArrayList fingersCollection)
        {
            var bioProcessor = new BioProcessor.BioProcessor();
            bioProcessor.DeserializeWSQArray(serializedWSQArray, out fingersCollection);
        }

        //public ArrayList processEnrolledData(byte[][] serializedWSQArray)
        public void processEnrolledData(byte[][] serializedWSQArray, out ArrayList fingersCollection)
        {
            //ArrayList fingersCollection;
            var bioProcessor = new BioProcessor.BioProcessor();
            try {
                bioProcessor.processEnrolledData(serializedWSQArray, out fingersCollection);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception(ex.Message);
            }
            //return fingersCollection;
        }

        public void SaveWSQImage(int id, byte[] buffer)
        {
            var bioProcessor = new BioProcessor.BioProcessor();
            Dictionary<string, byte[]> templates = bioProcessor.GetTemplatesFromWSQImage(id, buffer);

            var client = new ConfigurationServiceClient();

            Dictionary<string, string> settings = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in client.AppSettings())
            {
                settings.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in client.ConnectionStrings())
            {
                settings.Add(kvp.Key, kvp.Value);
            }

            var db = new Database(settings);
            db.SaveWSQTemplate(id, templates);
        }


//        public void SaveWSQImage(int id, byte[] buffer)
//        {
//            //string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
//            //string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];
//            //string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];

//            ////return;

//            //SqlConnection conn = null;
//            //SqlConnection conn2 = null;
//            //SqlCommand cmd = null;
//            //SqlCommand cmd2 = null;
//            //SqlDataReader reader = null;

//            //NSubject subject;

//            //List<WsqImage> fingersCollection = null;
//            //ArrayList fingersCollection = null;
//            //ArrayList arr = new ArrayList(10);
//            //MemoryStream[] ms = new MemoryStream[11];
//            //MemoryStream ms;
//            //byte[] buffer = new byte[0];
//            //int id = 0;
//            //int rowNumber = 0;

//            //StringBuilder sb = new StringBuilder();
//            Dictionary<string, byte[]> templates = new Dictionary<string, byte[]>();
//            templates.Add("wsq", buffer);

//            Dictionary<int, string> dict = new Dictionary<int, string>();
//            dict.Add(0, "li");
//            dict.Add(1, "lm");
//            dict.Add(2, "lr");
//            dict.Add(3, "ll");
//            dict.Add(4, "ri");
//            dict.Add(5, "rm");
//            dict.Add(6, "rr");
//            dict.Add(7, "rl");
//            dict.Add(8, "lt");
//            dict.Add(9, "rt");

//            BinaryFormatter formatter = new BinaryFormatter();
//            formatter.Binder = new WsqSerializationBinder.MyBinder<WsqImage>();

//            //formatter.Binder = new WsqSerializationBinder.GenericBinder<WsqImage>();


//        //private NBiometricClient _biometricClient;

//             var biometricClient = new NBiometricClient { UseDeviceManager = true, BiometricTypes = NBiometricType.Finger };
//            //_biometricClient.FingersFastExtraction = true;
//            biometricClient.FingersTemplateSize = NTemplateSize.Small;
//            biometricClient.FingersQualityThreshold = 40;
//            biometricClient.Initialize();

//            //Stopwatch sw = new Stopwatch();
//            //Stopwatch stwd = new Stopwatch();
//            //Stopwatch stws = new Stopwatch();
//            //stw.Start();
//            //stwd.Start();
//            //stws.Start();

//            //try
//            //{
//                //conn = buildConnectionString();
//                //var connStr = getConnectionString();
//                //conn = new SqlConnection(connStr);
//                //conn.Open();
//                //conn2 = new SqlConnection(connStr);
//                //conn2.Open();
//                //cmd = new SqlCommand();
//                //cmd.Connection = conn;

//                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE AppID = 20095420";

//                //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE datalength(" + dbFingerColumn + ") IS NOT NULL";
//                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM (SELECT ROW_NUMBER() OVER(ORDER BY AppID) AS row, AppID, AppWsq FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL) r WHERE row > {0} and row <= {1}", from, to);
//                //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
////                cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
//                //cmd.CommandText = "SELECT AppID, AppWsq FROM Egy_T_FingerPrint WHERE AppID = 20095423";

//                //reader = cmd.ExecuteReader();
//                //while (reader.Read())
//                //{
//                //    rowNumber++;
//                //    //                    Console.WriteLine("{0}", rowNumber + from);

//                //    if (!reader.IsDBNull(1))
//                //    {
//                //        id = (int)reader[dbIdColumn];
//                //        buffer = (byte[])reader[dbFingerColumn];

//            ArrayList fingersCollection = null;

//            using (var ms = new MemoryStream(buffer)) {
//                fingersCollection = formatter.Deserialize(ms) as ArrayList;
//            //using(MemoryStream memStream = new MemoryStream(100)) 
//            //ms[0] = new MemoryStream(buffer);
                        
                        
//            }

//                        //try
//                        //{
//                        //    //stwd.Restart();
//                        //    fingersCollection = formatter.Deserialize(ms) as ArrayList;
//                        //    //fingersCollection = formatter.Deserialize(ms[0]) as ArrayList;
//                        //    //Console.WriteLine("Deserialize ArrayList, Time elapsed: {0}, AppId: {1}", stwd.Elapsed, id);
//                        //}
//                        ////catch (Exception ex) { throw new Exception(ex.ToString()); }
//                        ////catch (Exception) { continue; }
//                        //finally { ms.Close(); }
//                        //finally { ms[0].Close(); }

//                        //if (cmd2 != null)
//                        //{
//                        //    cmd2.Dispose();
//                        //    cmd2 = null;
//                        //}

//                        //scontinue;

//                        //if (sb.Length != 0)
//                        //    sb.Clear();

//                        //stws.Restart();
//                        //String indx = "";

//            NSubject subject = new NSubject();

//            NImage nImage = null;
//            NFinger finger = null;
//            //NFRecord template = null;

//            for (int i = 0; i < fingersCollection.Count; i++)
//            {
//                if (fingersCollection[i] != null)
//                {
//                    try
//                    {
//                        //ms[i + 1] = new MemoryStream((fingersCollection[i] as WsqImage).Content);
//                        //nImage = NImageFormat.Wsq.LoadImage(ms[i + 1]);
//                        //nImage = NImage.FromStream(ms[i + 1], NImageFormat.Wsq);
//                        nImage = NImage.FromMemory((fingersCollection[i] as WsqImage).Content, NImageFormat.Wsq);

//                        finger = new NFinger { Image = nImage };
//                        //if (subject.Fingers.Count > 0)
//                        //    subject.Fingers.RemoveAt(0);

//                        //var subject = new NSubject();
//                        subject.Fingers.Add(finger);
//                        switch (i)
//                        {
//                            case 0:
//                                finger.Position = NFPosition.LeftIndex;
//                                break;
//                            case 1:
//                                finger.Position = NFPosition.LeftMiddle;
//                                break;
//                            case 2:
//                                finger.Position = NFPosition.LeftRing;
//                                break;
//                            case 3:
//                                finger.Position = NFPosition.LeftLittle;
//                                break;
//                            case 4:
//                                finger.Position = NFPosition.RightIndex;
//                                break;
//                            case 5:
//                                finger.Position = NFPosition.RightMiddle;
//                                break;
//                            case 6:
//                                finger.Position = NFPosition.RightRing;
//                                break;
//                            case 7:
//                                finger.Position = NFPosition.RightLittle;
//                                break;
//                            case 8:
//                                finger.Position = NFPosition.LeftThumb;
//                                break;
//                            case 9:
//                                finger.Position = NFPosition.RightThumb;
//                                break;
//                        }

//                    }
//                    catch (Exception)
//                    {
//                        continue;
//                        //throw new Exception(string.Format("Error creating image retrieved from database {0}", ex.Message));
//                    }
//                    finally
//                    {
//                        if (finger != null) {
//                            finger.Dispose();
//                            finger = null;
//                        }

//                        if (nImage != null)
//                        {
//                            nImage.Dispose();
//                            nImage = null;
//                        }

                        
//                        //if (ms[i + 1] != null)
//                        //{
//                        //    ms[i + 1].Close();
//                        //    ms[i + 1] = null;
//                        //}
//                    }
//                }
//            }

//            //sw = System.Diagnostics.Stopwatch.StartNew();
//            biometricClient.CreateTemplate(subject);
//            //sw.Stop();
//            //TimeSpan ts = sw.Elapsed;
//            //string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
//            //Console.WriteLine("RunTime " + elapsedTime);

//            bool valid; NFPosition pos = NFPosition.Unknown; //NFRecord record = null;
//            for (int i = 0; i < fingersCollection.Count; i++)
//            {
//                //indx = "@" + dict[i];

//                //if (sb.Length == 0)
//                //{
//                //    cmd2 = new SqlCommand();
//                //    cmd2.Connection = conn2;

//                //    sb.Append("update {0} with (serializable) SET ");
//                //}
//                //else
//                //    sb.Append(",");

//                //sb.Append(dict[i] + "=" + indx);
//                //cmd2.Parameters.Add(indx, SqlDbType.VarBinary);

//                //valid = false;

//                if (fingersCollection[i] != null)
//                {
//                    switch (i)
//                    {
//                        case 0:
//                            pos = NFPosition.LeftIndex;
//                            break;
//                        case 1:
//                            pos = NFPosition.LeftMiddle;
//                            break;
//                        case 2:
//                            pos = NFPosition.LeftRing;
//                            break;
//                        case 3:
//                            pos = NFPosition.LeftLittle;
//                            break;
//                        case 4:
//                            pos = NFPosition.RightIndex;
//                            break;
//                        case 5:
//                            pos = NFPosition.RightMiddle;
//                            break;
//                        case 6:
//                            pos = NFPosition.RightRing;
//                            break;
//                        case 7:
//                            pos = NFPosition.RightLittle;
//                            break;
//                        case 8:
//                            pos = NFPosition.LeftThumb;
//                            break;
//                        case 9:
//                            pos = NFPosition.RightThumb;
//                            break;
//                    }

//                    //if (sb.Length == 0)
//                    //{
//                    //    cmd2 = new SqlCommand();
//                    //    cmd2.Connection = conn2;

//                    //    sb.Append("update {0} with (serializable) SET ");
//                    //}
//                    //else
//                    //    sb.Append(",");

//                    //ms[i + 1] = new MemoryStream();
//                    //formatter.Serialize(ms[i + 1], template);

//                    //sb.Append(dict[i] + "=" + indx);
//                    //cmd2.Parameters.Add(indx, SqlDbType.VarBinary);

//                    valid = false;
//                    int k = 0;
//                    for (k = 0; k < subject.Fingers.Count; k++)
//                    {
//                        if (subject.Fingers[k].Position == pos)
//                        {
//                            if (subject.Fingers[k].Objects.First().Status == NBiometricStatus.Ok)
//                            {
//                                if (subject.Fingers[k].Objects.First().Quality != 254)
//                                {
//                                    valid = true;
//                                    //Console.WriteLine(" ----- Size: {0}", subject.Fingers[k].Objects.First().Template.GetSize());

//                                }
//                            }

//                            break;
//                        }
//                    }

//                    if (!valid)
//                    {
//                        templates.Add(dict[i], null);
//                    }
//                    else
//                    {
//                        templates.Add(dict[i], subject.Fingers[k].Objects.First().Template.Save().ToArray());
//                        //record = subject.Fingers[k].Objects.First().Template;
//                        //cmd2.Parameters[indx].Value = record.Save().ToArray();
//                    }
//                }
//                else
//                {
//                    templates.Add(dict[i], null);
//                }
//            }

//            try
//            {
//                var client = new ConfigurationServiceClient();

//                Dictionary<string, string> settings = new Dictionary<string, string>();
//                foreach (KeyValuePair<string, string> kvp in client.AppSettings())
//                {
//                    settings.Add(kvp.Key, kvp.Value);
//                }

//                foreach (var kvp in client.ConnectionStrings())
//                {
//                    settings.Add(kvp.Key, kvp.Value);
//                }

//                var db = new Database(settings);
//                db.SaveWSQTemplate(id, templates);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }

//            if (subject != null)
//                subject.Dispose();

//            if (biometricClient != null)
//                biometricClient.Dispose();

//            if (fingersCollection != null)
//            {
//                fingersCollection.Clear();
//                fingersCollection = null;
//            }
//        }
    }
}
