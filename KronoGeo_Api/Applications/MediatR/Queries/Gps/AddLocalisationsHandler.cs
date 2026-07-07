using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class AddLocalisationsHandler(ILogger<AddLocalisationsHandler> logger
        , KronoGeoDbContext context) : RepositoryHandler(logger, context),
        IRequestHandler<AddLocalisationsCommand, LocalisationGroupDTO>
    {
        /// <summary>
        /// ajoute un groupe de localisation à la base de données
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<LocalisationGroupDTO> Handle(AddLocalisationsCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
