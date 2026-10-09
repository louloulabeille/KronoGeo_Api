using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class UpdateLocalisationGroupHandler(ILogger<UpdateLocalisationGroupHandler> logger
        , KronoGeoDbContext context)
        : RepositoryHandler(logger, context)
        , IRequestHandler<UpdateLocalisationGroupCommand, ResponseApiLocalisations>
    {
        public async Task<ResponseApiLocalisations> Handle(UpdateLocalisationGroupCommand request, CancellationToken cancellationToken)
        {
            var localisationgroup = new LocalisationGroup()
            {
                Id = request.localisationGroup.Id,
                Name = request.localisationGroup.Name,
                Date = request.localisationGroup.Date,
                ApplicationUserId = request.localisationGroup.ApplicationUserId,
                RouteTelemetry = request.localisationGroup.RouteTelemetry?.Get(),
                Localisations = request.localisationGroup.Localisations?.Select( l => l.Get()).ToList()
            };

            if (localisationgroup is not null && _unitOfWork is not null)
            {
                try
                {
                    _unitOfWork.Repository<LocalisationGroup>().Update(localisationgroup);
                    if ( _unitOfWork.SaveChanges() > 0 )
                    {
                        return new ResponseApiLocalisations
                        {
                            ApiStatus   = EnumApiStatus.Success,
                            Message     = string.Empty,
                            LocalisationGroupDTO = request.localisationGroup
                        };
                    }
                    _logger.LogError("Erreur lors de la sauvegarde de localisation group {id} ", localisationgroup.Id);
                    return new ResponseApiLocalisations { ApiStatus = EnumApiStatus.BadRequest, Message = "Erreur lors de la sauvegarde" };
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la sauvegarde de localisation group {id}", localisationgroup.Id);
                    return new ResponseApiLocalisations { ApiStatus = EnumApiStatus.Problem, Message = "Erreur lors de la sauvegarde" };
                }
            }
            _logger.LogError("Erreur lors de la sauvegarde de localisation group {id}", localisationgroup?.Id);
            return new ResponseApiLocalisations { ApiStatus = EnumApiStatus.Problem, Message = "Erreur lors de la sauvegarde" };
        }
    }
}
