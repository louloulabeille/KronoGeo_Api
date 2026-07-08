using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Models;
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
            LocalisationGroup localisationGroup = new() { 
                Date = DateTime.Now,
                Name = request.LocalisationGroup.Name,
                ApplicationUserId = request.LocalisationGroup.ApplicationUserId,
            };

            if (request.LocalisationGroup.Localisations is not null ) 
            {
                foreach (var localisation in request.LocalisationGroup.Localisations)
                {
                    localisationGroup.Localisations ??= [];

                    if (localisation is LocalisationPhotoDTO localisationPhoto)
                    {
                        localisationGroup.Localisations.Add(new LocalisationPhoto
                        {
                            Latitude = localisationPhoto.Latitude,
                            Longitude = localisationPhoto.Longitude,
                            Accuracy = localisationPhoto.Accuracy,
                            Altitude = localisationPhoto.Altitude,
                            Course = localisationPhoto.Course,
                            Speed = localisationPhoto.Speed,
                            VerticalAccuracy = localisationPhoto.VerticalAccuracy,
                            Timestamp = localisationPhoto.Timestamp,
                            Name = localisationPhoto.Name,
                            PathPhoto = localisationPhoto.PathPhoto
                        });
                    }
                    else
                    {
                        localisationGroup.Localisations.Add(new Localisation
                        {
                            Latitude = localisation.Latitude,
                            Longitude = localisation.Longitude,
                            Accuracy = localisation.Accuracy,
                            Altitude = localisation.Altitude,
                            Course = localisation.Course,
                            Speed = localisation.Speed,
                            VerticalAccuracy = localisation.VerticalAccuracy,
                            Timestamp = localisation.Timestamp
                        });
                    }
                }
            }
            _unitOfWork.Repository<LocalisationGroup>().Add(localisationGroup);

            if ( _unitOfWork.SaveChanges() > 0 )
            { 
                return Task.FromResult(new LocalisationGroupDTO
                {
                    Id = localisationGroup.Id,
                    Date = localisationGroup.Date,
                    Name = localisationGroup.Name,
                    ApplicationUserId = localisationGroup.ApplicationUserId,
                    Localisations = localisationGroup.Localisations?.Select(l => new LocalisationDTO
                    {
                        Id = l.Id,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        Accuracy = l.Accuracy,
                        Altitude = l.Altitude,
                        Course = l.Course,
                        Speed = l.Speed,
                        VerticalAccuracy = l.VerticalAccuracy,
                        Timestamp = l.Timestamp
                    }).ToList()
                });
            }
            
            return Task.FromResult<LocalisationGroupDTO>(request.LocalisationGroup);
        }
    }
}
