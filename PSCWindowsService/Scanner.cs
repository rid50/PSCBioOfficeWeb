using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using WindowsService.DatabaseService;

namespace PSCWindowsService
{
    //public enum IMAGE_TYPE
    //{
    //    picture = 0,
    //    wsq = 1
    //}

    class Scanner
    {
        public int ErrorCode { get; set; }

        private ARHScanner _sc = null;
        private ArrayList _fingersCollection = null;

        public Scanner()
        {
            _sc = new ARHScanner();
            //_fingersCollection = fingersCollection;
            //_fingersCollection = new ArrayList(10);
            //for (int i = 0; i < _fingersCollection.Capacity; i++)
            //    _fingersCollection.Add(null);
        }

        public ArrayList FingersCollection
        {
            get { return _fingersCollection; }
            set { _fingersCollection = value; }
        }

        public string scan(int hand, string id, string[] checks)
        {
            var retCode = new StringBuilder();

            int count = 3;
            int offset = 0;

            if ((ErrorCode = _sc.fpsConnect()) != 0)
            {
                var err = GetError(_sc);
                Disconnect();
                throw new Exception(err);
            }
            else
            {
                if (hand == 0)      // only left hand
                {
                    count = 1;
                    offset = 0;
                }
                else if (hand == 1) // only right hand
                {
                    count = 2;
                    offset = 4;
                }
                else if (hand == 2) // only thumbs
                {
                    count = 3;
                    offset = 7;
                }
                else if (hand == 3) // all 10 fingers
                {
                    hand = 0;
                    count = 3;
                    offset = 0;
                }

                for (int i = hand; i < count; i++, hand++)
                {
                    if ((ErrorCode = scanFingers(i, checks)) != 0)
                    {
                        var err = GetError(_sc);
                        Disconnect();
                        throw new Exception(err);
                    }

                    for (int k = 0; k < 4; k++)
                    {
                        if (hand == 2 && k == 0)
                            continue;

                        if (_sc.ArrayOfWSQ[k] != null && ((WsqImage)_sc.ArrayOfWSQ[k]).Content != null)
                        {
                            _fingersCollection[k + offset] = _sc.ArrayOfWSQ[k];
                            if (hand == 0)
                                retCode.Insert(0, '0');
                            else
                                retCode.Append('0');
                        }
                        else
                        {
                            //if (checks.Length > k && checks[k] == "1")
                            if (hand == 0)
                                retCode.Insert(0, '1');
                            else
                                retCode.Append('1');
                            //else
                                //retCode.Append('0');
                        }
                    }

                    offset += 4;
                    if (offset == 8)
                        offset = 7;
                }

                if (ErrorCode == 0)
                {
                    byte[] buff = null;
                    MemoryStream ms = new MemoryStream();
                    // Construct a BinaryFormatter and use it to serialize the data to the stream.
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(ms, _fingersCollection as ArrayList);
                        buff = ms.ToArray();

                        var client = new DbDataServiceClient();
                        client.SendImage(IMAGE_TYPE.wsq, Convert.ToInt32(id), ref buff);

                        //saveWsqInDatabase(id, buff);
                    }
                    catch (SerializationException ex)
                    {
                        retCode.Clear();
                        retCode.Append(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        retCode.Clear();
                        retCode.Append(ex.Message);
                    }
                    finally
                    {
                        ms.Close();
                    }
                }
                else
                {
                    var err = GetError(_sc);
                    Disconnect();
                    throw new Exception(err);
                }
            }

            _sc.fpsDisconnect();
            _sc = null;

            return retCode.ToString();
        }

        private int scanFingers(int hand, string[] checks)  // 0 - left hand;  1 - right hand;  2 - thumbs
        {
            int errorCode = 0;

            //IList list = new ArrayList();

            // The finger list has the format 0hhh 0000 iiii mmmm rrrr llll tttt ssss
            //	h - scan object: 001 left hand, 010 right hand, 011 same fingers of both hands

            StringBuilder sb = new StringBuilder(10);
            //int maskOffset = 1;
            int count = 4;

            switch (hand)
            {
                case 0:
                    sb.Append("10");        //0x10333300    left hand
                    break;
                case 1:
                    sb.Append("20");        //0x20333300    right hand
                    //maskOffset = 5;
                    break;
                case 2:
                    sb.Append("300000");    //0x30000033    thumbs
                    //maskOffset = 9;
                    count = 2;
                    break;
            }

            int maskOffset = 5;
            for (int j = 0; j < count; j++)
            {
                //if (j + maskOffset == 9)
                //    continue;

                //if (((CheckBox)this.Controls.Find("checkBox" + (j + maskOffset).ToString(), true)[0]).Checked)
                //if (checks[j + maskOffset] == "1")

                if (hand == 0)
                    maskOffset -= 2;
                else
                    maskOffset = 0;

                if (checks[j + maskOffset] == "1")  // for a left hand, test the array in reverse order 
                    sb.Append("3");
                else
                    sb.Append("0");
            }

            switch (hand)
            {
                case 0:
                case 1:
                    sb.Append("00");
                    break;
            }

            //int handAndFingerMask = Int32.Parse(sb.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
            //if ((errorCode = _sc.fpsGetFingersImages(handAndFingerMask, true)) == 0)
            //{

            //    //saveWsqInDatabase();
            //    showFingers(maskOffset);
            //}

            int handAndFingerMask = Int32.Parse(sb.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
            errorCode = _sc.fpsGetFingersImages(handAndFingerMask, true);

            return errorCode;
        }

        private string GetError(ARHScanner sc)
        {
            //if (ErrorCode != 1303 && ErrorCode != -1)
            //    return String.Format("{0} --- Error code: {1}", sc.ErrorMessage, ErrorCode);
            //else
                return String.Format("{0}", sc.ErrorMessage);
        }

        public void Disconnect()
        {
            if (_sc != null)
            {
                _sc.fpsDisconnect();
                _sc = null;
            }
        }
    }
}
