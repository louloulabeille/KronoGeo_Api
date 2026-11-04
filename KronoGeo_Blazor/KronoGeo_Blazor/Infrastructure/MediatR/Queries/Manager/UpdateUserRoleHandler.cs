using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class UpdateUserRoleHandler(IServiceHttpManagerKronoGeo serviceHttp
        , ILogger<UpdateUserRoleHandler> logger)
        : ManagerHandler<UpdateUserRoleHandler>(serviceHttp, logger)
        , IRequestHandler<UpdateUserRoleCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.UpdateUserRoleAsync(request.Profil);
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
