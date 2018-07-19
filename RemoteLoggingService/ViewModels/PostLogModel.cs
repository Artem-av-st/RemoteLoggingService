using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RemoteLoggingService.ViewModels
{
    public class LogModel
    {
        [BindRequired]
        public Guid Guid { get; set; }
    }
}
