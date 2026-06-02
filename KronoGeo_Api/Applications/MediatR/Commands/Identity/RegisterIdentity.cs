using KronoGeo_Api.Applications.Model.DTO;
using Microsoft.AspNetCore.Identity;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class RegisterIdentity
    {
        #region properties
        public required RegisterDTO Register { get; set; }
        public IdentityResult? Result { get; set; } = null;
        public SignInResult? SignInResult { get; set; } = null;
        #endregion
    }
}
