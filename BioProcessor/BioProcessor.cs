using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Runtime.Serialization;
using Neurotec.Images;
using Neurotec.Biometrics;
using System.Drawing.Drawing2D;
using Neurotec.Biometrics.Client;

namespace BioProcessor
{
    public class BioProcessor
    {
        private NBiometricClient    _biometricClient;
        private NSubject            _probeSubject;
        enum FingerListEnum { li, lm, lr, ll, ri, rm, rr, rl, lt, rt }

        public BioProcessor()
        {
            try {
                _biometricClient = new NBiometricClient();
            } catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;

                throw new Exception(ex.Message);
            }
        }

        public bool verify(byte[] probeTemplate, byte[] galleryTemplate)
        {
            bool retcode = false;

            var probeSubject = NSubject.FromMemory(probeTemplate);
            if (probeSubject == null)
                throw new Exception("Probe template is null");

            var gallerySubject = NSubject.FromMemory(galleryTemplate);
            if (gallerySubject == null)
                throw new Exception("Gallery template is null");

            var status = _biometricClient.Verify(probeSubject, gallerySubject);
            if (status == NBiometricStatus.Ok)
                retcode = true;

            probeSubject.Dispose();
            gallerySubject.Dispose();
            _biometricClient.Dispose();

            return retcode;
        }

        private NFPosition getFingerPositionByTag(string tag)
        {
            switch (tag)
            {
                case "li":
                    return NFPosition.LeftIndex;
                case "lm":
                    return NFPosition.LeftMiddle;
                case "lr":
                    return NFPosition.LeftRing;
                case "ll":
                    return NFPosition.LeftLittle;
                case "ri":
                    return NFPosition.RightIndex;
                case "rm":
                    return NFPosition.RightMiddle;
                case "rr":
                    return NFPosition.RightRing;
                case "rl":
                    return NFPosition.RightLittle;
                case "lt":
                    return NFPosition.LeftThumb;
                case "rt":
                    return NFPosition.RightThumb;
                default:
                    return NFPosition.Unknown;
            }
        }
        public void enrollProbeTemplate(ArrayList fingerList, byte[] probeTemplate)
        {
            _probeSubject = NSubject.FromMemory(probeTemplate);
            if (_probeSubject == null)
                throw new Exception("Probe template is null");

            //finger.Position = NFPosition.UnknownTwoFingers;
            //_subject.MissingFingers.Clear();
            //foreach (Object o in chlbMissing.CheckedItems)
            //{
            //    _subject.MissingFingers.Add((NFPosition)o);
            //}
            //_biometricClient.FingersDeterminePatternClass = true;
            //_biometricClient.FingersCalculateNfiq = true;
            //NBiometricTask task = _biometricClient.CreateTask(NBiometricOperations.Segment | NBiometricOperations.CreateTemplate | NBiometricOperations.AssessQuality, _probeSubject);

            //NBiometricTask task = _biometricClient.CreateTask(NBiometricOperations.Segment | NBiometricOperations.CreateTemplate, _probeSubject);
            //_biometricClient.PerformTask(task);
            //if (task.Error == null)
            {
                //var sb = new StringBuilder();

                int segmentsCount = _probeSubject.Fingers.Count;
                if (segmentsCount > 0 && _probeSubject.Fingers[0].Status == NBiometricStatus.Ok)
                {
                    //sb.Append(string.Format("Templates extracted: \n"));
                    if (_probeSubject.Fingers[0].Objects[0].Template != null)
                    {
                        if (_probeSubject.Fingers[0].Position == NFPosition.Unknown)
                            _probeSubject.Fingers[0].Position = getFingerPositionByTag(fingerList[0].ToString());
                        //sb.Append(string.Format("{0}: {1}. Size: {2}\n", fingerList[0].ToString(),
                        //                    string.Format("Quality: {0}", _probeSubject.Fingers[1].Objects[0].Quality), _probeSubject.Fingers[1].Objects[0].Template.GetSize()));
                    }
                    //finger = _probeSubject.Fingers[1];
                }
                if (segmentsCount > 1 && _probeSubject.Fingers[1].Status == NBiometricStatus.Ok)
                {
                    if (_probeSubject.Fingers[1].Objects[0].Template != null)
                    {
                        if (_probeSubject.Fingers[1].Position == NFPosition.Unknown)
                            _probeSubject.Fingers[1].Position = getFingerPositionByTag(fingerList[1].ToString());
                        //sb.Append(string.Format("{0}: {1}. Size: {2}\n", fingerList[1].ToString(),
                        //                    string.Format("Quality: {0}", _probeSubject.Fingers[2].Objects[0].Quality), _probeSubject.Fingers[2].Objects[0].Template.GetSize()));
                    }
                }
                if (segmentsCount > 2 && _probeSubject.Fingers[2].Status == NBiometricStatus.Ok)
                {
                    if (_probeSubject.Fingers[2].Objects[0].Template != null)
                    {
                        if (_probeSubject.Fingers[2].Position == NFPosition.Unknown)
                            _probeSubject.Fingers[2].Position = getFingerPositionByTag(fingerList[2].ToString());
                        //sb.Append(string.Format("{0}: {1}. Size: {2}\n", fingerList[2].ToString(),
                        //                    string.Format("Quality: {0}", _probeSubject.Fingers[3].Objects[0].Quality), _probeSubject.Fingers[3].Objects[0].Template.GetSize()));
                    }
                }
                if (segmentsCount > 3 && _probeSubject.Fingers[3].Status == NBiometricStatus.Ok)
                {
                    if (_probeSubject.Fingers[3].Objects[0].Template != null)
                    {
                        if (_probeSubject.Fingers[3].Position == NFPosition.Unknown)
                            _probeSubject.Fingers[3].Position = getFingerPositionByTag(fingerList[3].ToString());
                        //sb.Append(string.Format("{0}: {1}. Size: {2}\n", fingerList[3].ToString(),
                        //                    string.Format("Quality: {0}", _probeSubject.Fingers[4].Objects[0].Quality), _probeSubject.Fingers[4].Objects[0].Template.GetSize()));
                    }
                }
            }
            //else {
            //    if (task.Error != null)
            //        throw new Exception("Probe template error: " + task.Error.Message);
            //    else
            //        throw new Exception("Probe template unknown error");
            //}

            //                _probeSubject = NSubject.FromMemory(probeTemplate[0]);

            //if (_probeSubject == null)
            //    throw new Exception("Probe template is null");
        }

        //public bool match(byte[] galleryTemplate)
        public bool match(ArrayList fingerList, byte[][] galleryTemplate)
        {
            //bool retcode = false;

            bool matched = true;
            if (true)
            {
                _biometricClient.MatchingWithDetails = true;
                _biometricClient.FingersMatchingSpeed = NMatchingSpeed.High;
                _biometricClient.FingersQualityThreshold = 48;
                int threshold = 48;
                var template = new NFTemplate();

                foreach (string finger in fingerList)
                {
                    FingerListEnum f = (FingerListEnum)Enum.Parse(typeof(FingerListEnum), finger);
                    if (galleryTemplate[(int)f] != null && (galleryTemplate[(int)f]).Length != 0)
                    {
                        var record = new NFRecord(galleryTemplate[(int)f]);
                        if (record.Position == NFPosition.Unknown)
                            record.Position = getFingerPositionByTag(f.ToString());

                        template.Records.Add((NFRecord)record.Clone());
                    }
                }

                if (template == null)
                    throw new Exception("Gallery template is null");

                using (var gallerySubject = NSubject.FromMemory(template.Save().ToArray()))
                {
                    if (gallerySubject == null)
                        throw new Exception("Gallery template is null");

                    var status = _biometricClient.Verify(_probeSubject, gallerySubject);
                    if (status == NBiometricStatus.Ok)
                    {
                        foreach (var matchingResult in _probeSubject.MatchingResults)
                        {
                            //int fsc = matchingResult.MatchingDetails.FingersScore;
                            foreach (var finger in matchingResult.MatchingDetails.Fingers)
                            {
                                if (threshold > finger.Score)
                                {
                                    matched = false;
                                    break;
                                }
                            }

                            if (!matched)
                                break;
                        }
                    } else
                        matched = false;
                }
            }
            else
            {
                var template = new NFTemplate();
                foreach (string finger in fingerList)
                {
                    FingerListEnum f = (FingerListEnum)Enum.Parse(typeof(FingerListEnum), finger);
                    if (galleryTemplate[(int)f] != null && (galleryTemplate[(int)f]).Length != 0)
                    {
                        var record = new NFRecord(galleryTemplate[(int)f]);
                        if (record.Position == NFPosition.Unknown)
                            record.Position = getFingerPositionByTag(f.ToString());

                        template.Records.Add((NFRecord)record.Clone());

                        using (var subject = NSubject.FromMemory(template.Save().ToArray()))
                        {
                            var status = _biometricClient.Verify(_probeSubject, subject);
                            if (status != NBiometricStatus.Ok)
                            {
                                matched = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        matched = false;
                        break;
                    }
                }
            }

            //var template = new NFTemplate();

            //foreach (string finger in _fingerList)
            //{
            //    FingerListEnum f = (FingerListEnum)Enum.Parse(typeof(FingerListEnum), finger);
            //    if (galleryTemplate[(int)f] != null && (galleryTemplate[(int)f]).Length != 0)
            //    {
            //        var record = new NFRecord(galleryTemplate[(int)f]);
            //        if (record.Position == NFPosition.Unknown)
            //            record.Position = getFingerPositionByTag(f.ToString());
            //        template.Records.Add((NFRecord)record.Clone());

            //        //matched = matcher.match(buffer[(int)f]);
            //        //if (matched)
            //        //{
            //        //    numOfMatches++;
            //        //}
            //    }



            //}

            //if (template == null)
            //    throw new Exception("Gallery template is null");

            //NSubject gallerySubject = NSubject.FromMemory(template.Save().ToArray());
            //if (gallerySubject == null)
            //    throw new Exception("Gallery template is null");

            //var status = _biometricClient.Verify(_probeSubject, gallerySubject);
            //if (status == NBiometricStatus.Ok)
            //     retcode = true;

            return matched;
        }

        public void CleanBiometrics()
        {
            _probeSubject.Dispose();
            _biometricClient.Dispose();
        }

        public void DeserializeWSQArray(byte[] serializedWSQArray, out ArrayList fingersCollection)
        {
            fingersCollection = null;
            if (serializedWSQArray != null)
            {
                MemoryStream ms = new MemoryStream(serializedWSQArray);

                //Assembly.Load(string assemblyString)
                // Construct a BinaryFormatter and use it to deserialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    //formatter.Binder = new GenericBinder<WsqImage>();
                    formatter.Binder = new WsqSerializationBinder.GenericBinder<WsqImage>();
                    fingersCollection = formatter.Deserialize(ms) as ArrayList;
                }
                catch (SerializationException)
                {
                    //fingersCollection = new ArrayList();
                    //for (int i = 0; i < 10; i++)
                    //    fingersCollection.Add(getEmptyBitmap());
                    //return;
                    fingersCollection = null;
                }
                finally
                {
                    ms.Close();
                }
            }
        }

        public int getImageQuality(byte[] wsqImage)
        {
            NFRecord record = new NFRecord(wsqImage);
            if (record.Quality == 254)
                return 0;
            else
                return record.Quality;
        }

        public void processEnrolledData(byte[][] serializedWSQArray, out ArrayList fingersCollection)
        {
            //if (Data.NFExtractor == null)
            //{
            //    Data.NFExtractor = new NFExtractor();
            //    Data.UpdateNfe();
            //    //Data.UpdateNfeSettings();
            //}

            //if (Data.NMatcher == null)
            //{
            //    Data.NMatcher = new NMatcher();
            //    Data.UpdateNM();
            //    //Data.UpdateNMSettings();
            //}

            DeserializeWSQArray(serializedWSQArray[0], out fingersCollection);
            if (fingersCollection == null)
                return;

            //byte[] buff = null;
            //System.Object theLock = new System.Object();

            //int bestQuality = 0;
            WsqImage wsqImage = null;
            //bool rbChecked = false;
            //, pbChecked = false;
            //int j = 0;
            //throw new Exception((fingersCollection[4] as WsqImage == null).ToString());

            //int bestQuality = 0;
            //int bestQualityImage = 0;
            int pct = 0;
            for (int i = 0; i < fingersCollection.Count; i++)
            {
                if (fingersCollection[i] != null)
                {
                    wsqImage = fingersCollection[i] as WsqImage;
                    try
                    {
                        NImage nImage = NImage.FromMemory(wsqImage.Content, NImageFormat.Wsq);
                        if (serializedWSQArray[i + 1].Length != 0)
                        {
                            NFRecord record = new NFRecord(serializedWSQArray[i + 1]);
                            pct = record.Quality;
                            if (pct == 254)
                                pct = 0;
                        }
                        else
                            pct = 0;

                        //verify(nImage);

                        string label = ""; Brush brush = Brushes.Transparent;

                        if (pct > 0) {
                            label = string.Format("Q: {0:P0}", pct / 100.0);
                            if (pct > 79)
                                brush = Brushes.Green;
                            else if (pct > 39)
                                brush = Brushes.Orange;
                            else
                                brush = Brushes.Red;
                        } else {
                            label = string.Format("q: {0:P0}", 0);
                            brush = Brushes.Red;
                        }

                        //Bitmap bmp = new Bitmap(nImage.ToBitmap(), new Size(65, 95));
                        Bitmap bmp = new Bitmap(nImage.ToBitmap(), new Size(100, 120));
                        //RectangleF rectf = new RectangleF(0.0f, 2.0f, 65.0f, 40.0f);
                        RectangleF rectf = new RectangleF(0.0f, 2.0f, 90.0f, 60.0f);
                        Graphics g = Graphics.FromImage(bmp);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.DrawString(label, new Font("Areal", 13), brush, rectf);
                        g.Flush();

                        using(var ms = new MemoryStream())
                        {
                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                            fingersCollection[i] = ms.ToArray();
                        }
                    }
                    catch (Exception)
                    {
                        //throw new Exception(ex.Message);
                        fingersCollection[i] = getEmptyBitmap();

                        continue;
                    }
                }
                else
                {
                    fingersCollection[i] = getEmptyBitmap();
                }
            }

            //Data.NFExtractor.Dispose();
            //Data.NFExtractor = null;
            //Data.NMatcher.Dispose();
            //Data.NMatcher = null;

            //throw new Exception(j.ToString());

            //rb = this.Controls.Find("radioButton" + (bestQualityRadioButton + 1).ToString(), true)[0] as RadioButton;
            //this.BeginInvoke(new MethodInvoker(delegate() { checkRadioButton(rb.Name); }));


            //System.Threading.Thread.Sleep(5000);
            //stopProgressBar();                           //     !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //buttonScan.Enabled = true;

            //pictureBox2.Image = Properties.Resources.redcross;
        }

        private byte[] getEmptyBitmap()
        {
            using (var ms = new MemoryStream())
            {
                //Properties.Resources.redcross
                //Image newImage = Image.
                //Bitmap bmp = new Bitmap(65, 95);
                Bitmap bmp = new Bitmap(100, 120);

                //Bitmap bmp = new Bitmap(65, 95, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);
                //bmp.Palette = System.Drawing.Imaging.ColorPalette.

                int x, y;

                // Loop through the images pixels to reset color. 
                for (x = 0; x < bmp.Width; x++)
                {
                    for (y = 0; y < bmp.Height; y++)
                    {
                        //Color pixelColor = bmp.GetPixel(x, y);
                        //int luma = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                        //bmp.SetPixel(x, y, Color.FromArgb(luma, luma, luma));

                        //Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                        //#eee
                        Color newColor = System.Drawing.ColorTranslator.FromHtml("#eee");
                        bmp.SetPixel(x, y, newColor);
                    }
                }


                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        //private void verify(NImage nImage)
        //{
        //    var biometricClient = new Neurotec.Biometrics.Client.NBiometricClient();
        //    var subject = new NSubject();
        //    var finger = new NFinger { Image = nImage };
        //    subject.Fingers.Add(finger);
        //    //subject.Fingers[0].Image.Save(subject.Fingers[0].Position + ".png");
        //    biometricClient.CreateTemplate(subject);

        //    if (subject.Fingers[0].Objects[0].Template == null)
        //    {
        //        throw new Exception("Template is null");
        //    }

        //    var subject2 = new NSubject();
        //    var finger2 = new NFinger { Image = nImage };
        //    subject2.Fingers.Add(finger2);
        //    //subject.Fingers[0].Image.Save(subject.Fingers[0].Position + ".png");
        //    biometricClient.CreateTemplate(subject2);

        //    if (subject2.Fingers[0].Objects[0].Template == null)
        //    {
        //        throw new Exception("Template2 is null");
        //    }

        //    var status = biometricClient.Verify(subject, subject2);
        //    if (status != NBiometricStatus.Ok)
        //    {
        //        throw new Exception("Verification failed");
        //    }
        //}

        public Dictionary<string, byte[]> GetTemplatesFromWSQImage(int id, byte[] buffer)
        {
            //string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
            //string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];
            //string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];

            ////return;

            //SqlConnection conn = null;
            //SqlConnection conn2 = null;
            //SqlCommand cmd = null;
            //SqlCommand cmd2 = null;
            //SqlDataReader reader = null;

            //NSubject subject;

            //List<WsqImage> fingersCollection = null;
            //ArrayList fingersCollection = null;
            //ArrayList arr = new ArrayList(10);
            //MemoryStream[] ms = new MemoryStream[11];
            //MemoryStream ms;
            //byte[] buffer = new byte[0];
            //int id = 0;
            //int rowNumber = 0;

            //StringBuilder sb = new StringBuilder();
            Dictionary<string, byte[]> templates = new Dictionary<string, byte[]>();
            templates.Add("wsq", buffer);

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


            //private NBiometricClient _biometricClient;

            var biometricClient = new NBiometricClient { UseDeviceManager = true, BiometricTypes = NBiometricType.Finger };
            _biometricClient.FingersFastExtraction = false;
            biometricClient.FingersTemplateSize = NTemplateSize.Small;
            biometricClient.FingersQualityThreshold = 48;
            biometricClient.Initialize();

            //Stopwatch sw = new Stopwatch();
            //Stopwatch stwd = new Stopwatch();
            //Stopwatch stws = new Stopwatch();
            //stw.Start();
            //stwd.Start();
            //stws.Start();

            //try
            //{
            //conn = buildConnectionString();
            //var connStr = getConnectionString();
            //conn = new SqlConnection(connStr);
            //conn.Open();
            //conn2 = new SqlConnection(connStr);
            //conn2.Open();
            //cmd = new SqlCommand();
            //cmd.Connection = conn;

            //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE AppID = 20095420";

            //cmd.CommandText = "SELECT " + dbIdColumn + "," + dbFingerColumn + " FROM " + dbFingerTable + " WHERE datalength(" + dbFingerColumn + ") IS NOT NULL";
            //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM (SELECT ROW_NUMBER() OVER(ORDER BY AppID) AS row, AppID, AppWsq FROM Egy_T_FingerPrint WHERE datalength(AppWsq) IS NOT NULL) r WHERE row > {0} and row <= {1}", from, to);
            //cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) WHERE datalength(AppWsq) IS NOT NULL ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
            //                cmd.CommandText = String.Format("SELECT AppID, AppWsq FROM Egy_T_FingerPrint WITH (NOLOCK) ORDER BY AppID ASC OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", from, count);
            //cmd.CommandText = "SELECT AppID, AppWsq FROM Egy_T_FingerPrint WHERE AppID = 20095423";

            //reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    rowNumber++;
            //    //                    Console.WriteLine("{0}", rowNumber + from);

            //    if (!reader.IsDBNull(1))
            //    {
            //        id = (int)reader[dbIdColumn];
            //        buffer = (byte[])reader[dbFingerColumn];

            ArrayList fingersCollection = null;

            using (var ms = new MemoryStream(buffer))
            {
                fingersCollection = formatter.Deserialize(ms) as ArrayList;
                //using(MemoryStream memStream = new MemoryStream(100)) 
                //ms[0] = new MemoryStream(buffer);


            }

            //try
            //{
            //    //stwd.Restart();
            //    fingersCollection = formatter.Deserialize(ms) as ArrayList;
            //    //fingersCollection = formatter.Deserialize(ms[0]) as ArrayList;
            //    //Console.WriteLine("Deserialize ArrayList, Time elapsed: {0}, AppId: {1}", stwd.Elapsed, id);
            //}
            ////catch (Exception ex) { throw new Exception(ex.ToString()); }
            ////catch (Exception) { continue; }
            //finally { ms.Close(); }
            //finally { ms[0].Close(); }

            //if (cmd2 != null)
            //{
            //    cmd2.Dispose();
            //    cmd2 = null;
            //}

            //scontinue;

            //if (sb.Length != 0)
            //    sb.Clear();

            //stws.Restart();
            //String indx = "";

            NSubject subject = new NSubject();

            NImage nImage = null;
            NFinger finger = null;
            //NFRecord template = null;

            for (int i = 0; i < fingersCollection.Count; i++)
            {
                if (fingersCollection[i] != null)
                {
                    try
                    {
                        //ms[i + 1] = new MemoryStream((fingersCollection[i] as WsqImage).Content);
                        //nImage = NImageFormat.Wsq.LoadImage(ms[i + 1]);
                        //nImage = NImage.FromStream(ms[i + 1], NImageFormat.Wsq);
                        nImage = NImage.FromMemory((fingersCollection[i] as WsqImage).Content, NImageFormat.Wsq);

                        finger = new NFinger { Image = nImage };
                        //if (subject.Fingers.Count > 0)
                        //    subject.Fingers.RemoveAt(0);

                        //var subject = new NSubject();
                        subject.Fingers.Add(finger);
                        switch (i)
                        {
                            case 0:
                                finger.Position = NFPosition.LeftIndex;
                                break;
                            case 1:
                                finger.Position = NFPosition.LeftMiddle;
                                break;
                            case 2:
                                finger.Position = NFPosition.LeftRing;
                                break;
                            case 3:
                                finger.Position = NFPosition.LeftLittle;
                                break;
                            case 4:
                                finger.Position = NFPosition.RightIndex;
                                break;
                            case 5:
                                finger.Position = NFPosition.RightMiddle;
                                break;
                            case 6:
                                finger.Position = NFPosition.RightRing;
                                break;
                            case 7:
                                finger.Position = NFPosition.RightLittle;
                                break;
                            case 8:
                                finger.Position = NFPosition.LeftThumb;
                                break;
                            case 9:
                                finger.Position = NFPosition.RightThumb;
                                break;
                        }

                    }
                    catch (Exception)
                    {
                        continue;
                        //throw new Exception(string.Format("Error creating image retrieved from database {0}", ex.Message));
                    }
                    finally
                    {
                        if (finger != null)
                        {
                            finger.Dispose();
                            finger = null;
                        }

                        if (nImage != null)
                        {
                            nImage.Dispose();
                            nImage = null;
                        }


                        //if (ms[i + 1] != null)
                        //{
                        //    ms[i + 1].Close();
                        //    ms[i + 1] = null;
                        //}
                    }
                }
            }

            //sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                biometricClient.CreateTemplate(subject);
            }
            catch (Exception ex) {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                throw new Exception(ex.Message);
            }

            //sw.Stop();
            //TimeSpan ts = sw.Elapsed;
            //string elapsedTime = String.Format("{0:00}.{1:00}", ts.Seconds, ts.Milliseconds / 10);
            //Console.WriteLine("RunTime " + elapsedTime);

            bool valid; NFPosition pos = NFPosition.Unknown; //NFRecord record = null;
            for (int i = 0; i < fingersCollection.Count; i++)
            {
                //indx = "@" + dict[i];

                //if (sb.Length == 0)
                //{
                //    cmd2 = new SqlCommand();
                //    cmd2.Connection = conn2;

                //    sb.Append("update {0} with (serializable) SET ");
                //}
                //else
                //    sb.Append(",");

                //sb.Append(dict[i] + "=" + indx);
                //cmd2.Parameters.Add(indx, SqlDbType.VarBinary);

                //valid = false;

                if (fingersCollection[i] != null)
                {
                    switch (i)
                    {
                        case 0:
                            pos = NFPosition.LeftIndex;
                            break;
                        case 1:
                            pos = NFPosition.LeftMiddle;
                            break;
                        case 2:
                            pos = NFPosition.LeftRing;
                            break;
                        case 3:
                            pos = NFPosition.LeftLittle;
                            break;
                        case 4:
                            pos = NFPosition.RightIndex;
                            break;
                        case 5:
                            pos = NFPosition.RightMiddle;
                            break;
                        case 6:
                            pos = NFPosition.RightRing;
                            break;
                        case 7:
                            pos = NFPosition.RightLittle;
                            break;
                        case 8:
                            pos = NFPosition.LeftThumb;
                            break;
                        case 9:
                            pos = NFPosition.RightThumb;
                            break;
                    }

                    //if (sb.Length == 0)
                    //{
                    //    cmd2 = new SqlCommand();
                    //    cmd2.Connection = conn2;

                    //    sb.Append("update {0} with (serializable) SET ");
                    //}
                    //else
                    //    sb.Append(",");

                    //ms[i + 1] = new MemoryStream();
                    //formatter.Serialize(ms[i + 1], template);

                    //sb.Append(dict[i] + "=" + indx);
                    //cmd2.Parameters.Add(indx, SqlDbType.VarBinary);

                    valid = false;
                    int k = 0;
                    for (k = 0; k < subject.Fingers.Count; k++)
                    {
                        if (subject.Fingers[k].Position == pos)
                        {
                            if (subject.Fingers[k].Objects.First().Status == NBiometricStatus.Ok)
                            {
                                if (subject.Fingers[k].Objects.First().Quality != 254)
                                {
                                    valid = true;
                                    //Console.WriteLine(" ----- Size: {0}", subject.Fingers[k].Objects.First().Template.GetSize());

                                }
                            }

                            break;
                        }
                    }

                    if (!valid)
                    {
                        templates.Add(dict[i], new byte[0]);
                    }
                    else
                    {
                        templates.Add(dict[i], subject.Fingers[k].Objects.First().Template.Save().ToArray());
                        //record = subject.Fingers[k].Objects.First().Template;
                        //cmd2.Parameters[indx].Value = record.Save().ToArray();
                    }
                }
                else
                {
                    templates.Add(dict[i], new byte[0]);
                }
            }

            //try
            //{
            //    var db = new DAO.Database();
            //    db.SaveWSQTemplate(id, templates);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}

            if (subject != null)
                subject.Dispose();

            if (biometricClient != null)
                biometricClient.Dispose();

            if (fingersCollection != null)
            {
                fingersCollection.Clear();
                fingersCollection = null;
            }

            return templates;
        }
    }
}
