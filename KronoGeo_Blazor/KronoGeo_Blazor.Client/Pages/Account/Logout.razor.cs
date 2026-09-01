using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KronoGeo_Blazor.Client.Pages.Account
{
    public class LogoutBase : ComponentBase
    {
        #region inject properties
        [Inject]
        private NavigationManager? _navigation { get; set; }
        [Inject]
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default!;
        [Inject]
        private AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        #endregion

        #region override method
        /// <summary>
        /// après le rendu de la page lance la méthode de logout vers le serveur pour la suppression du cookie d'authentification
        /// </summary>
        /// <param name="firstRender"></param>
        /*protected async override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            await LogoutAsync();
        }*/
        protected async override Task OnInitializedAsync()
        {
            await LogoutAsync();
            await base.OnInitializedAsync();
        }
        #endregion

        #region private methods
        /// <summary>
        /// method de suppression du cookie d'authentification sur le serveur blazor
        /// </summary>
        /// <returns></returns>
        private async Task LogoutAsync()
        {
            if (_serviceHttp == null)
            {
                _navigation?.NavigateTo("/");
                return;
            }

            var result = await _serviceHttp.LogoutAsync();
            if (result)
            {
                if (_authenticationStateProvider is BffAuthentificationStateProvider customAuthProvider)
                {
                    customAuthProvider.NotifyAuthenticationStateChanged();
                }
                _navigation?.NavigateTo("/");
            }
            else
            {
                // Handle logout failure if needed
                _navigation?.NavigateTo("map");
            }
        #endregion
        }

    }
}
