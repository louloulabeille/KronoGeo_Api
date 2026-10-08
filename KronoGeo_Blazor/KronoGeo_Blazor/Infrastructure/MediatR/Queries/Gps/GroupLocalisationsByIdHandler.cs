using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class GroupLocalisationsByIdHandler(IServiceHttpKronoGeo serviceHttp
        , ILogger<GroupLocalisationsByIdHandler> logger)
        : GpsHandler<GroupLocalisationsByIdHandler>(serviceHttp, logger)
        , IRequestHandler<GroupLocalisationsByIdCommand, ResponseApiLocalisations>
    {
        public async Task<ResponseApiLocalisations> Handle(GroupLocalisationsByIdCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is not null)
            {
                // -- requete vers l'APi
                var result = await ServiceHttp.GetLocalisationsByIdAsync(request.Id);

                if (result is not null)
                {
                    return result;
                }
            }

            Logger.LogError("Erreur lors de la récupération des groupes de localisation pour l'utilisateur");
            return new ResponseApiLocalisations
            {
                ApiStatus = EnumApiStatus.BadRequest,
                Message = $"Erreur lors de la récupération des localisations pour id {request.Id}."
            };
        }
    }
}
