using Microsoft.AspNetCore.Components;
using System.Net;

namespace KronoGeo_Blazor.Infrastructure.Http
{
    public class SessionExpiredException : Exception;

    // -- Handler attaché au httpclient pour déconnecter lors de la réception d'un retour 401
    public class UnauthorizedRedirectHandler : DelegatingHandler
    {
        /// <summary>
        /// lever d'une exception lors d'un statuscode a Unauthorized
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        /// <exception cref="SessionExpiredException"></exception>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            var response = await base.SendAsync(request, ct);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new SessionExpiredException();
            }

            return response;
        }
    }
}
