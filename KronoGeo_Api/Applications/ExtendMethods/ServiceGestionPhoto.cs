using KronoGeo_Api.Infrastructure.Services.DirectoryPhoto;
using KronoGeo_Api.Interface;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class ServiceGestionPhoto
    {
        extension (IServiceCollection services)
        {
            public IServiceCollection AddServiceGestionPhoto ()
            {
                services.AddSingleton<IServiceGestionPhoto, FilePhoto>();
                return services;
            }
        }
    }
}
