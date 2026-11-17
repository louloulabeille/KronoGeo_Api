using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Model.DTO.RegionBounds;
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
        public Task<ResponseApiBool> UpdateImageAsync(LocalisationPhotoDTO photo );
        public Task<ResponseApiLocalisations> GetLocalisationsByIdAsync(int IdLocalisationGroup);
        public Task<ResponseApiBool> DeleteImageAsync(LocalisationPhotoDTO photo);
        public Task<ResponseApiLocalisations> UpdateLocalisationGroupAsync(LocalisationGroupDTO localisationGroup);
        public Task<ResponseApiBool> DeleteLocalisationGroupAsync(LocalisationGroupDTO localisationGroup);
        /// <summary>
        /// Api BFF method d'appel vers l'API principal pour 
        /// retourner la liste des Pins de la région de map demandée
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public Task<ResponseApiPinRegionBounds> GetPinByRegionBounds(RegionBountsDTO region);
    }
}
