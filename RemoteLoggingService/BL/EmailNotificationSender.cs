using RemoteLoggingService.Services.ViewModels;
using Newtonsoft.Json;
using RemoteLoggingService.BL.Interfaces;
using RemoteLoggingService.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using RemoteLoggingService.Notifications;

namespace RemoteLoggingService.Services
{
    public class EmailNotificationSender : INotificationSender
    {
        private EmailSender _emailSender;
        private string _connectionString;
        private string _senderName;

        public EmailNotificationSender()
        {
            // Get application settings
            var settingsText = File.ReadAllText(@"appsettings.json");
            var appSettings = JsonConvert.DeserializeObject<AppSettings>(settingsText);

            
            _connectionString = appSettings.ConnectionStrings.DefaultConnection;
            
            // Configure email sender
            var smtpSettings = appSettings.SmtpSettings;
            _senderName = smtpSettings.Login;
            _emailSender = new EmailSender
            (
                smtpServer: smtpSettings.SmtpServer,
                smtpPort: smtpSettings.SmtpPort,
                login: smtpSettings.Login,
                password: smtpSettings.Password,
                isSslEnabled: smtpSettings.UseSsl
             );
        }
        
        public async Task<bool> Notify(Log message, IRepository db)
        {
            try
            {
                string developerEmail = (await db.GetClientById(message.ClientId)).Developer.Email;
                if (String.IsNullOrEmpty(developerEmail))
                {
                    return false;
                }
                string clientName = (await db.GetClientById(message.ClientId)).Name;
                if (String.IsNullOrEmpty(clientName))
                {
                    return false;
                }
                
                _emailSender.Configure
                (
                    recepients: new[] { developerEmail }, 
                    carbonCopies: null,
                    from: _senderName, 
                    fromDisplayName: "Remote Logging Service", 
                    subject: "You have some problems, Bro", 
                    body: GetMessageBody(clientName, message), 
                    isBodyHtml:false, 
                    attachments: null
                );
                _emailSender.Send();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetMessageBody(string clientName, Log message)
        {
            return "Hi Bro!\n\n" +
                    $"Unfortunately, the client {clientName} experienced a critical error:\n" +
                    $"{message.Message} at {message.Time.ToString("dd-MM-yy hh:mm:ss")}.\n\n" +
                    $" You have to fix it.\n\n" +
                    $"Take care!";
        }        
    }
}
