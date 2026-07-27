using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class GetGroupLocalisationHandler(ILogger<AddLocalisationsHandler> logger
        , KronoGeoDbContext context) : RepositoryHandler(logger, context),
        IRequestHandler<GetGroupLocalisationsCommand, ResponseApiLocalisations>
    {
        /// <summary>
        /// retourne les groupes de localisations par idUser
        /// avec order by desc id 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiLocalisations> Handle(GetGroupLocalisationsCommand request, CancellationToken cancellationToken)
        {
            var result = _unitOfWork.Repository<LocalisationGroup>()
                .Where(p => p.ApplicationUserId == request.IdUser)
                .OrderByDescending(o=>o.Id).ToList().
                Select( s => new LocalisationGroupDTO()
                {
                    Id = s.Id,
                    ApplicationUserId = s.ApplicationUserId,
                    Date = s.Date,
                    Name = s.Name,
                } ).ToList();

            if ( result is not null && result?.Count > 0)
            {
                ResponseApiLocalisations retour = new()
                {
                    ApiStatus = EnumApiStatus.Success,
                    Message = "return result."
                };
                retour.GroupsDTO.AddRange(result);
                return retour;
            }
            else return new()
            {
                ApiStatus = EnumApiStatus.NotFound,
                Message = "no result found."
            };

        }
    }
}
