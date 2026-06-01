namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class MediatRExtends
    {
        extension (IServiceCollection services )
        {
            /// <summary>
            /// method extension pour mettre en place du design patten MediatR
            /// Pour dissocier la demande et la réponse 
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddMediaTRExtend(IConfiguration configuration)
            {
                // - récupération de la configuration de MediatR
                var mediatRConfig = configuration.GetSection("MediatR");
                string licence = mediatRConfig.GetValue<string>("key") ?? string.Empty;

                // - lancement du service
                services.AddMediatR(cfg => {
                    cfg.LicenseKey = licence;
                    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                });
                return services;
            }
        }
    }
}
