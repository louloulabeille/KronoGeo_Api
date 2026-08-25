using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Components.Account.Pages
{
    public class AuthenticateBase : ComponentBase
    {
        #region private readonly properties 
        [Inject]
        private IServiceHttpKronoGeo _serviceHttp { get; set; }
        #endregion

        #region protected method 
        protected RegisterDTO Login { get; set; }
        #endregion

        #region constructeur
        public AuthenticateBase(IServiceHttpKronoGeo serviceHttp)
        {
            // -- service
            _serviceHttp = serviceHttp;

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
