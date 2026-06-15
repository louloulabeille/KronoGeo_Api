using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Identity;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class RegisterIdentity
    {
        #region properties
        public required RegisterDTO Register { get; set; }
        public IdentityResult? IdentityResult { get; set; } = null;
        public SignInResult? SignInResult { get; set; } = null;
        #endregion
    }
}
