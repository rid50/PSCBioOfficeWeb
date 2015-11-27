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
                    catch (Exception ex)
                    {
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

        //private NFRecord template;

        //private int GetImageQuality(NGrayscaleImage image, out string label, out Brush brush)
        //{
        //    label = "";
        //    brush = Brushes.Transparent;

        //    NGrayscaleImage resultImage = (NGrayscaleImage)image.Clone();
        //    //throw new Exception("clone: " + resultImage.Width.ToString());

        //    try
        //    {
        //        NfeExtractionStatus extractionStatus;
        //        //var extr = Data.NFExtractor;
        //        //throw new Exception("extr == null: " + (extr == null).ToString());
        //        //template = extr.Extract(resultImage, NFPosition.Unknown, NFImpressionType.LiveScanPlain, out extractionStatus);
        //        template = Data.NFExtractor.Extract(resultImage, NFPosition.Unknown, NFImpressionType.LiveScanPlain, out extractionStatus);
        //        if (extractionStatus != NfeExtractionStatus.TemplateCreated)
        //        {
        //            //throw new Exception("No TemplateCreated");
        //            if (label != null)
        //            {
        //                label = string.Format("q: {0:P0}", 0);
        //                brush = Brushes.Red;
        //            }
        //            return 0;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //throw new Exception("Ex: " + ex.Message);

        //        if (label != null)
        //        {
        //            label = string.Format("q: {0:P0}", 0);
        //            brush = Brushes.Red;
        //        }
        //        return 0;
        //    }

        //    this.template = (NFRecord)template.Clone();
        //    int i = 0;
        //    if (template != null)
        //    {
        //        i = Helpers.QualityToPercent(template.Quality);
        //        if (label != null)
        //        {
        //            label = string.Format("q: {0:P0}", i / 100.0);
        //            if (i > 80)
        //                brush = Brushes.Green;
        //            else if (i > 50)
        //                brush = Brushes.Orange;
        //            else
        //                brush = Brushes.Red;
        //        }
        //    }
        //    else
        //    {
        //        if (label != null)
        //        {
        //            label = string.Format("q: {0:P0}", 0);
        //            brush = Brushes.Red;
        //        }
        //    }

        //    return i;
        //}
    }
}
