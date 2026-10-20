using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class GetProfileHandler
        (ILogger<GetProfileHandler> logger, IServiceHttpManagerKronoGeo httpKronogeo)
        : ManagerHandler<GetProfileHandler>(httpKronogeo, logger)
        , IRequestHandler<GetProfileCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(GetProfileCommand request, CancellationToken cancellationToken)
        {
            
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.GetProfileAsync();
                return result;
            }

            return new()
            {
                ApiStatus   = EnumApiStatus.Problem,
                Message     = "Problème de connexion vers le serveur."
            };

        }
    }
}
