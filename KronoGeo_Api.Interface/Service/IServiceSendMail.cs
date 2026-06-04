using KronoGeo_Api.Models.Infrastructure.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceSendMail
    {
        public EmailResult SendEmail(string to, string subject, string body);
    }
}
