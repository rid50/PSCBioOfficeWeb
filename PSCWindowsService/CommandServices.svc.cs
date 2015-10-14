using System;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using System.Collections;
using WindowsService.CacheService;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PSCWindowsService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CommandServices : ICommandServices
    {
        class AjaxReturn { public string result { get; set; } public string message { get; set; }}

        public void Scan() {}

        public string CheckConnection()
        {
            //for all cors requests  
            WebOperationContext.Current.OutgoingResponse.Headers
                .Add("Access-Control-Allow-Origin","*");  
            //identify preflight request and add extra headers  
            if (WebOperationContext.Current.IncomingRequest.Method == "OPTIONS") {  
                WebOperationContext.Current.OutgoingResponse.Headers  
                    .Add("Access-Control-Allow-Methods", "POST, OPTIONS, GET");  
                WebOperationContext.Current.OutgoingResponse.Headers  
                    .Add("Access-Control-Allow-Headers",  
                        "Content-Type, Accept, Authorization, x-requested-with");  
                return null;  
            }
            return "ok";
        }

        public Stream LeftHand(string id, string checkBoxesStates, string callback)
        {
            return Scan(id, 0, checkBoxesStates, callback);
        }

        public Stream RightHand(string id, string checkBoxesStates, string callback)
        {
            return Scan(id, 1, checkBoxesStates, callback);
        }

        public Stream Thumbs(string id, string checkBoxesStates, string callback)
        {
            return Scan(id, 2, checkBoxesStates, callback);
        }

        public Stream TakeImage(string id, string checkBoxesStates, string callback)
        {
            return Scan(id, 3, checkBoxesStates, callback);
        }

        private Stream Scan(string id, int hand, string checkBoxesStates, string callback)
        {
            var result = "";
            var message = "";

            try
            {
                Int32.Parse(id);
            }
            catch (Exception)
            {
                //toReturn = "Please enter a valid ID";
                result = "error";
                message = "Please enter a valid ID";
            }

            if (result.Length == 0)
            {
                string[] checkBoxes = null;

                if (hand == 3)
                    checkBoxes = new[] { "1", "1", "1", "1" };
                else
                    checkBoxes = checkBoxesStates.Split(',');

                Scanner scanner = null;
                MemoryStream ms = null;
                try
                {
                    var cache = new MemoryCacheServiceClient();
                    byte[] buffer = cache.GetRawFingerCollection(id);

                    ms = new MemoryStream(buffer);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new WsqSerializationBinder.GenericBinder<WsqImage>();
                    ArrayList fingersCollection = formatter.Deserialize(ms) as ArrayList;

                    //var bioProcessor = new BioProcessor.BioProcessor();
                    //bioProcessor.DeserializeWSQArray(buff, out fingersCollection);

                    scanner = new Scanner();
                    scanner.FingersCollection = fingersCollection;

                    result = scanner.scan(hand, id, checkBoxes);

                    cache.SetDirty();
                    //var dataContract = new FingerPrintDataContract();
                    //dataContract.id = id;
                    //dataContract.fingersCollection = scanner.FingersCollection;
                    //cache.Put(dataContract);

                }
                catch (Exception ex)
                {
                    if (scanner != null)
                        scanner.Disconnect();
                    //toReturn = ex.Message;
                    result = "error";
                    message = ex.Message;
                }
                finally
                {
                    if (ms != null)
                        ms.Close();
                }
            }

            message = System.Web.HttpUtility.JavaScriptStringEncode(message);

            //toReturn = callback + "({\"Status\":\"" + toReturn + "\"});";
            //toReturn = callback + "({\"error\":\"" + error + "\"});";
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript";
            //return new MemoryStream(Encoding.UTF8.GetBytes(toReturn));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(new { result = result, message = message });
            str = callback + "(" + str + ")";
            return new MemoryStream(Encoding.UTF8.GetBytes(str));


            //IList<Object> UserInfo = new List<Object>();

           /* var jsonEmptyData = new
            {
                Total = 0,
                Page = 1,
                Records = 0,
                Rows = new { item_id = 0, part_no = "", part_no_description = "", sum_quantity = 0, unit_price = 0d, total_price = 0d }
            };
            return Json(jsonEmptyData, JsonRequestBehavior.AllowGet);*/
            //throw new Exception("scan left");
            /*
            string toReturn = id + " --- "; // +checkBoxesStates.Count.ToString();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            StringBuilder sb = new StringBuilder();
            sb.Append(callback + "(");
            sb.Append(serializer.Serialize(toReturn));
            sb.Append(");");

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript";
            return sb.ToString();
            */
            //WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript";
            //var toReturn = callback + "({\"Status\":\"" + checkBoxesStates.Length + "\"});";
            //return new MemoryStream(Encoding.UTF8.GetBytes(toReturn));

            //return callback + "({\"Status\":\"OK\"});";

            //Context.Response.Clear();
            //Context.Response.ContentType = "application/json";
            //Context.Response.Write(sb.ToString());
            //Context.Response.End();
            


            //return id + " --- "; // +checkBoxesStates.Count.ToString();
        }
    }
}
