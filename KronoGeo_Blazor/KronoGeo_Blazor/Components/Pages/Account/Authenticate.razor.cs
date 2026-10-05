using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Components.Pages.Account
{
    public class AuthenticateBase : ComponentBase
    {
        #region private properties
        private RegisterDTO _authenticate = new() { Login = string.Empty, Password = string.Empty };
        #endregion

        #region private inject properties
        [Inject]
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;
        [Inject]
        private NavigationManager? _navigationManager { get; set; }
        /*[Inject]
        private ILogger<AuthenticateBase>? _logger { get; set; }*/
        [Inject]
        private IMediator? _mediator { get; set; } = default;

        #endregion

        #region cascading parameter
        [CascadingParameter] 
        private HttpContext _httpContext { get; set; } = default!;
        #endregion

        #region properties formulaire 
        [SupplyParameterFromForm]
        protected RegisterDTO Authenticate { get => _authenticate; set => _authenticate = value; }
        #endregion

        #region protected properties
        protected bool ErreurLogin { get; set; } = false;
        protected bool ErreurMessage { get; set; } = false;
        protected bool ErreurLock { get; set; } = false;
        #endregion

        #region method override
        protected override void OnInitialized()
        {
            _authenticate ??= new() { Login = string.Empty, Password = string.Empty };
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
                if (_serviceHttp is not null && _authenticate is not null)
                {
                    // -- requete vers l'APi
                    //var result = await _serviceHttp.AuthenticateAsync(_authenticate);
                    var result = await _mediator.Send(new LoginUserCommand() { Register = _authenticate });
                    if (result.IsSuccess)
                    {
                        if (!string.IsNullOrEmpty(result.Register?.Token))
                        {                        
                            if (result.ClaimsPrincipal is not null && result.AuthenticationProperties is not null)
                            {
                                // Cette ligne émet le Cookie HttpOnly de façon transparente !
                                await _httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                                    , result.ClaimsPrincipal, result.AuthenticationProperties);

                                result.Register.Token = string.Empty;
                            }
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
                Console.WriteLine($"Erreur interne : {ex.Message}");
                ErreurMessage = true;
                //_logger?.LogError(ex, "Erreur interne {message}", ex.Message);
            }

        }
        #endregion
    }
}
