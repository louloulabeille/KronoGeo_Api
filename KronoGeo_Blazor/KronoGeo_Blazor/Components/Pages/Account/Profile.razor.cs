using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Components.Pages.Account
{
    public class ProfileBase : ComponentBase
    {
        #region protected properties
        protected ProfilDTO Profil { get; set; } = new();
        #endregion

        #region inject properties
        [Inject]
        private IMediator? _mediator { get; set; } = default;
        /// <summary>
        /// Service pour l'affichage des notifications toast
        /// </summary>
        [Inject]
        private ToastsService? _toastService { get; set; } = default;
        #endregion

        #region override method
        protected override async Task OnInitializedAsync()
        {
            await InitProfilAsync();
            await base.OnInitializedAsync();
        }

        #endregion

        #region public method
        public async Task ValidProfil()
        {

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
                        Profil = result.Profil;
                    }
                }
            }
            catch
            {
                _toastService?.ErreurAsync("Un problème interne a été remonté.");
            }
        }
        #endregion
    }
}
