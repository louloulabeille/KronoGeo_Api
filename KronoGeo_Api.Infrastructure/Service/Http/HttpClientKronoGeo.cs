using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpClientKronoGeo ( IOptions<UrlApi> options, HttpClient httpClient ) : IServiceHttpKronoGeo, IDisposable
    {
        #region private properties
        private readonly HttpClient _httpClient = httpClient;
        private readonly IOptions<UrlApi> _options = options;
        #endregion

        #region public method interface IServiceHttpKronoGeo
        /// <summary>
        /// method d'envoi des données pour vérification du login et mot de passe
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> AuthenticateAsync( RegisterDTO register )
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_options.Value.Login, content);
            
            return response;
        }


        #endregion

        #region public method interface IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
