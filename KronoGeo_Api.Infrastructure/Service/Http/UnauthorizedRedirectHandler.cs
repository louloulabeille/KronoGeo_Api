using KronoGeo_Api.Infrastructure.Exceptions.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class UnauthorizedRedirectHandler 
         : DelegatingHandler
        //(SessionNotificationService notificationService)
    {
        #region private properties
        // -- event handler qui va prendre en charge le code lors de la lever de exception
        // -- le mettre en place par exemple pour blazor dans Route.razzor
        // -- en Maui dans AppShell
        //private readonly IServiceProvider _provider = provider;
        //private readonly SessionNotificationService _notificationService = notificationService;
        #endregion


        /// <summary>
        /// lever d'une exception lors d'un statuscode a Unauthorized
        /// avec un delegatingHandler Http
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
                // -- on lance le code
                //var notificationService = _provider.GetRequiredService<SessionNotificationService>();
                //_notificationService.NotifySessionExpired();
                //Console.WriteLine($"UnauthorizedRedirectHandler guid :{_notificationService.Guid}");
                // -- on leve quand même l'exception au cas ou que event soit null
                throw new SessionExpiredException();
            }

            return response;
        }
    }
}
