using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public partial class HttpBlazorClient : IServiceHttpManagerClientAssembly
    {

        /*private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<HttpBlazorClient> _logger = logger;
        private readonly IOptions<UrlApiBlazorClient> _options = options;*/


        #region public method interface IServiceHttpManagerClientAssembly
        /// <summary>
        /// Retourne la liste des utilisateurs
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseApiManager> GetListUsersAsync()
        {
            try
            {
                var url = _options.Value.GetUsers;

                using var result = await _httpClient.GetAsync(url);
                //result.EnsureSuccessStatusCode();

                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new()
                    {
                        ApiStatus = EnumApiStatus.Unauthorized
                    };
                }

                if ( result.IsSuccessStatusCode )
                {
                    var retour = await result.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ResponseApiManager>(retour, JsonOptions.GetJsonOptions());

                    return data ?? new()
                    {
                        ApiStatus = EnumApiStatus.NotFound,
                        Message = $"Aucun groupe de localisation trouvé",
                    };
                }

                return new() { ApiStatus = EnumApiStatus.Problem, Message = "Problème lors de la récupération de la liste des utilisateurs." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des groupes de localisation pour l'utilisateur : {message}", ex.Message);
                return new ()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = $"Erreur lors de la récupération des groupes de localisation"
                };
            }
        }
        #endregion

    }
}
