using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using NetTopologySuite.Index.HPRtree;

namespace KronoGeo_Blazor.Components.Pages.Account
{
    [Authorize(Roles = "Admin,Manager")]
    public class GestionAccountBase : ComponentBase
    {
        #region private properties inject
        [Inject]
        private IMediator? _mediator { get; set; } = default;
        [Inject]
        private ToastsService? _toastService { get; set; } = default;
        [Inject]
        private NavigationManager? _navigationManager { get; set; } = default;
        #endregion

        #region properties protected 
        // -- pagination
        protected IEnumerable<ProfilDTOAdmin>? DataOnView => _profils
            .OrderBy(lg => lg.Login).Skip((PageActuel - 1) * _pageSize).Take(_pageSize).ToList();
        protected int PageActuel { get; set; } = 1;
        // ----------

        // -- Dictionnaire pour désactiver le bouton de sauvegarde pour chaque utilisateur
        protected Dictionary<string,bool> DesabledSaveUser { get; set; } = [];
        #endregion

        #region private properties
        // -- liste initial
        private List<ProfilDTOAdmin> _profils { get; set; } = new();
        private int _pageSize = 15;
        #endregion

        #region protected override method
        protected override async Task OnInitializedAsync()
        {
            await InitListUsers();
            await base.OnInitializedAsync();
        }
        #endregion

        #region protected method
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="user"></param>
        protected void OnDeleteUSer(ProfilDTOAdmin? user)
        {
            if (user is not null && _profils.Contains(user))
            {
                _profils.Remove(user);
                _toastService?.SuccesAsync("Supression réussie.");
            }
            StateHasChanged();
        }

        /// <summary>
        /// Save USer
        /// </summary>
        /// <param name="user"></param>
        protected void OnSaveUser(ProfilDTOAdmin? user)
        {
            if (user is not null && _profils.Contains(user))
            {
                var index = _profils.IndexOf(user);
                _profils[index] = user;
            }
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        protected void OnUserRolesChanged (ProfilDTOAdmin? user)
        {
            if (user is not null && _profils.Contains(user))
            {
                var index = _profils.IndexOf(user);
                _profils[index] = user;
                DesabledSaveUser[user.Id ?? string.Empty] = false;
            }
        }
        #endregion

        #region private method
        private async Task InitListUsers()
        {
            if (_mediator is not null)
            {
                var result = await _mediator.Send(new GetAllUsersCommand());
                if (result.IsSuccess && result.Profils is not null)
                {
                    _profils.AddRange(result.Profils);
                    foreach (var item in _profils)
                    {
                        DesabledSaveUser.Add(item.Id ?? string.Empty, true);
                    }
                    return;
                }
                if (result.IsUnauthorized)
                {
                    _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                }
                _toastService?.AvertissementAsync("Problème lors de la récupération des utilisateurs");
                return;
            }

            _toastService?.ErreurAsync("Impossible de remonter la liste. Erreur au niveau du navigateur.");
        }
        #endregion
    }
}
