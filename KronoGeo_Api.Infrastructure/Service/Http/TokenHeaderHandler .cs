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
        // -- pour la récupération de l'object httpcontext
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        #endregion

        #region protected method
        /// <summary>
        /// method delegatingHandler qui est appelé de l"utilisation du Httpclient
        /// pour injecter authomatiquement le token bearer dans l'entête
        /// si le token existe
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if ( httpContext is not null )
            {
                // -- récupération du token du cookie du client
                var jwtToken = await httpContext.GetTokenAsync("jwt_token");

                if ( !string.IsNullOrEmpty(jwtToken) )
                {// -- ajout de l'entête authorized avec le tokken
                    request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
