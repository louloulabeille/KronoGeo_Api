using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Parameter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static Microsoft.Maui.Authentication.AppleSignInAuthenticator;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    /// <summary>
    /// HttpClient pour le client blazor pour l'authentification 
    /// et la récupération des userinfos de la session
    /// </summary>
    /// <param name="options"></param>
    /// <param name="httpClient"></param>
    public class HttpBlazorClient(IOptions<UrlApiBlazorClient> options , HttpClient httpClient
        , ILogger<HttpBlazorClient> logger) 
        : IServiceHttpClientAssembly
    {
        #region private readonly properties
        
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<HttpBlazorClient> _logger = logger;
        private readonly IOptions<UrlApiBlazorClient> _options = options;
        #endregion


        #region override method
        /// <summary>
        /// override de la méthode AuthenticateAsync pour gérer les exceptions et logger les erreurs
        /// pour l'authentification  sur le serveur Api BFF du serveur blazor
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public async Task<ResponseApiAuthenticate> AuthenticateAsync(RegisterDTO register)
        {
            try
            {
                HttpContent content = new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await _httpClient.PostAsync(_options.Value.Login, content);
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
            catch (Exception ex) {
                _logger.LogError(ex, "Erreur lors de l'authentification du client blazor, {message}", ex.Message);
                return new ResponseApiAuthenticate
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Erreur lors de l'authentification du client blazor"
                };
            }
        }

        #endregion

        #region public method interface IServiceHttpClientAssembly
        /// <summary>
        /// retourne les userinfos de la session du client
        /// </summary>
        /// <returns></returns>
        public async Task<UserInfos> GetUserInfosAsync ()
        {
            try
            {
                var adress = _options.Value.Me;
                var userInfos = await _httpClient.GetFromJsonAsync<UserInfos>(adress);

                return userInfos ?? new()
                {
                    IsAuthenticate = false,
                    Id = string.Empty
                };
            }
            catch ( Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des userinfos du client blazor, {message}", ex.Message);

                return new()
                {
                    IsAuthenticate = false,
                    Id = string.Empty
                };
            }
        }

        /// <summary>
        /// logout du client blazor vers le serveur blazor pour la suppression du cookie d'authentification
        /// logout se fait en post pour éviter les attaques CSRF
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogoutAsync()
        {
            try
            {
                var adress = _options.Value.Logout;

                var result = await _httpClient.PostAsync(adress, null);
                return result.IsSuccessStatusCode;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la déconnexion du client blazor, {message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Method qui remonte les groupes de Localisation pour un utilisateur donné, en fonction de son userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiLocalisations> GetUserGroupLocalisationAsync(string userId)
        {
            try
            {
                var adress = _options.Value.SaveGroupLocalisations;
                var result = await _httpClient.GetFromJsonAsync<ResponseApiLocalisations>(adress +"/"+ userId);
                return result ?? new() { 
                    ApiStatus = EnumApiStatus.NotFound,
                    Message = $"Aucun groupe de localisation trouvé pour l'utilisateur {userId}",
                    LocalisationGroupDTO = null,
                    GroupsDTO = []
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des groupes de localisation pour l'utilisateur {userId}, {message}", userId, ex.Message);
                return new ResponseApiLocalisations
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = $"Erreur lors de la récupération des groupes de localisation pour l'utilisateur {userId}"
                };
            }
        }
        #endregion
    }
}
