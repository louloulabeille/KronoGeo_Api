using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KronoGeo_Blazor.Components.Account.Pages
{
    public class AuthenticateBase : ComponentBase
    {
        #region private readonly properties Injection
        [Inject]
        private IServiceHttpKronoGeo? _serviceHttp { get; set; } = default;
        [Inject]
        private UserTokenContainer? _userTokenContainer { get; set; }
        [Inject]
        private ProtectedSessionStorage? _sessionStorage { get; set; }
        [Inject]
        private NavigationManager? _navigationManager { get; set; }
        [Inject]
        private ILogger<AuthenticateBase>? _logger { get; set; }
        #endregion

        #region protected method 
        protected RegisterDTO Login { get; set; } = new() { Login = string.Empty };
        protected bool ErreurLogin { get; set; } = false;
        protected bool ErreurMessage { get; set; } = false;
        protected bool ErreurLock { get; set; } = false;
        #endregion

        #region override method
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        #endregion

        #region protected method
        protected async Task FormAuthenticate()
        {
            ErreurMessage = false;
            ErreurLogin = false;
            ErreurLock = false;
            try
            {
                if (_serviceHttp is not null)
                {
                    // -- requete vers l'APi
                    var result = await _serviceHttp.AuthenticateAsync(Login);
                    if ( result.IsSuccess)
                    {
                        if ( !string.IsNullOrEmpty(result.Register?.Token) )
                        {
                            _userTokenContainer?.AccessToken = result.Register?.Token;
                            _sessionStorage?.SetAsync("userId", result.Register!.Id);
                            _navigationManager?.NavigateTo("Map");
                        }
                    }
                    else
                    {
                        if( result.IsNotFound )
                        {
                            ErreurLogin = true;
                        }
                        else
                        {
                            if ( result.IsLocked )
                            {
                                ErreurLock = true;
                            }
                            else
                            {
                                ErreurMessage = true;
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                ErreurMessage = true;
                _logger?.LogError(ex, "Erreur interne {message}", ex.Message);
            }
            
        }
        #endregion
    }
}
