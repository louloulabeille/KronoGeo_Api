using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    /// <summary>
    /// service de connexion vers Api
    /// </summary>
    public interface IServiceHttpKronoGeo
    {
        /// <summary>
        /// Enregistre un tracé complet de localisation
        /// </summary>
        /// <param name="localisationGroup"></param>
        /// <returns></returns>
        public Task<ResponseApiLocalisations> SaveGroupLocalisationsAsync(LocalisationGroupDTO localisationGroup);
        /// <summary>
        /// Enregistre la photo au niveau du serveur
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public Task<ResponseApiImage> SavePhotoAsync(PhotoDTO photo);
        /// <summary>
        /// Retourn tous les tracés d'un utilisateur
        /// </summary>
        /// <returns></returns>
        public Task<ResponseApiLocalisations> GetUserGroupLocalisationAsync();
        public Task<ResponseApiLocalisations> GetLocalisationsByIdAsync(int IdLocalisationGroup);
        public Task<ResponseApiLocalisations> UpdateLocalisationGroupAsync(LocalisationGroupDTO localisationGroup);
        public Task<ResponseApiBool> UpdateImageAsync(LocalisationPhotoDTO photo);
        public Task<ResponseApiBool> DeleteImageAsync(int id);
        public Task<ResponseApiBool> DeleteLocalisationGroupAsync(int id);
    }
}
