using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceHttpManagerKronoGeo
    {
        public Task<ResponseApiAuthenticate> AuthenticateAsync(RegisterDTO register);
        public Task<ResponseApiManager> GetProfileAsync();
        public Task<ResponseApiManager> UpdateProfilAsync(ProfilDTO profil);
        public Task<ResponseApiAuthenticate> UpdateUserEmailAsync(ProfilUpdateEmailDTO register);
        public Task<ResponseApiManager> UpdatePasswordAsync(ProfilDTO profil);
        public Task<ResponseApiManager> GetListUsersAsync();
    }
}
