using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using KronoGeo_Blazor.Infrastructure.MediatR.Queries.Auth;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class GroupLocationUserHandler(IServiceHttpKronoGeo serviceHttp
        , ILogger<GroupLocationUserHandler> logger) : GpsHandler<GroupLocationUserHandler>(serviceHttp, logger)
        , IRequestHandler<GroupLocationUserCommand, ResponseApiLocalisations>
    {
        public async Task<ResponseApiLocalisations> Handle(GroupLocationUserCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is not null)
            {
                // -- requete vers l'APi
                var result = await ServiceHttp.GetUserGroupLocalisationAsync(request.UserId);

                if (result is not null)
                {
                    return result;
                }
            }

            Logger.LogError("Erreur lors de la récupération des groupes de localisation pour l'utilisateur {UserId}", request.UserId);
            return new ResponseApiLocalisations
            {
                ApiStatus = EnumApiStatus.BadRequest,
                Message = "Erreur lors de la récupération des groupes de localisation."
            };
        }
    }
}
