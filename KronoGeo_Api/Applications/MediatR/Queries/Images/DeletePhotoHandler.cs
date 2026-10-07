using KronoGeo_Api.Applications.MediatR.Commands.Images;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface;
using KronoGeo_Api.Models;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Images
{
    public class DeletePhotoHandler(ILogger<DeletePhotoHandler> logger
        , KronoGeoDbContext dbContext, IServiceGestionPhoto gestionPhoto) 
        : ImagesHandler(logger, dbContext)
        , IRequestHandler<DeletePhotoCommand, bool>
    {
        #region private readonly propertues
        private readonly IServiceGestionPhoto _gestionPhoto = gestionPhoto;
        #endregion


        public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<LocalisationPhoto>();
            // -- recherche de la localisationPhoto
            var localisationPhoto = repository.GetById(request.IdPhoto);

            if ( localisationPhoto is not null && localisationPhoto.PathPhoto is not null )
            {
                // -- chemin physique de la photo
                string path = Path.Combine(localisationPhoto.PathPhoto, localisationPhoto.Name);
                if (_gestionPhoto.DeletePhoto(path, localisationPhoto.PathPhoto)) // -- supression de la photo sur le disque
                {
                    // -- création de la localisation correspondant 
                    var localisation = new Localisation()
                    {
                        Id = localisationPhoto.Id,
                        Accuracy = localisationPhoto.Accuracy,
                        Altitude = localisationPhoto.Altitude,
                        Course = localisationPhoto.Course,
                        Latitude = localisationPhoto.Latitude,
                        LocalisationGroupId = localisationPhoto.LocalisationGroupId,
                        Longitude = localisationPhoto.Longitude,
                        OrderIndex = localisationPhoto.OrderIndex,
                        Speed = localisationPhoto.Speed??0,
                        Timestamp = localisationPhoto.Timestamp,
                        VerticalAccuracy = localisationPhoto.VerticalAccuracy,
                    };

                    //repository.Delete(localisationPhoto); // -- supression de la localisation photo
                    // -- comme le SaveChanges qui  ne se fait pas au niveau de la base il y a juste un changement
                    // -- de statut de localisationPhoto en localisation
                    //_unitOfWork.SaveChanges();
                    _unitOfWork.Repository<Localisation>().Update(localisation); // -- modification de la localisation

                    if (_unitOfWork.SaveChanges() > 0) // -- enregistrement en base
                        return true;
                    else
                        return false;
                }

                return false;
            }

            return false;
        }
    }
}
