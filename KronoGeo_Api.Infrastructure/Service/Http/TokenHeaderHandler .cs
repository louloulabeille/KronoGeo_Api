using KronoGeo_Api.Models.Infrastructure.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class TokenHeaderHandler : DelegatingHandler
    {
        #region private readonly properties
        private readonly UserTokenContainer _tokenContainer;
        #endregion

        #region constructeur
        public TokenHeaderHandler(UserTokenContainer tokenContainer)
        {
            _tokenContainer = tokenContainer;
        }
        #endregion

        #region protected method
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_tokenContainer.AccessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _tokenContainer.AccessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
