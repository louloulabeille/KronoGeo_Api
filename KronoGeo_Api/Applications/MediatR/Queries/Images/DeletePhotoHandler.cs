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

                if (_unitOfWork.LocalisationSpecifiqueRepository
                    .ConvertLocalisationPhotoToLocalisation(request.IdPhoto)) // -- enregistrement en base
                {
                    // -- supression de la photo en local
                    if (!_gestionPhoto.DeletePhoto(path, localisationPhoto.PathPhoto)) return false;
                    return true;
                }
                else
                    return false;
                
            }

            return false;
        }
    }
}
