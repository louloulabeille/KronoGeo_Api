using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class DeleteLocalisationsHandler(ILogger<AddLocalisationsHandler> logger
        , KronoGeoDbContext context) : RepositoryHandler(logger, context),
        IRequestHandler<DeleteLocalisationsCommand, ResponseApiLocalisations>
    {
        public async Task<ResponseApiLocalisations> Handle(DeleteLocalisationsCommand request, CancellationToken cancellationToken)
        {
            var localisations = _unitOfWork.Repository<LocalisationGroup>().GetById(request.IdLocalisationGroup);
;            

            if (localisations is not null && localisations.ApplicationUserId == request.IdUser )
            {
                _unitOfWork.Repository<LocalisationGroup>().Delete( localisations );
                return new ResponseApiLocalisations()
                {
                    ApiStatus = EnumApiStatus.Success,
                    Message = "Success delete"
                };
            }
            else
            {
                return new ResponseApiLocalisations()
                {
                    ApiStatus = EnumApiStatus.NotFound,
                    Message = "Impossible to delete.",
                }; 
            }
        }
    }
}
