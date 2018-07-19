using RemoteLoggingService.Models;

namespace RemoteLoggingService.BL.Interfaces
{
    interface INotificationSender
    {
        bool Notify(Log message, AppDbContext dbContext);
    }
}
