using KronoGeo_Api.Interface.Service;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Images
{
    public class ImagesHandler<T>(IServiceHttpKronoGeo serviceHttp
        , ILogger<T> logger) where T : class
    {
        #region internal properties 
        internal readonly IServiceHttpKronoGeo ServiceHttp = serviceHttp;
        internal readonly ILogger<T> Logger = logger;
        #endregion

    }
}
