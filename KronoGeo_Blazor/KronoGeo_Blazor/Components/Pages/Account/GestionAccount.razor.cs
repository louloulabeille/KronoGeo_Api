using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NetTopologySuite.Index.HPRtree;
using System.Security.Claims;

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
        // -- pour récupérer l'état de l'authentification de l'utilisateur
        [Inject]
        private AuthenticationStateProvider? _authenticationStateProvider { get; set; } = default;
        #endregion

        #region properties protected 
        // -- pagination
        // -- liste des profils à afficher sur la page
        protected IEnumerable<ProfilDTOAdmin>? DataOnView => _filterProfils
            .OrderBy(lg => lg.Login).Skip((PageActuel - 1) * _pageSize).Take(_pageSize).ToList();
        protected int PageActuel { get; set; } = 1;
        protected bool DisabledPreviousPage => PageActuel == 1;
        protected bool DisabledNextPage => PageActuel == TotalPages;
        /// <summary>
        /// nombre total de page pour la pagination
        /// nombre / pageSize + 1 si reste > 0 calcul avec le modulo
        /// </summary>
        protected int TotalPages =>
            (_filterProfils?.Count ?? 0) / _pageSize + ((_filterProfils?.Count ?? 0) % _pageSize > 0 ? 1 : 0);
        // ------------------------------------------------------//

        // -- Dictionnaire pour désactiver le bouton de sauvegarde pour chaque utilisateur
        protected Dictionary<string,bool> DesabledSaveUser { get; set; } = [];

        // -- Search pour la recherche d'utilisateur au niveau du login
        protected string Search { get; set; } = string.Empty;
        // -- user manager or not
        protected bool IsManager { get; set; } = false;
        #endregion

        #region private properties
        // -- liste initial
        private List<ProfilDTOAdmin> _profils { get; set; } = new();
        private int _pageSize = 15;
        // -- list des profils filtrés en fonction de la recherche
        protected List<ProfilDTOAdmin> _filterProfils { get; set; } = [];
        #endregion

        #region protected override method
        protected override async Task OnInitializedAsync()
        {
            await InitListUsers();
            // -- vérifie si l'utilisateur est un manager pour désactiver certaines fonctionnalités
            // -- au niveau du tableau de gestion des utilisateurs
            if ( _authenticationStateProvider is not null )
            {
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                var userRoles = authState.User.Identities.FirstOrDefault()?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                IsManager = userRoles?.Contains("Manager") == true;
            }
            await base.OnInitializedAsync();
        }
        #endregion

        #region protected method
        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="user"></param>
        protected async Task OnDeleteUSer(ProfilDTOAdmin? user)
        {
            try
            {
                // -- vérification si l'admin ou le manager ne peut pas supprimer son propre compte
                // -- vérification si l'utilisateur est un manager il ne peut pas supprimer un admin
                // -- juste une vérifiacation mais normalement le manager ne peut pas voir un compte admin dans sa liste
                // -- juste au cas ou
                if (_authenticationStateProvider is not null && user is not null)
                {
                    // -- récupération de l'utilisateur connecté pour récupérer les groupes de localisation
                    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                    // -- récupération du claim NameIdentifier pour récupérer l'id de l'utilisateur
                    var userId = authState.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    // -- récupération des roles de l'utilisateur connecté
                    var userRoles = authState.User.Identities.FirstOrDefault()?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                    if (userId is not null  && user.Id == userId.Value)
                    {
                        _toastService?.AvertissementAsync("Vous ne pouvez pas supprimer votre propre compte.");
                        return;
                    }
                    // -- vérification si l'utilisateur est un manager il ne peut pas supprimer un admin
                    if ( userRoles?.Contains("Manager") == true && user.Roles?.Contains("Admin") == true)
                    {
                        _toastService?.AvertissementAsync("Vous ne pouvez pas supprimer un utilisateur avec le rôle d'administrateur.");
                        return;
                    }
                }

                // -- suppression de l'utilisateur en utilisant le mediator pour envoyer la commande DeleteUserCommand
                if (user is not null && _profils.Contains(user) && _mediator is not null
                    && user.Id is not null)
                {
                    var result = await _mediator.Send(new DeleteUserCommand { Id = user.Id });

                    if(result.IsSuccess)
                    { // -- suppression réussie supression dans la liste aussi
                        _profils.Remove(user);
                        _toastService?.SuccesAsync("Suppression réussie.");
                    }
                    else if (result.IsUnauthorized)
                    {   // -- logout - déconnexion pas d'autorized dfe l'utilisateur
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                    else
                    { // -- problème autre 
                        _toastService?.AvertissementAsync("Impossible de supprimer l'utilisateur. Problème interne.");
                    }
                    
                    return;
                }
                _toastService?.AvertissementAsync("Impossible de supprimer l'utilisateur. Vérifier que l'utilisateur existe.");
            }
            catch(Exception ex)
            {
                _toastService?.ErreurAsync($"Erreur lors de la suppression de l'utilisateur : {ex.Message}");
            }
           
        }

        /// <summary>
        /// Save USer
        /// </summary>
        /// <param name="user"></param>
        protected async Task OnSaveUser(ProfilDTOAdmin? user)
        {
            try
            {
                if ( _authenticationStateProvider is not null && user is not null )
                {
                    // -- récupération de l'utilisateur connecté pour récupérer les groupes de localisation
                    var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                    // -- récupération du claim role de l'utilisateur
                    var roles = authState.User.Identities.FirstOrDefault()?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                    var idUser = authState.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    // -- vérification si l'utilisateur est un manager il ne peut pas modifier le rôle d'un admin ou l'ajouter ou l'enlever
                    if ( roles is not null && user.Roles is not null 
                        && roles.Contains("Manager") && user.Roles.Contains("Admin"))
                    {
                        _toastService?.AvertissementAsync("Vous n'avez pas les droits pour ajouter le role administrateur.");
                        return;
                    }

                    // -- vérification si la personne s'enlève les droits bloqués
                    if( idUser is not null && user.Id is not null
                        && idUser.Value == user.Id)
                    {
                        user.Roles?.Add("Manager");
                        DesabledSaveUser[user.Id] = true;
                        _toastService?.AvertissementAsync("Vous ne pouvez pas changer vos attributions vous même.");
                        return;
                    }

                }

                if (user is not null && _profils.Contains(user) && _mediator is not null )
                {

                    var result = await _mediator.Send(new UpdateUserRoleCommand { Profil = user });
                    var index = _profils.IndexOf(user);
                    if (result.IsSuccess)
                    { // -- suppression réussie supression dans la liste aussi
                        _profils[index] = user;
                        DesabledSaveUser[user.Id ?? string.Empty] = true;
                        _toastService?.SuccesAsync("Modification réussie.");
                    }
                    else if (result.IsUnauthorized)
                    {   // -- logout - déconnexion pas d'autorized dfe l'utilisateur
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                    else
                    { // -- problème autre
                        _toastService?.AvertissementAsync("Impossible de modifier le role de l'utilisateur. Vous n'avez pas les droits.");
                       StateHasChanged();
                    }

                    return;
                }

                _toastService?.AvertissementAsync("Impossible de modifier le role de l'utilisateur. Problème interne.");
            }
            catch(Exception ex)
            {
                _toastService?.ErreurAsync($"Erreur lors de la sauvegarde de l'utilisateur : {ex.Message}");
            }
        }

        /// <summary>
        /// method qui modifie les roles d'un utilisateur 
        /// et active le bouton de sauvegarde pour cet utilisateur
        /// 1 -> Admin
        /// 2 -> Manager
        /// </summary>
        /// <param name="user"></param>
        protected void OnUserRolesChanged (ProfilDTOAdmin? user, bool ischecked, int role)
        {
            if (user is not null && _profils.Contains(user))
            {
                var index = _profils.IndexOf(user);
                // -- si le role est coché, on l'ajoute à la liste des roles de l'utilisateur
                if (ischecked)
                {
                    user.Roles?.Add(role == 1 ? "Admin" : "Manager");
                    // -- évite d'avoir Manager et admin en même temps
                    // -- c'est soit l'un ou l'autre
                    user.Roles?.Remove(role == 1 ? "Manager" : "Admin");
                }
                else
                { //-- supprimer le role de l'utilisateur si il est décoché
                    user.Roles?.Remove(role == 1 ? "Admin" : "Manager");
                }
                // -- modification de l'utilisateur dans la liste des profils
                _profils[index] = user;
                // -- Activer le bouton de sauvegarde pour cet utilisateur en utilisant le dictionnaire DesabledSaveUser
                if (user.Id is not null && DesabledSaveUser.ContainsKey(user.Id))
                {   // -- Activer le bouton de sauvegarde pour cet utilisateur
                    DesabledSaveUser[user.Id] = false;
                }
            }
        }

        /// <summary>
        /// Fait une recherche par rapport au login des utilisateurs
        /// </summary>
        protected void FilterSearch()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                // -- filtrer la liste des utilisateurs en fonction du login
                _filterProfils = _profils.Where(u => u.Login?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }
            else
            {
                InitialiserUserList();
            }
        }
        #endregion

        #region protected method Pagination
        protected void ChangePage(int page)
        {
            if (page >= 1 && page <= TotalPages)
            {
                PageActuel = page;
            }
        }

        protected void PreviousPage()
        {
            if (PageActuel > 1)
            {
                PageActuel--;
            }
        }

        protected void NextPage()
        {
            if (PageActuel < TotalPages)
            {
                PageActuel++;
            }
        }
        #endregion

        #region private method
        /// <summary>
        /// method d'initialisation de la liste des user
        /// </summary>
        /// <returns></returns>
        private async Task InitListUsers()
        {
            if (_mediator is not null)
            {
                var result = await _mediator.Send(new GetAllUsersCommand());
                if (result.IsSuccess && result.Profils is not null)
                {
                    _profils.AddRange(result.Profils);
                    //AjoutTestUser();
                    InitDesabledSaveUser();
                    InitialiserUserList();
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

        /// <summary>
        /// init le dictionnaire pour desabled le bouton de sauvegarde pour chaque utilisateur
        /// </summary>
        private void InitDesabledSaveUser()
        {
            foreach (var item in _profils)
            {
                DesabledSaveUser.Add(item.Id ?? string.Empty, true);
            }
        }

        // -- initalisation de la liste des utilisateurs filtrés
        private void InitialiserUserList()
        {
            _filterProfils = _profils;
        }

        /*private void AjoutTestUser()
        {
            for(int i = 0; i<= 50; i++)
            {
                ProfilDTOAdmin profil = new() { 
                    Email = $"MagalieCalmon{i}@sfr.fr", 
                    Id = $"212123dsdq3{i}1dq3d1q", 
                    Login = $"MagalieCalmon{i}" 
                };
                _profils.Add(profil);
            }

        }*/
        #endregion
    }
}
