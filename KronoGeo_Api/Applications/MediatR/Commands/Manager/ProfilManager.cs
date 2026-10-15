using KronoGeo_Api.Models.Model.DTO;
using Microsoft.AspNetCore.Identity;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class ProfilManager
    {
        #region properties
        public required ProfilDTO Profil { get; set; }
        public IdentityResult? IdentityResult { get; set; } = null;
        public SignInResult? SignInResult { get; set; } = null;
        #endregion
    }
}
