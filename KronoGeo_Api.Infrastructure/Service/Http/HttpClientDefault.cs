using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpClientDefault : IDisposable
    {
        #region private properties
        private readonly IOptions<TokenTunnel>? _tokenTunnel;
        #endregion

        #region protected properties
        protected readonly HttpClient HttpClient;
        #endregion

        #region public constructor
        public HttpClientDefault(HttpClient httpClient, IOptions<TokenTunnel>? tokenTunnel)
        {
            HttpClient = httpClient;
      
            // - initialise les protections du tunnel pour le debug
#if DEBUG
            if (tokenTunnel is not null)
            {
                _tokenTunnel = tokenTunnel;
                ChargingTokenTunnel();
            }
#endif

        }
        #endregion

        #region public interface Idisposable
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region private method
        /// <summary>
        /// quand le tunnel de développement est en mode sécurisé
        /// il faut enregistrer le token pour accéder au tunnel
        /// ne le faire qu'en développement
        /// </summary>
        private void ChargingTokenTunnel()
        {
            // -- charge le token du tunnel en Debug pour la sécurité
            // - ajout du token pour l'utilisation du tonnel sécurisé
            var token = _tokenTunnel?.Value.Token ?? string.Empty;
            if (string.IsNullOrEmpty(token)) return;
            // -- ajout au niveau de l'entete http du tunnel authorization
            HttpClient.DefaultRequestHeaders.Add("X-Tunnel-Authorization", $"{token}");
            //_httpClient.DefaultRequestHeaders.Add("X-Tunnel-Authorization", "tunnel eyJhbGciOiJFUzI1NiIsImtpZCI6IjcyRjZDNUU3OEE2M0UzOEUxM0UyOTE1MjM0NjMyMDFGMDFDMzQ2MTUiLCJ0eXAiOiJKV1QifQ.eyJjbHVzdGVySWQiOiJldXciLCJ0dW5uZWxJZCI6InBlYWNlZnVsLWNoYWlyLWI2Y3ZnYzIiLCJzY3AiOiJjb25uZWN0IiwiZXhwIjoxNzg3MDUxMzQ3LCJpc3MiOiJodHRwczovL3R1bm5lbHMuYXBpLnZpc3VhbHN0dWRpby5jb20vIiwibmJmIjoxNzg2OTY0MDQ3fQ.UBF-WTYmgM1qIUJ9lGL7ElALXBZOqXK4ZXKJ3y4qZ-niUxOoAcvApYd3tFyhpZgAodbtNHEz-CqVmliesx__bw");
        }

        #endregion
    }
}
