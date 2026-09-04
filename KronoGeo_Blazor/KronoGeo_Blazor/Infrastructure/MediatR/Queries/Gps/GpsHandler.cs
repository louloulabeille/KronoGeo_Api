using KronoGeo_Api.Interface.Service;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class GpsHandler <T> (IServiceHttpKronoGeo serviceHttp
        , ILogger<T> logger ) where T : class 
    {
        #region protected readonly properties
        protected readonly IServiceHttpKronoGeo ServiceHttp = serviceHttp;
        protected readonly ILogger<T> Logger = logger;
        #endregion
    }
}
