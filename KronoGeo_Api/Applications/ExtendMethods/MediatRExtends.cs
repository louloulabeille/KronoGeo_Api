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
            public IServiceCollection AddMediaTRExtend()
            {
                //services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
                return services;
            }
        }
    }
}
