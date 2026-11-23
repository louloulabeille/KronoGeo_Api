namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class CorsExtends
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCorsdAPI (IConfiguration config)
            {

                var allowedOrigins = config.GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

                // -- configuration Cors autorisation pour se connecter à API à partir d'un navigateur
                services.AddCors(options =>
                {
                    options.AddPolicy("BlazorPolicy", policy =>
                    {
                        policy.WithOrigins(allowedOrigins)  // -- adresse des sites
                              .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                              .WithHeaders("Content-Type", "Authorization")
                              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                    });
                });

                return services;
            }
        }
    }
}
