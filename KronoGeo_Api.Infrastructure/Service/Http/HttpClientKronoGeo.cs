using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
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
using static Microsoft.Maui.ApplicationModel.Permissions;
using static System.Net.WebRequestMethods;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpClientKronoGeo(IOptions<UrlApiMetier> options, HttpClient httpClient
            , IOptions<TokenTunnel>? tokenTunnel) 
        : HttpClientDefault(httpClient, tokenTunnel), IServiceHttpKronoGeo
    {
        #region private properties
        private readonly IOptions<UrlApiMetier> _options = options;
        #endregion

        #region public method interface IServiceHttpKronoGeo
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

        /// <summary>
        /// method de récupération des groupes de localisation pour un utilisateur
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseApiLocalisations> GetUserGroupLocalisationAsync()
        {
            /*if( string.IsNullOrEmpty(userId) ) throw new ArgumentNullException(nameof(userId), "L'identifiant de l'utilisateur est null ou vide.");
            
            var url = $"{_options.Value.GetUserGroupLocalisation}/{WebUtility.UrlEncode(userId)}";
            */
            var url = $"{_options.Value.GetUserGroupLocalisation}";

            using var retour = await HttpClient.GetAsync(url);
            retour.EnsureSuccessStatusCode();

            var result = await retour.Content.ReadAsStringAsync();
            var deserializedResult = JsonSerializer.Deserialize<ResponseApiLocalisations>(result, JsonOptions.GetJsonOptions());
            return deserializedResult ?? new ResponseApiLocalisations
            {
                ApiStatus = EnumApiStatus.NotFound,
                Message = $"Aucun groupe de localisation trouvé pour l'utilisateur ",
                LocalisationGroupDTO = null,
                GroupsDTO = []
            };
        }

        /// <summary>
        /// method de mise à jour de localisationPhotoDTO pour un utilisateur
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> UpdateImageAsync(LocalisationPhotoDTO photo)
        {

            // -- sérialisation de localisationPhotoDTO en JSON
            var content = new StringContent(JsonSerializer.Serialize(photo), Encoding.UTF8, "application/json");

            var url = $"{_options.Value.UpdateImage}";
            // -- envoi en Http 
            using var retour = await HttpClient.PostAsync(url, content);
            retour.EnsureSuccessStatusCode();

            if( retour.IsSuccessStatusCode )
            {
                var result = await retour.Content.ReadAsStringAsync();
                var deserializedResult = JsonSerializer.Deserialize<bool>(result, JsonOptions.GetJsonOptions());
                return deserializedResult;
            }

            return false;
        }

        /// <summary>
        /// method de suppression de localisationPhotoDTO pour un utilisateur
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImageAsync(int id)
        {
            var url = $"{_options.Value.DeleteImage}/{WebUtility.UrlEncode(id.ToString())}";

            using var result = await HttpClient.DeleteAsync(url);
            result.EnsureSuccessStatusCode();

            if (result.IsSuccessStatusCode)
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                var deserializedResult = JsonSerializer.Deserialize<bool>(responseContent, JsonOptions.GetJsonOptions());
                return deserializedResult;
            }

            return false;
        }

        /// <summary>
        /// method qui retourne un groupe de localisation par son identifiant
        /// </summary>
        /// <param name="IdLocalisationGroup"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiLocalisations> GetLocalisationsByIdAsync(int IdLocalisationGroup)
        {
            var url = $"{_options.Value.GetLocalisationsById}/{WebUtility.UrlEncode(IdLocalisationGroup.ToString())}";

            using var result = await HttpClient.GetAsync(url);
            result.EnsureSuccessStatusCode();

            if( result.IsSuccessStatusCode )
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                var deserializedResult = JsonSerializer.Deserialize<ResponseApiLocalisations>(responseContent, JsonOptions.GetJsonOptions());
                return deserializedResult ??
                    new ResponseApiLocalisations() {
                        ApiStatus = EnumApiStatus.NotFound,
                        Message = $"Aucun groupe de localisation trouvé pour l'identifiant {IdLocalisationGroup}.",
                        LocalisationGroupDTO = null,
                        GroupsDTO = []
                    };
            }

            return new ResponseApiLocalisations()
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = $"Probleme lors de la récupération des localisations pour l'identifiant {IdLocalisationGroup}.",
                LocalisationGroupDTO = null,
                GroupsDTO = []
            };
        }

        /// <summary>
        /// Modifie LocalisationGroup 
        /// </summary>
        /// <param name="localisationGroup"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiLocalisations> UpdateLocalisationGroupAsync(LocalisationGroupDTO localisationGroup)
        {
            var url = $"{_options.Value.UpdateLocalisationGroup}";
            // -- sérialisation de la localisation Group
            var content = new StringContent(JsonSerializer.Serialize(localisationGroup), Encoding.UTF8, "application/json");

            // -- envoi en Httpclient
            using var result = await HttpClient.PostAsync(url, content);
            result.EnsureSuccessStatusCode();

            if (result.IsSuccessStatusCode)
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                var deserializedResult = JsonSerializer.Deserialize<ResponseApiLocalisations>(responseContent, JsonOptions.GetJsonOptions());
                return deserializedResult ??
                    new ResponseApiLocalisations()
                    {
                        ApiStatus = EnumApiStatus.BadRequest,
                        Message = $"Erreur lors de l'enregistrement de {localisationGroup.Id}.",
                        LocalisationGroupDTO = null,
                        GroupsDTO = []
                    };
            }

            return new ResponseApiLocalisations()
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = $"Probleme lorsd l'enregistrement pour {localisationGroup.Id}.",
                LocalisationGroupDTO = null,
                GroupsDTO = []
            };
        }

        /// <summary>
        /// Supprime le localisation group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeleteLocalisationGroupAsync(int id)
        {
            var url = $"{_options.Value.DeleteLocalisationGroup}/{WebUtility.UrlEncode(id.ToString())}";
            
            using var result = await HttpClient.DeleteAsync(url);
            result.EnsureSuccessStatusCode();

            if (result.IsSuccessStatusCode)
            {
                var responseContent = await result.Content.ReadAsStringAsync();
                var content = JsonSerializer.Deserialize<bool>(responseContent, JsonOptions.GetJsonOptions());
                return content;
            }
            
            return false;
        }

        #endregion

        #region public method interface IDisposable
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        

    }
}
