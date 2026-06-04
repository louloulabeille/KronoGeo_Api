using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email
{
    public class ServiceSmtp(CourrielOptions options) : IServiceSendMail
    {
        #region private properties
        private readonly CourrielOptions _options = options;
        #endregion

        #region interface implementation
        public EmailResult SendEmail(string to, string subject, string body)
        {
            MimeMessage message = new();
            message.From.Add(new MailboxAddress(_options.FromName, _options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

            var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                client.Connect(_options.Server, _options.Port, true);
                client.Authenticate(_options.Username, _options.Password);
                client.Send(message);
                client.Disconnect(true);
                return new EmailResult
                {
                    To = to,
                    Status = EmailResultStatus.Success,
                    Message = "Email sent successfully."
                };

            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                return new EmailResult
                {
                    To = to,
                    Status = EmailResultStatus.Failure,
                    Message = "Authentication failed.",
                    Exception = ex
                };
            }
            catch (Exception ex)
            {
                return new EmailResult
                {
                    To = to,
                    Status = EmailResultStatus.Failure,
                    Message = "An error occurred while sending the email.",
                    Exception = ex
                };
            }

        }
        #endregion
    }
}
