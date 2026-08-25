using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Components.Account.Pages
{
    public class AuthenticateBase : ComponentBase
    {

        #region protected method 
        protected RegisterDTO Login { get; set; }
        #endregion

        #region constructeur
        public AuthenticateBase()
        {
            // -- initialisation du Login
            Login = new() { Login = string.Empty };
        }
        #endregion

        #region protected method
        protected void FormAuthenticate()
        {

        }
        #endregion
    }
}
