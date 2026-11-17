using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class GetPinRegionBoundsHandler(IServiceHttpKronoGeo serviceHttp
        , ILogger<GetPinRegionBoundsHandler> logger)
        : GpsHandler<GetPinRegionBoundsHandler>(serviceHttp, logger)
        , IRequestHandler<GetPinRegionBoundsCommand, ResponseApiPinRegionBounds>
    {
        public async Task<ResponseApiPinRegionBounds> Handle(GetPinRegionBoundsCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is not null)
            {
                // -- requete vers l'APi
                var result = await ServiceHttp.GetPinByRegionBounds(request.Region);

                if (result is not null)
                {
                    return result;
                }
            }

            Logger.LogError("Erreur lors de la récupération des pins selon la region de recherche.");
            return new ()
            {
                ApiStatus = EnumApiStatus.BadRequest,
                Message = $"Erreur lors de la récupération des localisations pour coordonnées min {request.Region.MinBounds.ToString()} & " +
                $"coordonnées max {request.Region.MaxBounds.ToString()}."
            };
        }
    }
}
