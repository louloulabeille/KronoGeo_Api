using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Options
{
    /// <summary>
    /// class pour IOptions - url Api pour le web Assembly
    /// </summary>
    public class UrlApiBlazorClient : UrlApiMetier
    {
        public string Login { get; set; } = string.Empty;
        public string Me { get; set; } = string.Empty;
        public string Logout { get; set; } = string.Empty;
    
    }
}
