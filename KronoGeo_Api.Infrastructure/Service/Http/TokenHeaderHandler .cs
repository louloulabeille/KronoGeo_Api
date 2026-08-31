using KronoGeo_Api.Models.Infrastructure.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    /// <summary>
    /// class de mise en place du DelegatingHandler qui sera appélé lors de l'utilisation 
    /// du Httpclient a qui sera associé pour ajouter le jwtToken.
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class TokenHeaderHandler (IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        #region private readonly properties
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        #endregion

        #region protected method
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if ( httpContext is not null )
            {
                var jwtToken = await httpContext.GetTokenAsync("jwt_token");

                if ( !string.IsNullOrEmpty(jwtToken) )
                {
                    request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
