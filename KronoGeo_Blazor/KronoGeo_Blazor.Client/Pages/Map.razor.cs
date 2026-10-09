using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Linq;
using System.Security.Claims;

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
        /// <summary>
        /// Service pour l'affichage des notifications toast
        /// </summary>
        [Inject]
        private ToastsService? _toastService { get; set; } = default;
        #endregion

        #region protected properties view
        protected bool IsLoading { get; set; } = false;
        // -- recherche dans les groupes de localisations
        protected string Search { get; set; } = string.Empty;
        /// <summary>
        /// indique si la carte est ouverte ou non
        /// </summary>
        protected bool IsMapOpen { get; set; } = true;
        /// <summary>
        /// liste des groupes de localisation de l'utilisateur connecté
        /// par page de 10 éléments par défaut
        /// avec le filtre search ou non
        /// </summary>
        protected IEnumerable<LocalisationGroup>? DataOnView => _filterLocalisationGroup?
            .OrderByDescending(lg => lg.Date).Skip((PageActuel - 1) * _pageSize).Take(_pageSize).ToList();

        /// <summary>
        /// Id de Localsiation Group sélectionné pour l'affichage des localisations 
        /// & pour la supression
        /// </summary>
        protected LocalisationGroup? SelectedGroup { get; set; }
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
            _mapState?.OpenMapwithLocalisations(null); // -- initialise la Map
            PageActuel = page;
            StateHasChanged();
        }

        /// <summary>
        /// charge la liste de localisations pour idgrouplocalisation données
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        protected async Task ChargeLocationsOnMap(LocalisationGroup group)
        {
            // -- initialisation du select localisationgroup 
            SelectedGroup = null;
            IsMapOpen = true;

            if (_serviceHttp is null) {
                if (_toastService is not null)
                    // -- affichage d'un message de succès
                    await _toastService.ErreurAsync("Le service de récupération des localisations est indisponible.");
                return;
            }

            var result = await _serviceHttp.GetLocalisationsByIdAsync(group.Id);
            if ( result.IsSuccess ) 
                group.Localisations = result.LocalisationGroupDTO?.Localisations?.Select(l => l.Get()).ToList();
            else
            {
                if (_toastService is not null)
                    // -- affichage d'un message d'erreur
                    await _toastService.ErreurAsync(result.Message??"Erreur lors de la récupération de la liste de localisations.");
            }

            if (group is null || group.Localisations is null || group.Localisations.Count == 0) return;
            // -- programmer la récupération des localisations recharger du serveur
            // pour éviter les problèmes de mémoire si l'utilisateur a beaucoup de localisations
            _mapState?.OpenMapwithLocalisations(group.Localisations);

        }

        /// <summary>
        /// ouvre le formulaire d'édition du groupe de localisation
        /// </summary>
        /// <param name="localisationGroup"></param>
        protected void EditLocalisation (LocalisationGroup localisationGroup)
        {
            SelectedGroup = localisationGroup;
            IsMapOpen = false;

            StateHasChanged();
        }

        protected void DeleteLocalisation(LocalisationGroup localisationGroup)
        {
            SelectedGroup = localisationGroup;
            
            StateHasChanged();
        }

        /// <summary>
        /// ferme le formulaire d'édition du groupe de localisation et réaffiche la carte
        /// </summary>
        protected void CloseEditLocalisation()
        {
            //SelectedGroup = null;
            IsMapOpen = true;
            StateHasChanged();
        }

        protected void SaveLocalisationGroup()
        {
            var select = SelectedGroup;
            StateHasChanged();
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
                // -- récupération de l'utilisateur connecté pour récupérer les groupes de localisation
                //var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                // -- récupération du claim NameIdentifier pour récupérer l'id de l'utilisateur
                //var user = authState.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var result = await _serviceHttp.GetUserGroupLocalisationAsync();

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
