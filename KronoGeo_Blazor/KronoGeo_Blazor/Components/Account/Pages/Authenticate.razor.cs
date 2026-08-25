using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Components.Account.Pages
{
    public class AuthenticateBase : ComponentBase
    {
        #region private readonly properties 
        [Inject]
        private IServiceHttpKronoGeo? _serviceHttp { get; set; } = default;
        #endregion

        #region protected method 
        protected RegisterDTO Login { get; set; } = new() { Login = string.Empty };
        #endregion

        #region override method
        protected override void OnInitialized()
        {
            base.OnInitialized();

        }

        #endregion

        #region protected method
        protected async Task FormAuthenticate()
        {
            if ( _serviceHttp is not null ) 
            { 

                var result = await _serviceHttp.AuthenticateAsync(Login);
            }
        }
        #endregion
    }
}
