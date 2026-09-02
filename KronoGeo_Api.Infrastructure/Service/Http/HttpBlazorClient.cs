using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Parameter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

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
        : HttpClientKronoGeo(options, httpClient, null), IServiceHttpClientAssembly
    {
        #region private readonly properties
        private readonly ILogger<HttpBlazorClient> _logger = logger;
        #endregion


        #region override method
        /// <summary>
        /// override de la méthode AuthenticateAsync pour gérer les exceptions et logger les erreurs
        /// pour l'authentification  sur le serveur Api BFF du serveur blazor
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public override Task<ResponseApiAuthenticate> AuthenticateAsync(RegisterDTO register)
        {
            try
            {
                return base.AuthenticateAsync(register);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Erreur lors de l'authentification du client blazor, {message}", ex.Message);
                return Task.FromResult(new ResponseApiAuthenticate
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Erreur lors de l'authentification du client blazor"
                });
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
                var adress = options.Value.Me;
                var userInfos = await HttpClient.GetFromJsonAsync<UserInfos>(adress);

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
                var adress = options.Value.Logout;

                var result = await HttpClient.PostAsync(adress, null);
                return result.IsSuccessStatusCode;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la déconnexion du client blazor, {message}", ex.Message);
                return false;
            }
        }
        #endregion
    }
}
