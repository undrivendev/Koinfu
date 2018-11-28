using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Mtd.Koinfu.BLL.Services.Logging;

namespace Mtd.Koinfu.BLL
{
    public class MailSender
    {

        private readonly Policy callWithRetry = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(new[]
          {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
            });

        private readonly ILogger logger;
        private readonly IEmailValidator emailValidator;
        private readonly string serverUri;
        private readonly int port;
        private readonly string username;
        private readonly string password;

        public MailSender(ILogger logger, IEmailValidator emailValidator, string serverUri, int port, string username, string password)
        {
            this.logger = logger;
            this.emailValidator = emailValidator ?? throw new ArgumentNullException(nameof(emailValidator));
            this.serverUri = serverUri;
            this.port = port;
            this.username = username;
            this.password = password;
        }


        public async Task SendMailsAsync(string stringMessage, CancellationToken token, params string[] recipientEmails)
        {
            var validEmailAddresses = new List<string>();
            foreach (var address in recipientEmails)
            {
                if (this.emailValidator.IsEmailValid(address))
                {
                    validEmailAddresses.Add(address);
                }
                else
                {
                    logger.Log(new LogEntry(LoggingEventType.Warning, $"Invalid email address for alert notification recipient: {address}"));
                }
            }

            if (validEmailAddresses.Count == 0)
            {
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Koinfu Alert System", this.username));
            message.To.AddRange(validEmailAddresses.Select(a => new MailboxAddress(a)));
            message.Subject = "Something requires your attention";

            message.Body = GetMailBody(stringMessage);


            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true; //TODO: do not skip validation

                client.Connect(this.serverUri, this.port, false, token);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(this.username, this.password, token);

                try
                {
                    await callWithRetry.ExecuteAsync(async (t) => await client.SendAsync(message, t), token);
                }
                catch (Exception e)
                {
                    logger.Log(e);
                }
                client.Disconnect(true, token);

            }
        }

        private MimeEntity GetMailBody(string message)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            return bodyBuilder.ToMessageBody();
        }
    }
}
