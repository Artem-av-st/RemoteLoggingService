using RemoteLoggingService.Models;
using System.Threading.Tasks;

namespace RemoteLoggingService.BL.Interfaces
{
    interface INotificationSender
    {
        Task<bool> Notify(Log message, IRepository dbContext);
    }
}
