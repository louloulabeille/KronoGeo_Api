using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Email
{
    public class UrlOptions
    {
        public required string UrlEmailAuthentification { get; set; }
        public required string UrlRecupEmail { get; set; }
        public required string UrlUpdateEmail { get; set; }
    }
}
