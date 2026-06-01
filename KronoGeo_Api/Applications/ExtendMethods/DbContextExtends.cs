using KronoGeo_Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class DbContextExtends
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// method extension pour mettre du dbcontext 
            /// et la configuration de la connexion à la base de données
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddDbContextSecretExtend(IConfiguration config)
            {
                // - récupération de la ligne de connexion vers la base de données -- stocker dans les données secrètes
                string? stringConnection = config.GetConnectionString("DefaultConnection") ??
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

                // - appel du DbContext pour la connexion vers la base
                services.AddDbContext<KronoGeoDbContext>(options =>
                    options.UseNpgsql(stringConnection));

                return services;
            }
        }
    }
}
