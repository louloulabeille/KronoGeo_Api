using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.ApplicationModel.Communication;

namespace KronoGeo_Blazor.Client.Pages.Account
{
    public class ProfileBase : ComponentBase
    {
        #region protected properties
        protected ProfilDTO Profil { get; set; } = new();
        #endregion

        #region inject properties

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
        private async Task InitProfilAsync ()
        {
            Profil.Login = "louloulabeille";
            Profil.Email = "louloulabeille@proton.me";   
        }
        #endregion
    }
}
