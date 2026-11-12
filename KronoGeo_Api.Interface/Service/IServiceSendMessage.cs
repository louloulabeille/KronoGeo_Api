using KronoGeo_Api.Models.Infrastructure.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceSendMessage
    {
        public Task<MessageResult> Send(string to, string subject, string body);
    }
}
