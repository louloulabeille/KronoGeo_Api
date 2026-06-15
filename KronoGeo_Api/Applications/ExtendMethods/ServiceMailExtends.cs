using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using MailKit;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class ServiceMailExtends
    {
        extension (IServiceCollection services)
        {
            public IServiceCollection AddServiceMessage(IConfiguration config)
            {
                // - ajout dans le services Ioptions de CourrielOptions en injection de dépendance
                // pour pouvoir l'utiliser dans les controllers ou autres services
                services.AddOptions<CourrielOptions>().Bind(config.GetSection("Courriel"));

                services.AddScoped<IServiceSendMessage, ServiceSmtp>();
                return services;
            } 
        }

    }
}
