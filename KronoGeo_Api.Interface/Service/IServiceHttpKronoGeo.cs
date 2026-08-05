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
        public Task<ResponseApiAuthenticate> AuthenticateAsync( RegisterDTO register );
        public Task<ResponseApiLocalisations> SaveGroupLocalisationsAsync( LocalisationGroupDTO localisationGroup );
        public Task<ResponseApiImage> SavePhotoAsync(PhotoDTO photo);
    }
}
