using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Runtime.Serialization;
using Neurotec.Images;
using Neurotec.Biometrics;
using System.Drawing.Drawing2D;

namespace BioProcessor
{
    public class BioProcessor
    {
        public void DeserializeWSQArray(byte[] serializedWSQArray, out ArrayList fingersCollection)
        {
            if (serializedWSQArray == null)
            {
                fingersCollection = new ArrayList();
                for (int i = 0; i < 10; i++)
                    fingersCollection.Add(getEmptyBitmap());
            }

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
                fingersCollection = new ArrayList();
                for (int i = 0; i < 10; i++)
                    fingersCollection.Add(getEmptyBitmap());
                //return;
            }
            finally
            {
                ms.Close();
            }
        }

        public void processEnrolledData(byte[] serializedWSQArray, out ArrayList fingersCollection)
        {
            if (Data.NFExtractor == null)
            {
                Data.NFExtractor = new NFExtractor();
                Data.UpdateNfe();
                //Data.UpdateNfeSettings();
            }

            if (Data.NMatcher == null)
            {
                Data.NMatcher = new NMatcher();
                Data.UpdateNM();
                //Data.UpdateNMSettings();
            }

            DeserializeWSQArray(serializedWSQArray, out fingersCollection);
/*
            //fingersCollection = null;
            //ResourceManager rm = new ResourceManager("PSCBioVerification.Form1", this.GetType().Assembly);
            if (serializedWSQArray == null)
            {
                //clearFingerBoxes();
                //string text = rm.GetString("msgThePersonHasNotYetBeenEnrolled"); // "The person has not yet been enrolled"

                //LogLine(text, true);
                //ShowErrorMessage(text);
                //stopProgressBar();
                fingersCollection = new ArrayList();
                for (int i = 0; i < 10; i++)
                    fingersCollection.Add(getEmptyBitmap());

                //throw new Exception("The person has not yet been enrolled");
                //return;
            }

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
                //LogLine(ex.Message, true);
                //ShowErrorMessage(ex.Message);
                //stopProgressBar();
                //throw new Exception("SerializationException: " + ex.Message);
                fingersCollection = new ArrayList();
                for (int i = 0; i < 10; i++)
                    fingersCollection.Add(getEmptyBitmap());
                //return;
            }
            finally
            {
                ms.Close();
            }
*/
            //byte[] buff = null;
            //System.Object theLock = new System.Object();

            //int bestQuality = 0;
            WsqImage wsqImage = null;
            //bool rbChecked = false;
            //, pbChecked = false;
            //int j = 0;
            //throw new Exception((fingersCollection[4] as WsqImage == null).ToString());

            for (int i = 0; i < fingersCollection.Count; i++)
            {
                wsqImage = fingersCollection[i] as WsqImage;
                //if (wsqImage == null)
                //{
                    //rb.Enabled = false;
                    //lab.Enabled = false;
                //}
                //else
                //{
                    //rb.Enabled = true;
                    //lab.Enabled = true;
                //}

                //pb = this.Controls.Find("fpPictureBox" + (i + 1).ToString(), true)[0] as PictureBox;

                //throw new Exception("Arr: " + (fingersCollection[i] != null).ToString());
                //if (fingersCollection[i] != null)
                if (wsqImage != null)
                {
                    //throw new Exception("Cont: " + wsqImage.Content.ToString());
                    //WsqImage wsq = _fingersCollection[i] as WsqImage;

                    MemoryStream ms = null;
                    //NImage nImage;
                    try
                    {
                        ms = new MemoryStream(wsqImage.Content);
                        //throw new Exception("Cont2: " + wsqImage.Content.ToString());

                        //Bitmap bm = new Bitmap(ms);

                        //image = NImage.FromFile(fileName, NImageFormat.Wsq);

                        NImage nImage = NImageFormat.Wsq.LoadImage(ms);
                        //throw new Exception("NImageFormat.Wsq.LoadImage(ms)");
                        
                        float horzResolution = nImage.HorzResolution;
                        //throw new Exception("hor: " + nImage.HorzResolution.ToString());
                        float vertResolution = nImage.VertResolution;
                        if (horzResolution < 250) horzResolution = 500;
                        if (vertResolution < 250) vertResolution = 500;

                        NGrayscaleImage grayImage = (NGrayscaleImage)NImage.FromImage(NPixelFormat.Grayscale, 0, horzResolution, vertResolution, nImage);
                        //throw new Exception(grayImage.Size.ToString());
                        //int q = GetImageQuality(grayImage, this.Controls.Find("lbFinger" + (i + 1).ToString(), true)[0] as Label);
                        string label = ""; Brush brush = Brushes.Transparent;
                        int q = GetImageQuality(grayImage, out label, out brush);

                        //label = "kuku";
                        //brush = Brushes.Red;
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

                        //throw new Exception(bmp.Width.ToString());

                        //throw new Exception("Quality: " + q.ToString());


                        //if (bestQuality < q)
                        //{
                        //    bestQuality = q;
                            //bestQualityRadioButton = i;
                        //}

                        //ms.Close();

                        using(var ms2 = new MemoryStream())
                        {
                            //nImage.ToBitmap().Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                            bmp.Save(ms2, System.Drawing.Imaging.ImageFormat.Bmp);
                            fingersCollection[i] = ms2.ToArray();
                        }

                        //fingersCollection[i] = nImage.ToBitmap();
                        //(fingersCollection[dict[wsqQuery]] as System.Drawing.Bitmap).ToByteArray(ImageFormat.Bmp);
                        //throw new Exception("nImage.ToBitmap: " + nImage.Size.ToString());

                        //pb.Image = nImage.ToBitmap();
                        ////image = NImage.FromBitmap(bm);
                        //pb.SizeMode = PictureBoxSizeMode.Zoom;

                    }
                    catch (Exception)
                    {
                        //j++;
                        //throw new Exception(e.Message);

                        //MessageBox.Show(string.Format("Error creating image retrieved from database {0}", ex.Message),
                        //  Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        //string text = string.Format("Error creating image retrieved from database {0}", ex.Message);
                        //ShowErrorMessage(text);

                        continue;
                    }
                    finally
                    {
                        if (ms != null)
                            ms.Close();
                    }
                }
                else
                {
                    fingersCollection[i] = getEmptyBitmap();
/*
                    using (ms = new MemoryStream())
                    {
                        //Properties.Resources.redcross
                        //Image newImage = Image.
                        Bitmap bmp = new Bitmap(65, 95);

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
                        fingersCollection[i] = ms.ToArray();
                    }
*/
                }
            }

            Data.NFExtractor.Dispose();
            Data.NFExtractor = null;
            Data.NMatcher.Dispose();
            Data.NMatcher = null;

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

        private NFRecord template;

        private int GetImageQuality(NGrayscaleImage image, out string label, out Brush brush)
        {
            label = "";
            brush = Brushes.Transparent;

            NGrayscaleImage resultImage = (NGrayscaleImage)image.Clone();
            //throw new Exception("clone: " + resultImage.Width.ToString());

            try
            {
                NfeExtractionStatus extractionStatus;
                //var extr = Data.NFExtractor;
                //throw new Exception("extr == null: " + (extr == null).ToString());
                //template = extr.Extract(resultImage, NFPosition.Unknown, NFImpressionType.LiveScanPlain, out extractionStatus);
                template = Data.NFExtractor.Extract(resultImage, NFPosition.Unknown, NFImpressionType.LiveScanPlain, out extractionStatus);
                if (extractionStatus != NfeExtractionStatus.TemplateCreated)
                {
                    //throw new Exception("No TemplateCreated");
                    if (label != null)
                    {
                        label = string.Format("q: {0:P0}", 0);
                        brush = Brushes.Red;
                    }
                    return 0;
                }
            }
            catch (Exception)
            {
                //throw new Exception("Ex: " + ex.Message);

                if (label != null)
                {
                    label = string.Format("q: {0:P0}", 0);
                    brush = Brushes.Red;
                }
                return 0;
            }

            this.template = (NFRecord)template.Clone();
            int i = 0;
            if (template != null)
            {
                i = Helpers.QualityToPercent(template.Quality);
                if (label != null)
                {
                    label = string.Format("q: {0:P0}", i / 100.0);
                    if (i > 80)
                        brush = Brushes.Green;
                    else if (i > 50)
                        brush = Brushes.Orange;
                    else
                        brush = Brushes.Red;
                }
            }
            else
            {
                if (label != null)
                {
                    label = string.Format("q: {0:P0}", 0);
                    brush = Brushes.Red;
                }
            }

            return i;
        }
    }
/*
    public class GenericBinder<T> : System.Runtime.Serialization.SerializationBinder
    {
        /// <summary>
        /// Resolve type
        /// </summary>
        /// <param name="assemblyName">eg. App_Code.y4xkvcpq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null</param>
        /// <param name="typeName">eg. String</param>
        /// <returns>Type for the deserializer to use</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            // We're going to ignore the assembly name, and assume it's in the same assembly 
            // that <T> is defined (it's either T or a field/return type within T anyway)

            string[] typeInfo = typeName.Split('.');
            bool isSystem = (typeInfo[0].ToString() == "System");
            string className = typeInfo[typeInfo.Length - 1];

            // noop is the default, returns what was passed in
            Type toReturn = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));

            if (!isSystem && (toReturn == null))
            {   // don't bother if system, or if the GetType worked already (must be OK, surely?)
                System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(typeof(T));
                string assembly = a.FullName.Split(',')[0];   //FullName example: "App_Code.y4xkvcpq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
                if (a == null)
                {
                    throw new ArgumentException("Assembly for type '" + typeof(T).Name.ToString() + "' could not be loaded.");
                }
                else
                {
                    Type newtype = a.GetType(assembly + "." + className);
                    if (newtype == null)
                    {
                        throw new ArgumentException("Type '" + typeName + "' could not be loaded from assembly '" + assembly + "'.");
                    }
                    else
                    {
                        toReturn = newtype;
                    }
                }
            }
            return toReturn;
        }
    }

    [Serializable]
    public class WsqImage
    {
        public int XSize { get; set; }
        public int YSize { get; set; }
        public int XRes { get; set; }
        public int YRes { get; set; }
        public int PixelFormat { get; set; }
        public byte[] Content { get; set; }
    }
*/
}
