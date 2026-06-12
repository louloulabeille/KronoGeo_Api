using KronoGeo_Api.Applications.Model.DTO;
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
        public Task<HttpResponseMessage> AuthenticateAsync( RegisterDTO register );
    }
}
