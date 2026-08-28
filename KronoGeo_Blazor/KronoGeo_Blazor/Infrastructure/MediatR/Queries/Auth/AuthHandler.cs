using KronoGeo_Api.Interface.Service;
using KronoGeo_Blazor.Components.Api;
using Microsoft.Extensions.Caching.Memory;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Auth
{
    public class AuthHandler<T>(IServiceHttpKronoGeo httpKronoGeo
        , IMemoryCache memoryCache, ILogger<T> logger ) where T : class
    {
        #region private readonly properties
        internal readonly IServiceHttpKronoGeo HttpKronoGeo = httpKronoGeo;
        internal readonly IMemoryCache MemoryCache = memoryCache;
        internal readonly ILogger<T> Logger = logger;
        #endregion
    }
}
