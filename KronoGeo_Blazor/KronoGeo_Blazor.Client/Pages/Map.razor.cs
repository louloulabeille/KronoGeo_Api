using System.Linq;
using System.Security.Claims;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.ValueGeneration;

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
        [Inject]
        private IMapStateService? _mapState { get; set; } = default;
        #endregion

        #region protected properties view
        protected bool IsLoading { get; set; } = false;
        // -- recherche dans les groupes de localisations
        protected string Search { get; set; } = string.Empty;
        

        /// <summary>
        /// liste des groupes de localisation de l'utilisateur connecté
        /// par page de 10 éléments par défaut
        /// avec le filtre search ou non
        /// </summary>
        protected IEnumerable<LocalisationGroup>? DataOnView => _filterLocalisationGroup?
            .OrderByDescending(lg => lg.Date).Skip((PageActuel - 1) * _pageSize).Take(_pageSize).ToList();


        // -- pagination calcul
        protected int PageActuel = 1;

        /// <summary>
        /// nombre total de page pour la pagination
        /// nombre / pageSize + 1 si reste > 0 calcul avec le modulo
        /// </summary>
        protected int TotalPages =>
            (_filterLocalisationGroup?.Count ?? 0) / _pageSize + ((_filterLocalisationGroup?.Count ?? 0) % _pageSize > 0 ? 1 : 0);

        /// <summary>
        /// indique si le bouton précédent est désactivé ou non
        /// </summary>
        protected bool DisabledPreviousPage => PageActuel <= 1;
        /// <summary>
        /// indique si le bouton suivant est désactivé ou non
        /// </summary>
        protected bool DisabledNextPage => PageActuel >= TotalPages;
        /// <summary>
        /// nombre de boutons numériques visibles pour la pagination
        /// </summary>
        protected int VisibleNumericButtonCount { get; set; } = 3; 
        #endregion

        #region private properties
        /// <summary>
        /// data de localisation group de l'utilisateur connecté
        /// </summary>
        private List<LocalisationGroup>? _localisationGroup { get; set; } = [];

        private List<LocalisationGroup>? _filterLocalisationGroup { get; set; } = [];

        // -- nombre d'éléments par page
        private readonly int _pageSize = 10;
        #endregion

        #region protected override method
        protected async override Task OnInitializedAsync()
        {
            IsLoading = true; // -- mise en place du loader trop rapide le chargement
            await GetLoadGroupLocationAsync();

            // -- initialisation du filter List avec tous les enregistrements
            _filterLocalisationGroup = _localisationGroup;

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

        #region public method event
        protected async Task FilterSearch()
        {
            PageActuel = 1;
            _filterLocalisationGroup = _localisationGroup?.Where(w => w.Name.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        #endregion

        #region protected method

        protected void PreviousPage()
        {
            if (PageActuel > 1)
            {
                PageActuel--;
                ChangePage(PageActuel);
            }
        }

        protected void NextPage()
        {
            if (PageActuel < TotalPages)
            {
                PageActuel++;
                ChangePage(PageActuel);
            }
        }

        protected void ChangePage(int page)
        {
            if (DataOnView is null || _localisationGroup is null ) return;

            PageActuel = page;
            StateHasChanged();
        }

        protected void ChargeLocationsOnMap(LocalisationGroup group)
        {
            if (group is null || group.Localisations is null || group.Localisations.Count == 0) return;

            _mapState?.OpenMapwithLocalisations(group.Localisations);

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

                _localisationGroup = result?.GroupsDTO?
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
