using KronoGeo_Api.Infrastructure.Repository;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Models;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class RepositoryExtends
    {
        extension (IServiceCollection services)
        {
            /// <summary>
            /// ajout des repositories dans le conteneur d'injection de dépendances
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddRepository ()
            {
                services.AddScoped<IRepository<LocalisationGroup>, Repository<LocalisationGroup>>();
                services.AddScoped<IRepository<Localisation>, Repository<Localisation>>();
                services.AddScoped<IRepository<LocalisationPhoto>, Repository<LocalisationPhoto>>();
                return services;
            }
        }
    }
}
