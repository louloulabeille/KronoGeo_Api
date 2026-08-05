using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
        public Task<ResponseApiLocalisations> SaveGroupLocalisationsAsync(LocalisationGroupDTO localisationGroup)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// method d'envoi de la photo pour sauvegarde sur le serveur API
        /// avec retour d'un objet PhotoDTO 
        /// avec le chemin de la photo sur le serveur et le nom de la photo
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ResponseApiImage> SavePhotoAsync(PhotoDTO photo)
        {
            if ( photo.PathComplet is null )
            {
                throw new ArgumentNullException(nameof(photo));
            }

            using var multipartContent = new MultipartFormDataContent();

            // -- Load the file and set the file's Content-Type header
            var streamContent = new StreamContent(File.OpenRead(photo.PathComplet));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            // -- Add the file content to the multipart content
            multipartContent.Add(streamContent);

            // send it 
            using var retour = _httpClient.PostAsync(_options.Value.SavePhoto, multipartContent).Result;
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
    }
}
