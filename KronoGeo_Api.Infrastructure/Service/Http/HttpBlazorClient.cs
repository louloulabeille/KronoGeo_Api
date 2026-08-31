using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpBlazorClient(IOptions<UrlApiBlazorClient> options , HttpClient httpClient) 
        : HttpClientKronoGeo(options, httpClient, null), IServiceHttpClientAssembly
    {
        #region override method
        public override Task<ResponseApiAuthenticate> AuthenticateAsync(RegisterDTO register)
        { 
            return base.AuthenticateAsync(register);
        }
        #endregion

        #region public method
        /// <summary>
        /// retourne les userinfos de la session du client
        /// </summary>
        /// <returns></returns>
        public async Task<UserInfos> GetUserInfosAsync ()
        {
            var userInfos = await HttpClient.GetFromJsonAsync<UserInfos> (options.Value.Me);

            return userInfos ?? new()
            {
                IsAuthenticate = false,
                Id = string.Empty
            };
        }
        #endregion
    }
}
