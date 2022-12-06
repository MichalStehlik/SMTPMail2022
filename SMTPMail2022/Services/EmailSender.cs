using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using static System.Net.Mime.MediaTypeNames;

namespace SMTPMail2022.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSenderOptions _options;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSenderOptions> oa)
        {
            _logger = logger;
            _options = oa.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.From, _options.FromName));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            bodyBuilder.TextBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_options.Server, _options.Port, _options.UseSSL);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_options.Username, _options.Password);

                client.Send(message);
                client.Disconnect(true);
                return Task.FromResult(0);
            }
        }
    }

    public class EmailSenderOptions
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public bool UseSSL { get; set; }
    }
}
