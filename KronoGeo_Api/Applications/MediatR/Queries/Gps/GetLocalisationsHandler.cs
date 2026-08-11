using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class GetLocalisationsHandler(ILogger<AddLocalisationsHandler> logger
        , KronoGeoDbContext context) : RepositoryHandler(logger, context),
        IRequestHandler<GetLocalisationsCommand, ResponseApiLocalisations>
    {

        private readonly KronoGeoDbContext _context = context;


        /// <summary>
        /// retourne la liste des localisations par id group localsiation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiLocalisations> Handle(GetLocalisationsCommand request, CancellationToken cancellationToken)
        {
            var result = _unitOfWork.Repository<LocalisationGroup>().GetById(request.Id);
            _unitOfWork.Repository<Localisation>()
                .Where(w => w.LocalisationGroupId == request.Id)
                .ToList();
            //result?.Localisations?.AddRange(localisations);
            if (result is not null)
            {
                return new ResponseApiLocalisations()
                {
                    ApiStatus = EnumApiStatus.Success,
                    Message = "return result;",
                    LocalisationGroupDTO = new () 
                    { 
                        Id = result.Id,
                        Name = result.Name,
                        ApplicationUserId = result.ApplicationUserId,
                        Date = result.Date,
                        RouteTelemetry = RouteTelemetryDTO.Parse(result.RouteTelemetry),
                        Localisations = result.Localisations?.OrderBy(o => o.OrderIndex)
                        .Select(s =>
                        {
                            if (s is LocalisationPhoto photo)
                            {
                                return LocalisationDTO.Parse(photo);
                            }
                            else
                            {
                                return LocalisationDTO.Parse(s);
                            }
                        }).ToList()
                    }
                };
            }

            return new() { ApiStatus = EnumApiStatus.NotFound, Message = "Not found" };
        }
    }
}
