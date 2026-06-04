using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Email
{
    public class CourrielOptions
    {
        public required string From { get; set; }
        public required string FromName { get; set; }
        public required string Server { get; set; }
        public required int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
