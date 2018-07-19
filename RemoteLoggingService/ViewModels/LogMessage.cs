using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RemoteLoggingService.ViewModels
{
    public class LogMessage
    {
        public LogMessage(string clientGuid, DateTime time, string message, string status, string metadata)
        {
            ClientGuid = clientGuid;
            Time = time;
            Message = message;
            Metadata = metadata;
            switch(status)
            {
                case "Message": Status = LogStatus.Message; break;
                case "Warning": Status = LogStatus.Warning; break;
                case "Error": Status = LogStatus.Error; break;
                case "Debug": Status = LogStatus.Debug; break;
                case "Fatal": Status = LogStatus.Fatal; break;
                default: throw new Exception("Invalid log status");
            }
        }
        public string ClientGuid { get; set; }
        public DateTime Time { get; }
        public string Message { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LogStatus Status { get; }
        public string Metadata { get; set; }        
        
        public enum LogStatus
        {
            Message,
            Warning,
            Error,
            Debug,
            Fatal,
            Any
        }
    }
}
