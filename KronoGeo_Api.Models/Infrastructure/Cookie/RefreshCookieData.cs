using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Cookie
{
    /// <summary>
    /// record pour tranférer les données pour la création du cookie
    /// </summary>
    public record RefreshCookieData
    {
        public required string Token { get; set; }
        public required string Id { get; set; }
        public required string Login { get; set; }
    }
}
