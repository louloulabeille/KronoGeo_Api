using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.Maui.Devices.Sensors;
using Org.BouncyCastle.Security.Certificates;
using System.Globalization;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class AddLocalisationsHandler(ILogger<AddLocalisationsHandler> logger
        , KronoGeoDbContext context, IServiceGestionPhoto servicePhoto) : RepositoryHandler(logger, context),
        IRequestHandler<AddLocalisationsCommand, ResponseApiLocalisations>
    {
        #region private properties
        private readonly IServiceGestionPhoto _servicePhoto = servicePhoto;
        #endregion


        /// <summary>
        /// ajoute un groupe de localisation à la base de données
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiLocalisations> Handle(AddLocalisationsCommand request, CancellationToken cancellationToken)
        {

            LocalisationGroup localisationGroup = new() { 
                // -- correction pour erreur avec datetime Utc et postgreSql
                Date = DateTime.Parse(request.LocalisationGroup.Date.ToString(),null, 
                System.Globalization.DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal),
                Name = request.LocalisationGroup.Name,
                ApplicationUserId = request.LocalisationGroup.ApplicationUserId,
                RouteTelemetry = request.LocalisationGroup.RouteTelemetry is null ? null :
                    new RouteTelemetry()
                    {
                         Id = request.LocalisationGroup.RouteTelemetry.Id,
                         Distance = request.LocalisationGroup.RouteTelemetry.Distance,
                         DistanceUnit = request.LocalisationGroup.RouteTelemetry.DistanceUnit,
                         AverageSpeed = request.LocalisationGroup.RouteTelemetry.AverageSpeed,
                         PositiveElevationGain = request.LocalisationGroup.RouteTelemetry.PositiveElevationGain,
                         NegativeElevationGain = request.LocalisationGroup.RouteTelemetry.NegativeElevationGain,
                         DateTimeBegin = request.LocalisationGroup.RouteTelemetry.DateTimeBegin.ToUniversalTime(),
                         DateTimeEnd = request.LocalisationGroup.RouteTelemetry.DateTimeEnd.ToUniversalTime(),
                         TotalTime = request.LocalisationGroup.RouteTelemetry.TotalTime,
                         TotalTimePaused = request.LocalisationGroup.RouteTelemetry.TotalTimePaused,
                         TotalLocalisations = request.LocalisationGroup.RouteTelemetry.TotalLocalisations
                    }
            };

            if (request.LocalisationGroup.Localisations is not null ) 
            {
                foreach (var localisation in request.LocalisationGroup.Localisations)
                {
                    /*_logger.LogInformation("Localisation {Id} Date {Date}", 
                        localisation.Id, localisation.Timestamp);*/
                    localisationGroup.Localisations ??= [];

                    if (localisation is LocalisationPhotoDTO localisationPhoto)
                    {
                        localisationGroup.Localisations.Add(new LocalisationPhoto
                        {
                            OrderIndex = localisationPhoto.OrderIndex,
                            Latitude = localisationPhoto.Latitude,
                            Longitude = localisationPhoto.Longitude,
                            Accuracy = localisationPhoto.Accuracy,
                            Altitude = localisationPhoto.Altitude,
                            Course = localisationPhoto.Course,
                            Speed = localisationPhoto.Speed,
                            VerticalAccuracy = localisationPhoto.VerticalAccuracy,
                            Timestamp = localisationPhoto.Timestamp.ToUniversalTime(),
                            Name = localisationPhoto.Name,
                            PathPhoto = localisationPhoto.PathPhoto
                        });
                    }
                    else
                    {
                        localisationGroup.Localisations.Add(new Localisation
                        {
                            OrderIndex = localisation.OrderIndex,
                            Latitude = localisation.Latitude,
                            Longitude = localisation.Longitude,
                            Accuracy = localisation.Accuracy,
                            Altitude = localisation.Altitude,
                            Course = localisation.Course,
                            Speed = localisation.Speed,
                            VerticalAccuracy = localisation.VerticalAccuracy,
                            Timestamp = localisation.Timestamp.ToUniversalTime()
                        });
                    }
                }
            }
            _unitOfWork.Repository<LocalisationGroup>().Add(localisationGroup);

            if ( _unitOfWork.SaveChanges() > 0 )
            {
                // -- déplace les images dans le bon répertoire 
                var photos = localisationGroup.Localisations?.Where(p => p is LocalisationPhoto).ToList();
                if ( photos is not null )
                {
                    foreach (var item in photos)
                    {
                        var localisationPhoto = item as LocalisationPhoto;

                        var retour = await _servicePhoto.CutPhoto(localisationGroup.Id.ToString(),
                            new PhotoDTO()
                            {
                                Name = localisationPhoto?.Name,
                                PathPhoto = localisationPhoto?.PathPhoto
                            });

                        if (retour != null && localisationPhoto is not null)
                        {
                            localisationPhoto.PathPhoto = retour.PathPhoto;
                            _unitOfWork.Repository<LocalisationPhoto>().Update(localisationPhoto);
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                
                return 
                    new ResponseApiLocalisations()
                    {
                        ApiStatus = EnumApiStatus.Success,
                        Message = "Success Add",
                        LocalisationGroupDTO = new LocalisationGroupDTO()
                        {
                            Id = localisationGroup.Id,
                            Date = localisationGroup.Date,
                            Name = localisationGroup.Name,
                            ApplicationUserId = localisationGroup.ApplicationUserId,
                            RouteTelemetry = RouteTelemetryDTO.Parse(localisationGroup.RouteTelemetry),
                            Localisations = localisationGroup.Localisations?.Select(l =>
                            {
                                if (l is LocalisationPhoto photo)
                                {
                                    return LocalisationDTO.Parse(photo);
                                }
                                return LocalisationDTO.Parse(l);
                            }).ToList()
                        }

                    };
            }
            
            return new ResponseApiLocalisations() { 
                ApiStatus = EnumApiStatus.Problem,
                Message = "No save localisations.",
                LocalisationGroupDTO = request.LocalisationGroup
            };
        }
    }
}
