using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Options
{
    public record UrlProxyOpenStreetMap
    {
        public string BasicAdress { get; set; } = string.Empty;
    }
}
