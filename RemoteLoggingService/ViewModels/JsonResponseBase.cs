using Newtonsoft.Json;
using System.Net;

namespace RemoteLoggingService.ViewModels
{
    public class JsonResponseBase
    {
        public JsonResponseBase(HttpStatusCode code)
        {
            StatusCode = code;
        }
        public JsonResponseBase(HttpStatusCode code, string message)
        {
            StatusCode = code;
            Message = message;
        }
        public HttpStatusCode StatusCode { get; }
        
        public string Message { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this).ToString();
        }

    }
}
