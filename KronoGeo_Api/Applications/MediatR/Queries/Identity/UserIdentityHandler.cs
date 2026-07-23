using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class UserIdentityHandler (ILogger<object> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<ApplicationUser> signInManager  )
    {
        #region private properties
        internal readonly ILogger<object> _logger = logger;
        internal readonly IOptions<KeyBearer> _keyBearer = keyBearer;
        internal readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        #endregion
    }
}
