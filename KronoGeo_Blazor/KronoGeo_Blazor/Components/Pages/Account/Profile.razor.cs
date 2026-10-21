using KronoGeo_Api.Models.Infrastructure.Cookie;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                _toastService?.InfoAsync("Mot de passe modifié.");
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
                        { profil = profilUpdate });

                    if( result.IsSuccess )
                    {
                        if (_toastService is not null)
                            await _toastService.InfoAsync("Enregistrement.");

                        // Dans le composant, après l'appel API réussi
                        // -- appel vers le endpoint de refresh du cookie
                        // -- enregistrement des datas dans le cache avec IMemory ou sinon plusieurs instance de serveur
                        // -- utiliser IDistributedCache
                        var key = Guid.NewGuid().ToString("N"); // -- clé d'instance
                        // -- enregistrement dans la cache
                        var data = new RefreshCookieData(result.Token ?? string.Empty
                            , result.Profil?.Id ?? string.Empty, result.Profil?.Login ?? string.Empty);
                        _cache?.Set(key, data, TimeSpan.FromSeconds(30));
                        // -- test data is null
                        //_cache?.Set(string.Empty, data, TimeSpan.FromSeconds(30));
                        _navigationManager?.NavigateTo($"/Account/refresh?key={key}&returnUrl=/Account/profile", forceLoad: true);

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
                }
            }
            catch(Exception ex)
            {
                if (_logger is not null) _logger.LogError(ex, "Erreur au niveau de la remontée du profil {message}",ex.Message);
                _toastService?.ErreurAsync("Un problème interne a été remonté.");
            }
        }
        #endregion
    }
}
