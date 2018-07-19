namespace RemoteLoggingService.Services.ViewModels
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public SmtpSettings SmtpSettings { get; set; }

    }
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
    public class SmtpSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
    }    
}
