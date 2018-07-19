using RemoteLoggingService.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteLoggingService.BL.Inerfaces
{
    interface ILogProvider
    {
        string GetUserRole(string userGuid);
        List<LogMessage> GetLogMessages(string clientGuid, LogMessage.LogStatus status);       
        void PostLogMessages(List<LogMessage> logMessage);
        bool CheckIfDeveloperAssignedToClient(string clientGuid, string developerGuid);
    }
}
