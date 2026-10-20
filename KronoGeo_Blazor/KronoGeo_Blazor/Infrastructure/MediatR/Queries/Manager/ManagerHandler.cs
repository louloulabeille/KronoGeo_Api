using KronoGeo_Api.Interface.Service;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class ManagerHandler<T>(IServiceHttpManagerKronoGeo httpKronoGeo
        , ILogger<T> logger) where T : class
    {
        #region private readonly properties
        internal readonly IServiceHttpManagerKronoGeo HttpKronoGeo = httpKronoGeo;
        internal readonly ILogger<T> Logger = logger;
        #endregion
    }
}
