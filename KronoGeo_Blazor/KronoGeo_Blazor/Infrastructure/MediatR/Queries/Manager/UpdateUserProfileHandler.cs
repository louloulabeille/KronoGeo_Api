using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Manager
{
    public class UpdateUserProfileHandler(IServiceHttpManagerKronoGeo httpKronoGeo
        , ILogger<UpdateUserProfileHandler> logger)
        : ManagerHandler<UpdateUserProfileHandler>(httpKronoGeo, logger)
        , IRequestHandler<UpdateUserProfileCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.UpdateProfilAsync(request.profil);
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
