using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp;

namespace KronoGeo_Api.Infrastructure.Service.Email
{
    public class ServiceSmtp(IOptions<CourrielOptions> options) : IServiceSendMessage
    {
        #region private properties
        private readonly CourrielOptions _options = options.Value;
        #endregion

        #region interface implementation
        public async Task<MessageResult> Send(string to, string subject, string body)
        {
            MimeMessage message = new();
            message.From.Add(new MailboxAddress(_options.FromName, _options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_options.Server, _options.Port, true);
                await client.AuthenticateAsync(_options.Username, _options.Password);
                var result = await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return new MessageResult
                {
                    To = to,
                    Status = EmailResultStatus.Success,
                    Message = "Email sent successfully."
                };

            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                return new MessageResult
                {
                    To = to,
                    Status = EmailResultStatus.Failure,
                    Message = "Authentication failed.",
                    Exception = ex
                };
            }
            catch (Exception ex)
            {
                return new MessageResult
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
