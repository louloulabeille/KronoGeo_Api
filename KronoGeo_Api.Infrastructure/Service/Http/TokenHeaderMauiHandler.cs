using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    /// <summary>
    /// class Delegating handler pour ajouter le token automatiquement lors de l'appel du Httpclient
    /// </summary>
    /// <param name="saveUser"></param>
    public class TokenHeaderMauiHandler(IServiceSaveUser saveUser) : DelegatingHandler
    {
        #region private properties readonly
        private readonly IServiceSaveUser _saveUSer = saveUser;
        #endregion

        #region protected overide method 
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request
            , CancellationToken cancellationToken)
        {
            var user = await _saveUSer.GetRegister();

            if( user is not null ) {
                var token = user.Token;
                if( !string.IsNullOrEmpty(token) )
                {
                    // -- ajout du token
                    request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
        #endregion
    }
}
