using System.ComponentModel;

namespace KronoGeo_Blazor.Infrastructure.Extends
{
    public static class MediatRExtend
    {
        extension ( IServiceCollection services )
        {
            public IServiceCollection AddServiceMediatR()
            {
                // - lancement du service au niveau du programm
                services.AddMediatR(cfg => {
                    // cfg.LicenseKey = licence; // -- pas de licence pour blazor pas grave
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                });

                return services;
            }
        }
    }
}
