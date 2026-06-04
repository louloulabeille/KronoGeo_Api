using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Models.Infrastructure.Email;
using MailKit;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class ServiceMailExtends
    {
        extension (IServiceCollection services)
        {
            public IServiceCollection AddServiceMail(IConfiguration config)
            {
                // - ajout dans le services Ioptions de CourrielOptions en injection de dépendance
                // pour pouvoir l'utiliser dans les controllers ou autres services
                services.AddOptions<CourrielOptions>().Bind(config.GetSection("Courriel"));

                services.AddScoped<IMailService, MailService>();
                return services;
            } 
        }

    }
}
