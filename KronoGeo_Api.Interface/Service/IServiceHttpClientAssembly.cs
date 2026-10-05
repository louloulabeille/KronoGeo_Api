using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    /// <summary>
    /// interface de connexion pour le client web Assembly 
    /// method spécifique
    /// </summary>
    public interface IServiceHttpClientAssembly
    {
        public Task<ResponseApiAuthenticateBlazor> AuthenticateAsync(RegisterDTO register);
        public Task<UserInfos> GetUserInfosAsync();
        public Task<bool> LogoutAsync();
        public Task<ResponseApiLocalisations> GetUserGroupLocalisationAsync();
        public Task<bool> UpdateImageAsync(LocalisationPhotoDTO photo );
        public Task<ResponseApiLocalisations> GetLocalisationsByIdAsync(int localisationGroupId);
    }
}
