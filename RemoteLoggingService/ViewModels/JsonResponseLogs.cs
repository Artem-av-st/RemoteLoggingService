using RemoteLoggingService.Models;
using System.Collections.Generic;
using System.Net;

namespace RemoteLoggingService.ViewModels
{
    public class JsonResponseLogs:JsonResponseBase
    {
        public JsonResponseLogs(HttpStatusCode statusCode, string message, IEnumerable<Log> logMessages):base(statusCode, message)
        {
            LogMessages = logMessages;
        }

        public JsonResponseLogs(HttpStatusCode statusCode, IEnumerable<Log> logMessages) : base(statusCode)
        {
            LogMessages = logMessages;
        }

        public IEnumerable<Log> LogMessages { get;}


    }
}
