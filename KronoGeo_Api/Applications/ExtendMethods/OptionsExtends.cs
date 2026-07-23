using KronoGeo_Api.Models.Infrastructure.Email;
using KronoGeo_Api.Models.Infrastructure.Options;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class OptionsExtends
    {
        extension ( IServiceCollection services)
        {
            public IServiceCollection AddIOptions( IConfiguration config )
            {
                services.AddOptions<PhotoOptions>().Bind(config.GetSection("PhotoOptions"));
                return services;
            }
        }
    }
}
