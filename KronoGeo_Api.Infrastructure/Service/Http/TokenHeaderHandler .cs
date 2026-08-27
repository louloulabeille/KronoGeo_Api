using KronoGeo_Api.Models.Infrastructure.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class TokenHeaderHandler (IHttpContextAccessor httpContextAccessor
        , IMemoryCache memoryCache) : DelegatingHandler
    {
        #region private readonly properties
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IMemoryCache _memoryCache = memoryCache;
        #endregion

        #region protected method
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
             var httpContext = _httpContextAccessor.HttpContext;

            // -- récupération de id de la session
            var sessionId = httpContext.User.FindFirst("BffSessionId")?.Value;

            if (!string.IsNullOrEmpty(sessionId) && 
                _memoryCache.TryGetValue($"jwt_{sessionId}", out string? jwtToken))
            {
                
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
