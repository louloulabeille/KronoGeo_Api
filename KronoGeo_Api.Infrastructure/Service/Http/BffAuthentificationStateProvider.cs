using KronoGeo_Api.Interface.Service;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    /// <summary>
    /// classe provider attaché au HttpClient pour la gestion de l'état d'authentification de l'utilisateur
    /// </summary>
    /// <param name="httpClient"></param>
    public class BffAuthentificationStateProvider( IServiceHttpClientAssembly httpClient) : AuthenticationStateProvider
    {

        #region private readonly properties
        private readonly IServiceHttpClientAssembly _httpClient = httpClient;
        #endregion

        #region private properties
        private AuthenticationState? _authenticationState = default;
        #endregion

        #region public method override
        /// <summary>
        /// retourne l'état d'authentification de l'utilisateur,
        /// cette methode est appelé par le framework Blazor pour déterminer l'état d'authentification de l'utilisateur
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if ( _authenticationState is not null) return _authenticationState;

            try
            {
               var userinfos = await _httpClient.GetUserInfosAsync();

                if (userinfos is not null && userinfos.IsAuthenticate)
                {
                    var claims = userinfos.Claims.Select(c => new Claim(c.Key, c.Value)).ToList();
                    if ( !string.IsNullOrEmpty(userinfos.Id) )
                    {
                        //claims.Add(new Claim(ClaimTypes.NameIdentifier, userinfos.Id));
                        claims.Add(new Claim(ClaimTypes.Name, userinfos.Login));
                        foreach (var role in userinfos.Roles) { 
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }
                    var identity = new ClaimsIdentity(claims, authenticationType:"BffAuth");
                    var user = new ClaimsPrincipal(identity);

                    _authenticationState = new(user);
                    return _authenticationState;
                }

                return SetAnonymousState();
            }
            catch
            {
                return SetAnonymousState();
            }
        }

        #endregion

        #region private method
        /// <summary>
        /// retourne un état d'authentification anonyme
        /// </summary>
        /// <returns></returns>
        private AuthenticationState SetAnonymousState()
        {
            var anomymous = new ClaimsPrincipal(new ClaimsIdentity());
            _authenticationState = new(anomymous);
            return _authenticationState;
        }
        #endregion

        #region public method
        /// <summary>
        /// notifie le changement d'état d'authentification
        /// </summary>
        public void NotifyAuthenticationStateChanged()
        {
            _authenticationState = SetAnonymousState();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        #endregion
    }
}
