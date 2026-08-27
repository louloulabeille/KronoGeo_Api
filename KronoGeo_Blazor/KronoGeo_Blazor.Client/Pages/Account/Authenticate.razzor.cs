using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using Mapsui.Logging;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages.Account
{
    public class AuthenticateBase : ComponentBase
    {

        #region private inject properties
        [Inject]
        private IServiceHttpKronoGeo? _serviceHttp { get; set; } = default;
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
                    if (result.IsSuccess)
                    {
                        if (!string.IsNullOrEmpty(result.Register?.Token))
                        {
                            var sessionId = Guid.NewGuid().ToString();
                            // -- enregistrement 
                            _navigationManager?.NavigateTo("Map");
                        }
                    }
                    else
                    {
                        if (result.IsNotFound)
                        {
                            ErreurLogin = true;
                        }
                        else
                        {
                            if (result.IsLocked)
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
            }
            catch (Exception ex)
            {
                ErreurMessage = true;
                _logger?.LogError(ex, "Erreur interne {message}", ex.Message);
            }

        }
        #endregion
    }
}
