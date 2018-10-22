using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;


namespace RemoteLoggingService.Notifications
{
    /// <summary>
    /// Class that used for email sending
    /// </summary> 
    public class EmailSender
    {
        private string smtpServer;
        private int smtpPort;
        private string login;
        private string password;
        private bool isSslEnabled;

        public SmtpClient Client { get; protected set; }
        public MailMessage Message { get; protected set; }
        public string Log { get; protected set; }

        /// <summary>
        /// Configuration of SMTP client
        /// </summary>      
        public EmailSender(string smtpServer, int smtpPort, string login, string password, bool isSslEnabled)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.login = login;
            this.password = password;
            this.isSslEnabled = isSslEnabled;
            Log = string.Empty;
        }

        /// <summary>
        /// Configuration of email using incoming parameters
        /// </summary>
        /// <param name="recepients">Array of recipients addresses</param>
        /// <param name="carbonCopies">Array of CC addresses</param>
        /// <param name="from">Sender's Email Address</param>
        /// <param name="fromDisplayName">Sender's Name</param>
        /// <param name="subject">Mail's subject</param>
        /// <param name="body">Mail's body</param>
        /// <param name="isBodyHtml">Set true if you use html for message's body markup</param>
        /// <param name="attachments">Array of attached files' paths</param>
        public virtual void Configure(string[] recepients, string[] carbonCopies, string from, string fromDisplayName, string subject, string body, bool isBodyHtml, string[] attachments)
        {
            try
            {
                Log += "Configuration was started \n";
                CreateClient();
                CreateMessage(recepients, from, fromDisplayName, subject, body, isBodyHtml);
                AddAttachments(attachments);
                AddCarbonCopies(carbonCopies);
            }
            catch (Exception exc)
            {
                throw new Exception($"Configuration failed: ${exc.Message}.");
            }
        }

        /// <summary>
        /// Sends email using configuration. You should configure settings before.
        /// </summary>
        public virtual void Send()
        {
            try
            {
                Log += "Sending was started \n";
                if (Client == null)
                    throw new Exception("SMTP client was not configured.");
                if (Message == null)
                    throw new Exception("Email message was not configured.");

                Client.Send(Message);
                Log += "Message was sent \n";
            }
            catch (Exception exc)
            {
                throw new Exception($"Sending failed: ${exc.Message}.");
            }
        }

        //Configuration of SMTP client
        protected virtual void CreateClient()
        {
            try
            {
                Log += "Configuration of SMTP client \n";
                Client = new SmtpClient
                {
                    Host = smtpServer,
                    Port = smtpPort,
                    EnableSsl = isSslEnabled,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                Log += "Server details added \n";
                Client.UseDefaultCredentials = false;
                Log += "Default credentials disabled \n";
                Client.Credentials = new NetworkCredential(login, password);
                Log += "New credentials added \n";
                Log += "Client was configured \n";
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }

        //Configuration of Mail Message
        protected virtual void CreateMessage(string[] recepients, string from, string name, string subject, string body, bool isBodyHtml)
        {
            try
            {
                Log += "Configuration of message \n";
                if (recepients == null)
                    throw new ArgumentNullException("recipients", "Cannot be null");

                if (String.IsNullOrEmpty(from))
                    throw new ArgumentNullException("from", "Cannot be null or empty string");

                // Mail Configuration
                Message = new MailMessage
                {
                    From = new MailAddress(from, name),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };
                Log += "Subject, Body and Address From added \n";

                // Add recipients
                Log += $"Message has {recepients.Length} recipients: ";
                foreach (var to in recepients)
                {
                    if (!String.IsNullOrEmpty(to))
                    {
                        Message.To.Add(new MailAddress(to));
                        Log += $"'{to}' ";
                    }
                }
                Log += "\nMessage was configured \n";
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }

        //Add attachments from array to Message
        protected virtual void AddAttachments(string[] attachments)
        {
            if (attachments != null
                && attachments.Length > 0)
            {
                Log += $"Message has {attachments.Length} attachments \n";
                foreach (var attachment in attachments)
                    if (!String.IsNullOrEmpty(attachment))
                        Message.Attachments.Add(new Attachment(attachment));
            }
        }

        //Add attachments from Streams to Message
        protected virtual void AddAttachments(Dictionary<Stream, string> attachments)
        {
            if (attachments != null)
            {
                Log += $"Message has {attachments.Count} attachments \n";
                foreach (var attachment in attachments)
                    if (attachment.Value != null)
                        Message.Attachments.Add(new Attachment(attachment.Key, attachment.Value));
            }
        }

        //Add emails for sending of copy of message
        protected virtual void AddCarbonCopies(string[] carbonCopies)
        {
            if (carbonCopies != null)
            {
                Log += $"Message has {carbonCopies.Length} carbon copies \n";
                foreach (var cc in carbonCopies)
                    if (!String.IsNullOrEmpty(cc))
                        Message.CC.Add(new MailAddress(cc));
            }
        }
    }
}
