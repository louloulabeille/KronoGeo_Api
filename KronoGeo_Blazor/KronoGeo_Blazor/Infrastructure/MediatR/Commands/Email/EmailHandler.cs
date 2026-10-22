using KronoGeo_Api.Interface.Service;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Email
{
    public class EmailHandler<T>(ILogger<T> logger
        , IServiceHttpManagerKronoGeo serviceHttp) where T : class
    {
        internal ILogger<T> Logger { get; set; } = logger;
        internal IServiceHttpManagerKronoGeo HttpKronoGeo { get; set; } = serviceHttp;
    }
}
