using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Cookie
{
    /// <summary>
    /// record pour tranférer les données pour la création du cookie
    /// </summary>
    public record RefreshCookieData(string token, string id, string login)
    {
        public string Token { get; set; } = token;
        public string Id { get; set; } = id;
        public string Login { get; set; } = login;

    }
}
