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
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpClientKronoGeo  : IServiceHttpKronoGeo, IDisposable
    {
        #region private properties
        private readonly IOptions<UrlApi> _options;
        private readonly IOptions<TokenTunnel>? _tokenTunnel;
        #endregion

        #region protected properties
        protected readonly HttpClient HttpClient;
        #endregion

        #region constructeur
        public HttpClientKronoGeo(IOptions<UrlApi> options, HttpClient httpClient
            , IOptions<TokenTunnel>? tokenTunnel )
        {
            HttpClient = httpClient;
            _options = options;

            // - initialise les protections du tunnel pour le debug
#if DEBUG
            _tokenTunnel = tokenTunnel;
            ChargingTokenTunnel();
#endif


        }
        #endregion

        #region public method interface IServiceHttpKronoGeo
        /// <summary>
        /// method d'envoi des données pour vérification du login et mot de passe
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public virtual async Task<ResponseApiAuthenticate> AuthenticateAsync( RegisterDTO register )
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
        /// Sauvegarde un group de localisation au niveau du serveur Api
        /// </summary>
        /// <param name="localisationGroup"></param>
        /// <param name="tokkenBearer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<ResponseApiLocalisations> SaveGroupLocalisationsAsync(LocalisationGroupDTO localisationGroup, string tokkenBearer)
        {
            if ( localisationGroup.Localisations is not null )
            {
                HttpContent content = new StringContent(JsonSerializer.Serialize(localisationGroup), Encoding.UTF8, "application/json");
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokkenBearer);
                
                using var retour = await HttpClient.PostAsync(_options.Value.SaveGroupLocalisations, content);
                retour.EnsureSuccessStatusCode();

                if (retour.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ResponseApiLocalisations>(retour.Content.ReadAsStringAsync().Result, JsonOptions.GetJsonOptions());
                        
                    if ( result is not null )
                    {
                        return result;
                    }

                    return new()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = "Erreur lors de la désérialisation de la réponse de l'API."
                    };
                }
                else
                {
                    return new()
                    {
                        ApiStatus = EnumApiStatus.BadRequest,
                        Message = $" Status Code retour {retour.IsSuccessStatusCode}"
                    };
                }
            }

            throw new ArgumentNullException(nameof(localisationGroup), "Les localisations sont nulles. Pas d'enregistrements.");
        }       

        /// <summary>
        /// method d'envoi de la photo pour sauvegarde sur le serveur API
        /// avec retour d'un objet PhotoDTO 
        /// avec le chemin de la photo sur le serveur et le nom de la photo
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task<ResponseApiImage> SavePhotoAsync(PhotoDTO photo, string tokkenBearer)
        {
            if ( photo.PathComplet is null )
            {
                throw new ArgumentNullException(nameof(photo));
            }

            using var multipartContent = new MultipartFormDataContent();

            // -- Load the file and set the file's Content-Type header
            var streamContent = new StreamContent(System.IO.File.OpenRead(photo.PathComplet));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            // -- Add the file content to the multipart content
            multipartContent.Add(streamContent,"file",photo.Name??string.Empty);

            // send it
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokkenBearer);
            using var retour = await HttpClient.PostAsync(_options.Value.SavePhoto, multipartContent);
            retour.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<PhotoDTO>(retour.Content.ReadAsStringAsync().Result, JsonOptions.GetJsonOptions())
                ?? new PhotoDTO
                {
                    PathPhoto = null,
                    Name = null
                };

            if (retour.IsSuccessStatusCode)
            {
                return new ResponseApiImage
                {
                    ApiStatus = EnumApiStatus.Success,
                    Message = "Photo saved successfully",
                    PhotoDTO = result
                };
            }
            else
            {
                return new ResponseApiImage
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Error saving photo",
                    PhotoDTO = result
                };
            }

        }

        #endregion

        #region public method interface IDisposable
        public void Dispose()
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
            HttpClient.DefaultRequestHeaders.Add("X-Tunnel-Authorization", $"{token}");
            //_httpClient.DefaultRequestHeaders.Add("X-Tunnel-Authorization", "tunnel eyJhbGciOiJFUzI1NiIsImtpZCI6IjcyRjZDNUU3OEE2M0UzOEUxM0UyOTE1MjM0NjMyMDFGMDFDMzQ2MTUiLCJ0eXAiOiJKV1QifQ.eyJjbHVzdGVySWQiOiJldXciLCJ0dW5uZWxJZCI6InBlYWNlZnVsLWNoYWlyLWI2Y3ZnYzIiLCJzY3AiOiJjb25uZWN0IiwiZXhwIjoxNzg3MDUxMzQ3LCJpc3MiOiJodHRwczovL3R1bm5lbHMuYXBpLnZpc3VhbHN0dWRpby5jb20vIiwibmJmIjoxNzg2OTY0MDQ3fQ.UBF-WTYmgM1qIUJ9lGL7ElALXBZOqXK4ZXKJ3y4qZ-niUxOoAcvApYd3tFyhpZgAodbtNHEz-CqVmliesx__bw");
        }
        #endregion

    }
}
