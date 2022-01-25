using System.Net;
using System.Net.Mail;

namespace NetCore6WebApi.Providers
{ 
    public class DefaultEmailer : IEmailer
        {
            private TestingConfig Config { get; set; }

            public DefaultEmailer(TestingConfig config)
            {
                Config = config;
            }

            public void Send(string from, string to, string subject, string body)
            {
                MailAddress toEmail = new MailAddress(to);
                MailAddress fromEmail = new MailAddress(from);

                MailMessage message = new MailMessage(from, to);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient(Config.EmailCredentials.SmtpClientHost, Config.EmailCredentials.SmtpClientPort)
                {
                    Credentials = new NetworkCredential(Config.EmailCredentials.CredentialUserName, Config.EmailCredentials.CredentialPassword),
                    EnableSsl = false
                };

                try
                {
                    client.Send(message);
                }
                catch (SmtpException)
                {
                }
            }
        }
    
}
