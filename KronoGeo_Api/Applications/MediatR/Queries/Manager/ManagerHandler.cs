using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog.Core;

namespace KronoGeo_Api.Applications.MediatR.Queries.Manager
{
    public class ManagerHandler (ILogger<object> logger
        , SignInManager<ApplicationUser> signInManager
        , IOptions<KeyBearer> keyBearer)
    {
        #region private properties
        internal readonly ILogger<object> Logger = logger;
        internal readonly SignInManager<ApplicationUser> SignInManager = signInManager;
        internal IOptions<KeyBearer> KeyBearer = keyBearer;
        #endregion
    }
}
