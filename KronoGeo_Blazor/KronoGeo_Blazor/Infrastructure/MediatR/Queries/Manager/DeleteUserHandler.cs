using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class DeleteUserHandler(IServiceHttpManagerKronoGeo serviceHttp
        , ILogger<DeleteUserHandler> logger)
        : ManagerHandler<DeleteUserHandler>(serviceHttp, logger)
        , IRequestHandler<DeleteUserCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.DeleteUserAsync(request.Id);
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
