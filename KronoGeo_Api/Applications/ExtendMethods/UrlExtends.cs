using KronoGeo_Api.Models.Infrastructure.Email;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class UrlExtends
    {
        extension(IServiceCollection service)
        {
            public IServiceCollection AddOptionsUrl(IConfiguration config)
            {
                // - ajout de la configuration des url 
                service.AddOptions<UrlOptions>().Bind(config.GetSection("Url"));
                return service;
            }
        }
    }
}
