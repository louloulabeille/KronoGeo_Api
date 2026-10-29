using KronoGeo_Api.Models.Infrastructure.Cookie;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Email;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace KronoGeo_Blazor.Components.Pages.Account
{
    public class ProfileBase : ComponentBase
    {
        #region protected properties
        // -- profilDTO de modification celui affiché 
        protected ProfilUpdateDTO Profil { get; set; } = new();
        // -- profilDTO par défaut remonter de l'API
        protected ProfilDTO? ProfilDTO { get; set; } = default;
        protected ProfilMpDTO ProfilMpDTO { get; set; } = new();
        protected string TypePassword1 { get; set; } = _passwdNoView;
        protected string TypePassword2 { get; set; } = _passwdNoView;
        protected string TypePasswordOld { get; set; } = _passwdNoView;
        protected string ClassIPassword1 { get; set; } = _iconPasswdNoView;
        protected string ClassIPassword2 { get; set; } = _iconPasswdNoView;
        protected string ClassIPasswordOld { get; set; } = _iconPasswdNoView;
        #endregion

        #region private const properties
        private const string _passwdNoView = "password";
        private const string _passwdView = "text";
        private const string _iconPasswdNoView = "input-group-text bi-eye";
        private const string _iconPasswdView = "input-group-text bi-eye-slash";
        #endregion

        #region inject properties
        [Inject]
        private IMediator? _mediator { get; set; } = default;
        /// <summary>
        /// Service pour l'affichage des notifications toast
        /// </summary>
        [Inject]
        private ToastsService? _toastService { get; set; } = default;
        [Inject]
        private ILogger<ProfileBase>? _logger { get; set; } = default;
        [Inject]
        private NavigationManager? _navigationManager { get; set; } = default;
        [Inject]
        private IMemoryCache? _cache { get; set; } = default;
        #endregion

        #region CascadingParameter properties
        /*[CascadingParameter]
        private Task<AuthenticationState>? AuthState { get; set; }*/
        #endregion

        #region override method
        protected override async Task OnInitializedAsync()
        {
            await InitProfilAsync();
            await base.OnInitializedAsync();
        }

        #endregion

        #region public method
        /// <summary>
        /// Modifie le mail
        /// </summary>
        /// <returns></returns>
        public async Task UpdateMail()
        {
            try
            {
                // -- différence de mail 
                if( Profil?.Email?.Trim() != ProfilDTO?.Email && _mediator is not null)
                {
                    var register = new ProfilUpdateEmailDTO
                    {
                        Id = ProfilDTO?.Id,
                        Login = Profil?.Login ?? string.Empty,
                        Email = Profil?.Email?.Trim(),
                    };

                    var result = await _mediator.Send(new UpdateUserEmailCommand { RegisterDTO = register });

                    if (result.IsSuccess)
                    {
                        if (_toastService is not null)
                            await _toastService.InfoAsync("Enregistrement du nouveau email.\n" +
                                "Un email a été envoyé vers votre nouveau adresse, il faut valider le lien pour mettre en place la modification." +
                                "Puis lors de la la validation, un email sera envoyé vers l'ancienne adresse pour la récupération valable 72h.\n" +
                                "Après le changement, veuillez vous déconnecter.");
                    }
                    else if (result.IsUnauthorized)
                    {   // -- logout - déconnexion
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                    else
                    {
                        Profil?.Email = ProfilDTO?.Email;

                        if (_toastService is not null)
                            await _toastService.ErreurAsync($"Erreur lors de l'enregistrement. \n{result.Message ?? "Erreur lors de l'enregistrement du nouveau email."}");
                    }


                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception dans UpdateMail dans Profil.razor, {message}", ex.Message);
                if (_toastService is not null)
                    await _toastService.ErreurAsync("Erreur lors de l'enregistrement de la nouvelle adresse mail.");

            }
        }

        /// <summary>
        /// modification du mot de passe
        /// </summary>
        /// <returns></returns>
        public async Task UpdateMotdePasse()
        {
            try
            {
                if (!string.IsNullOrEmpty(ProfilMpDTO.PasswordOld.Trim())
                     && !string.IsNullOrEmpty(ProfilMpDTO.Password.Trim())
                     && _mediator is not null)
                {

                    /*var user = (await AuthState!).User;
                    var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;*/

                    var changeMP = new ProfilDTO
                    {
                        Id = ProfilDTO?.Id,
                        Login = ProfilDTO?.Login,
                        Password = ProfilMpDTO.PasswordOld.Trim(),
                        NewPassword = ProfilMpDTO.Password.Trim()
                    };

                    var result = await _mediator.Send(new UpdatePasswordCommand { Profil = changeMP });

                    if (result.IsSuccess)
                    {
                        if (_toastService is not null)
                            await _toastService.InfoAsync("Modification effectuée.");

                        var data = new RefreshCookieData( result.Token ?? string.Empty
                            , result?.Profil?.Id ?? string.Empty
                            , result?.Profil?.Login ?? string.Empty);

                        RefreshCookieOnly(data);
                    }
                    else if (result.IsUnauthorized)
                    {
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                    else
                    {
                        if (_toastService is not null)
                            await _toastService.ErreurAsync($"Erreur lors de la modification. \n{result.Message ?? "Erreur lors de l'enregistrement du nouveau mot de passe."}");
                    }
                }
                
                
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex, "Exception dans UpdateMotdePasse dans Profil.razor, {message}", ex.Message);
                if (_toastService is not null)
                    await _toastService.ErreurAsync("Erreur lors de l'enregistrement du nouveau mot de passe.");

            }
        }

        /// <summary>
        /// modifie le login ou le numéro de téléphone
        /// </summary>
        /// <returns></returns>
        public async Task ValidProfil()
        {
            try
            {
                if (_mediator is not null && ProfilDTO is not null && ( Profil.Login?.Trim() != ProfilDTO?.Login
                || Profil.PhoneNumber?.Trim() != ProfilDTO?.PhoneNumber) )
                {
                    var profilUpdate = new ProfilDTO()
                    {
                        Id = ProfilDTO?.Id,
                        Login = Profil.Login?.Trim(),
                        PhoneNumber = Profil.PhoneNumber?.Trim(),
                        Email = ProfilDTO?.Email,
                        NewPassword = ProfilDTO?.NewPassword,
                        Password = ProfilDTO?.Password,
                        Roles = ProfilDTO?.Roles
                    };
                    
                    var result = await _mediator.Send(new UpdateUserProfileCommand() 
                        { Profil = profilUpdate });

                    if( result.IsSuccess )
                    {
                        if (_toastService is not null)
                            await _toastService.InfoAsync("Enregistrement.");

                        
                        // -- enregistrement dans la cache
                        var data = new RefreshCookieData(result.Token ?? string.Empty
                            , result.Profil?.Id ?? string.Empty, result.Profil?.Login ?? string.Empty);
                        RefreshCookieOnly(data);

                    }
                    else if ( result.IsUnauthorized )
                    {   // -- logout - déconnexion
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                    else
                    {

                        Profil.Login = ProfilDTO?.Login;
                        Profil.PhoneNumber = ProfilDTO?.PhoneNumber;

                        if (_toastService is not null)
                            await _toastService.ErreurAsync($"Erreur lors de l'enregistrement. \n{result.Message ?? "Erreur lors de l'enregistrement du profil."}");
                    }
                }
            }
            catch(Exception ex)
            {
                if (_toastService is not null)
                    await _toastService.ErreurAsync("Erreur interne retournée.");
                _logger?.LogError(ex, "Exception dans ValidProfil dans Profil.razor, {message}", ex.Message);
            }
            
        }

        /// <summary>
        /// méthod qui affiche ou non le mot de passe
        /// </summary>
        /// <param name="i"></param>
        protected void OnChangeAffichePassword(int i)
        {
            switch(i)
            {
                case 1: // -- pour le premier mot de passe
                    TypePassword1 = TypePassword1 == _passwdNoView ? _passwdView : _passwdNoView;
                    ClassIPassword1 = TypePassword1 == _passwdNoView ? _iconPasswdNoView : _iconPasswdView;
                    break;
                case 2: // -- pour le 2ième mot de passe
                    TypePassword2 = TypePassword2 == _passwdNoView ? _passwdView : _passwdNoView;
                    ClassIPassword2 = TypePassword2 == _passwdNoView ? _iconPasswdNoView : _iconPasswdView;
                    break;
                case 3:
                    TypePasswordOld = TypePasswordOld == _passwdNoView ? _passwdView : _passwdNoView;
                    ClassIPasswordOld = TypePasswordOld == _passwdNoView ? _iconPasswdNoView : _iconPasswdView;
                    break;
                default: // -- par défaut on affiche les 2 ou pas en même temps
                    TypePassword1 = TypePassword1 == _passwdNoView ? _passwdView : _passwdNoView;
                    ClassIPassword1 = TypePassword1 == _passwdNoView ? _iconPasswdNoView : _iconPasswdView;
                    TypePassword2 = TypePassword2 == _passwdNoView ? _passwdView : _passwdNoView;
                    ClassIPassword2 = TypePassword2 == _passwdNoView ? _iconPasswdNoView : _iconPasswdView;
                    TypePasswordOld = TypePasswordOld == _passwdNoView ? _passwdView : _passwdNoView;
                    ClassIPasswordOld = TypePasswordOld == _passwdNoView ? _iconPasswdNoView : _iconPasswdView;
                    break;
            }
            StateHasChanged();
        }
        #endregion

        #region private method
        /// <summary>
        /// init le profil de l'utilisateur
        /// </summary>
        /// <returns></returns>
        private async Task InitProfilAsync()
        {
            try
            {
                if (_mediator is not null)
                {
                    var result = await _mediator.Send(new GetProfileCommand());
                    if (result.IsSuccess && result.Profil is not null)
                    {
                        ProfilDTO = result.Profil;

                        Profil = new() {
                            Login = result.Profil.Login,
                            Email = result.Profil.Email,
                            PhoneNumber = result.Profil.PhoneNumber
                        };
                    }
                    else if (result.IsUnauthorized)
                    {   // -- logout - déconnexion
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                }
            }
            catch(Exception ex)
            {
                if (_logger is not null) _logger.LogError(ex, "Erreur au niveau de la remontée du profil {message}",ex.Message);
                _toastService?.ErreurAsync("Un problème interne a été remonté.");
            }
        }

        /// <summary>
        /// method qui envoie les datas pour le rafraichissement du cookie
        /// vers le serveur avec un ImemoryCache et envoie la requete avec un lien
        /// vers un endPoint RefreshCookie 
        /// </summary>
        /// <param name="cookieData"></param>
        private void RefreshCookieOnly(RefreshCookieData cookieData)
        {
            // Dans le composant, après l'appel API réussi
            // -- appel vers le endpoint de refresh du cookie
            // -- enregistrement des datas dans le cache avec IMemory ou sinon plusieurs instance de serveur
            // -- utiliser IDistributedCache
            var key = Guid.NewGuid().ToString("N"); // -- clé d'instance
            // -- envoie de la data vers le serveur
            _cache?.Set(key, cookieData, TimeSpan.FromSeconds(30));
            
            // -- requete vers le endpoint pou rafraichir le cookie
            _navigationManager?.NavigateTo($"/Account/refresh?key={key}&returnUrl=/Account/profile", forceLoad: true);
        }
        #endregion
    }
}
