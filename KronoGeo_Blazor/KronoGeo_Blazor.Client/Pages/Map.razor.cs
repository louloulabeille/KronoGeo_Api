using System.Linq;
using System.Security.Claims;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace KronoGeo_Blazor.Client.Pages
{
    [Authorize]
    public class MapBase : ComponentBase
    {
        #region private inject properties
        [Inject]
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;
        [Inject]
        private AuthenticationStateProvider? _authenticationStateProvider { get; set; } = default;
        #endregion

        #region protected properties view
        protected bool IsLoading { get; set; } = false;
        protected List<LocalisationGroup>? LocalisationGroup { get; set; } = [];
        #endregion

        #region protected override method
        protected async override Task OnInitializedAsync()
        {
            IsLoading = true; // -- mise en place du loader trop rapide le chargement
            await GetLoadGroupLocationAsync();
            
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                IsLoading = false;
                StateHasChanged();
            }
            base.OnAfterRender(firstRender);
        }

        #endregion

        #region private method 
        /// <summary>
        /// method qui va rechercher les groupes de localisation de l'utilisateur connecté
        /// </summary>
        /// <returns></returns>
        private async Task GetLoadGroupLocationAsync()
        {
            if (_serviceHttp is not null && _authenticationStateProvider is not null)
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var result = await _serviceHttp.GetUserGroupLocalisationAsync(user?.Value ?? string.Empty);

                LocalisationGroup = result?.GroupsDTO?
                    .Select(lg => new LocalisationGroup()
                    {
                        Id = lg.Id,
                        ApplicationUserId = lg.ApplicationUserId,
                        Date = lg.Date,
                        Name = lg.Name,
                        RouteTelemetry = lg.RouteTelemetry?.Get() ?? null,
                        Localisations = lg.Localisations?.Select(l => l.Get()).ToList()
                    }).ToList();
            }
        }
        #endregion

    }
}
