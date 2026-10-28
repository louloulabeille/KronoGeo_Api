using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class UpdatePasswordHandler(IServiceHttpManagerKronoGeo serviceHttp
        , ILogger<UpdatePasswordHandler> logger)
        : ManagerHandler<UpdatePasswordHandler>(serviceHttp, logger)
        , IRequestHandler<UpdatePasswordCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.UpdatePasswordAsync(request.Profil);
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
