using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpBlazorClient(IOptions<UrlApi> options , HttpClient httpClient) 
        : HttpClientKronoGeo(options, httpClient, null)
    {
        #region override method
        public override Task<ResponseApiAuthenticate> AuthenticateAsync(RegisterDTO register)
        { 
            return base.AuthenticateAsync(register);
        }
        #endregion


    }
}
