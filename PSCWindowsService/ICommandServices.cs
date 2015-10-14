using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace PSCWindowsService
{
    [ServiceContract]
    public interface ICommandServices
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "CheckConnection/")]
        string CheckConnection();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TakeImage/{id}?checkBoxesStates={checkBoxesStates}&callback={callback}")]
        Stream TakeImage(string id, string checkBoxesStates, string callback);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "LeftHand/{id}?checkBoxesStates={checkBoxesStates}&callback={callback}")]
        Stream LeftHand(string id, string checkBoxesStates, string callback);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RightHand/{id}?checkBoxesStates={checkBoxesStates}&callback={callback}")]
        Stream RightHand(string id, string checkBoxesStates, string callback);
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Thumbs/{id}?checkBoxesStates={checkBoxesStates}&callback={callback}")]
        Stream Thumbs(string id, string checkBoxesStates, string callback);
    }
}
