using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using static Microsoft.Maui.Authentication.AppleSignInAuthenticator;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public partial class HttpClientManagerKronogeo(HttpClient httpClient
        , IOptions<TokenTunnel> tokenTunnel, IOptions<UrlApiManager> options)
        : HttpClientDefault(httpClient, tokenTunnel), IServiceHttpManagerKronoGeo
    {

        #region private readonly properties
        private readonly IOptions<UrlApiManager> _options = options;
        #endregion

        #region public method interface IServiceHttpManagerKronoGeo
        /// <summary>
        /// method d'envoi des données pour vérification du login et mot de passe
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public async virtual Task<ResponseApiAuthenticate> AuthenticateAsync(RegisterDTO register)
        {
            HttpContent content = new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await HttpClient.PostAsync(_options.Value.Login, content);
            //response.EnsureSuccessStatusCode();

            // -- quand 5 tentatives au niveau de HttpClient retour erreur
            if (response.StatusCode == HttpStatusCode.BadGateway
                || response.StatusCode == HttpStatusCode.InternalServerError
                || response.StatusCode == HttpStatusCode.RequestTimeout
                || response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                throw new HttpRequestException("Connexion impossible au serveur.");
            }

            var retour = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResponseApiAuthenticate>(retour, JsonOptions.GetJsonOptions())
                ?? new ResponseApiAuthenticate
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = retour
                };

            return result;
        }

        /// <summary>
        /// Retourne le profil de la personne
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiManager> GetProfileAsync()
        {

            using var response = await HttpClient.GetAsync(_options.Value.GetProfile);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var retour = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResponseApiManager>(retour, JsonOptions.GetJsonOptions())
                    ?? new ResponseApiManager
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = "Erreur lors de la désérialisation de la réponse de l'API."
                    };

                return result;
            }

            return new ResponseApiManager
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = $" Status Code retour {response.StatusCode}"
            };

        }

        /// <summary>
        /// modifie le profil de l'utilisateur
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiManager> UpdateProfilAsync( ProfilDTO profil )
        {
            var content = new StringContent(JsonSerializer.Serialize(profil), Encoding.UTF8, "application/json");
            using var response = await HttpClient.PostAsync(_options.Value.UpdateProfile, content);
            response.EnsureSuccessStatusCode();

            if ( response.IsSuccessStatusCode )
            {
                var retour = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ResponseApiManager>(retour, JsonOptions.GetJsonOptions())
                    ?? new ResponseApiManager
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = "Erreur lors de la désérialisation de la réponse de l'API."
                    };

                return result;
            }

            return new ResponseApiManager
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = $" Status Code retour {response.StatusCode}"
            };

        }

        
        #endregion
    }
}
