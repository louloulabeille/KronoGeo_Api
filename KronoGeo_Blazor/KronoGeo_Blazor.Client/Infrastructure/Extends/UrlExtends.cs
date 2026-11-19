using KronoGeo_Api.Models.Infrastructure.Options;

namespace KronoGeo_Blazor.Client.Infrastructure.Extends
{
    public static class UrlExtends
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddIoptionsUrlImages(IConfiguration configuration)
            {
                services.AddOptions();
                services.Configure<UrlImage>(configuration.GetSection("UrlImage"));
                return services;
            }
        }
    }
}
