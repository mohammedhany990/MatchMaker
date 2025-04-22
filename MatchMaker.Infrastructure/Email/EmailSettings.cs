using MailKit.Net.Smtp;
using MailKit.Security;
using MatchMaker.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MatchMaker.Infrastructure.Email
{
    public class EmailSettings : IEmailSettings
    {
        private MailSettings _options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            _options = options.Value;
        }

        public void SendEmail(Core.Entities.Email email)
        {

            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = email.Subject
            };
            mail.From.Add(new MailboxAddress(_options.KnownAs, _options.Email));

            mail.To.Add(MailboxAddress.Parse(email.To));

            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_options.Email, _options.Password);
            smtp.Send(mail);
            smtp.Disconnect(true);
        }
    }
}
