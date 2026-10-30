using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class GetAllUsersHandler (IServiceHttpManagerKronoGeo serviceHttp
        , ILogger<GetAllUsersHandler> logger) 
        : ManagerHandler<GetAllUsersHandler>(serviceHttp, logger)
        , IRequestHandler<GetAllUsersCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
        {
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.GetListUsersAsync();
                return result;
            }

            return new()
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = "Problème de connexion vers le serveur."
            };
        }
    }
}
