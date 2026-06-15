using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
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
        public async Task<ResponseApiAuthenticate> AuthenticateAsync( RegisterDTO register )
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await _httpClient.PostAsync(_options.Value.Login, content);

            var retour = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseApiAuthenticate>(retour , JsonOptions.GetJsonOptions()) 
                ?? new ResponseApiAuthenticate
                   {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = retour
                   }; 

            return result;
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
