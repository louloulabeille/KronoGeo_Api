using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Infrastructure.Exceptions.Http;
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

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public partial class HttpClientManagerKronogeo
    {
        // -- private properties IOptions<UrlApiManager> _options
        // -- protected properties HttpClient 

        #region public method implement interface IServiceHttpManagerKronoGeo
        /// <summary>
        /// Method de modification de l'adresse mail vers l'API
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiAuthenticate> UpdateUserEmailAsync(ProfilUpdateEmailDTO register)
        {
            try
            {
                HttpContent content = new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json");
                using var response = await HttpClient.PostAsync(_options.Value.UpdateUserEmail, content);
                //response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var retour = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResponseApiAuthenticate>(retour, JsonOptions.GetJsonOptions())
                        ?? new ResponseApiAuthenticate
                        {
                            ApiStatus = EnumApiStatus.Problem,
                            Message = "Erreur lors de la désérialisation de la réponse de l'API."
                        };

                    return result;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new() { ApiStatus = EnumApiStatus.Unauthorized };
                }
                else
                {
                    return new()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = $" Status Code retour {response.StatusCode}",
                    };
                }
            }
            catch (SessionExpiredException)
            {
                return new()
                {
                    ApiStatus = EnumApiStatus.Unauthorized,
                };
            }
            catch (Exception)
            {
                return new()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Erreur lors de la modification du email du profil utilisateur."
                };
            }
        }

        /// <summary>
        /// Method qui modifie le mot de passe
        /// </summary>
        /// <param name="profil"></param>
        /// <returns></returns>
        public async Task<ResponseApiManager> UpdatePasswordAsync(ProfilDTO profil)
        {
            try
            {
                HttpContent content = new StringContent(JsonSerializer.Serialize(profil), Encoding.UTF8, "application/json");
                using var response = await HttpClient.PostAsync(_options.Value.UpdateUserPassword, content);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var retour = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResponseApiManager>(retour, JsonOptions.GetJsonOptions())
                        ?? new ()
                        {
                            ApiStatus = EnumApiStatus.Problem,
                            Message = "Erreur lors de la désérialisation de la réponse de l'API."
                        };

                    return result;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                { // -- gestion des Unauthorized
                    return new() { ApiStatus = EnumApiStatus.Unauthorized };
                }
                else
                {
                    return new()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = $" Status Code retour {response.StatusCode}",
                    };
                }
            }
            catch (SessionExpiredException)
            {
                return new()
                {
                    ApiStatus = EnumApiStatus.Unauthorized,
                };
            }
            catch (Exception)
            {
                return new()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Erreur lors de la modification du email du profil utilisateur."
                };
            }
        }

        /// <summary>
        /// method qui retourne les utilisateurs pour l'admin ou Manager
        /// /// </summary>
        /// <returns></returns>
        public async Task<ResponseApiManager> GetListUsersAsync()
        {
            try
            {
                using var response = await HttpClient.GetAsync(_options.Value.GetUsers);

                if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var retour = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResponseApiManager>(retour, JsonOptions.GetJsonOptions())
                        ?? new()
                        {
                            ApiStatus = EnumApiStatus.Problem,
                            Message = "Erreur lors de la désérialisation de la réponse de l'API."
                        };

                    return result;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                { // -- gestion des Unauthorized
                    return new() { ApiStatus = EnumApiStatus.Unauthorized };
                }
                else
                {
                    return new()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = $" Status Code retour {response.StatusCode}",
                    };
                }
            }
            catch (SessionExpiredException)
            {
                return new()
                {
                    ApiStatus = EnumApiStatus.Unauthorized,
                };
            }
            catch (Exception)
            {
                return new()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Erreur lors de la remonté de la liste des utilisateurs."
                };
            }
        }
        #endregion
    }
}
