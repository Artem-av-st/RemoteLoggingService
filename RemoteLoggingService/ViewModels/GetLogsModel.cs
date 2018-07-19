using System;
using System.ComponentModel.DataAnnotations;

namespace RemoteLoggingService.ViewModels
{
    public class GetLogsModel
    {
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Get logs from date")]
        [DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [Display(Name = "Get logs to date")]    
        [DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Log type")]
        public string LogType { get; set; }

        [Display(Name = "Find text")]
        public string FindText { get; set; }

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
