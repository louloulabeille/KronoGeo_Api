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
    public interface IServiceHttpClientAssembly : IServiceHttpKronoGeo
    {
        public Task<UserInfos> GetUserInfosAsync();
        public Task<bool> LogoutAsync();
    }
}
