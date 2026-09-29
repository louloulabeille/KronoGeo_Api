using KronoGeo_Api.Applications.MediatR.Commands.Images;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Images
{
    public class UpdatePhotoHandler(ILogger<UpdatePhotoHandler> logger, KronoGeoDbContext context) :
        ImagesHandler(logger, context), IRequestHandler<UpdatePhotoCommand,bool>
    {
        public async Task<bool> Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
        {
            var localisation = request.Photo.Get() as LocalisationPhoto;
            localisation?.LocalisationGroup = _unitOfWork.Repository<LocalisationGroup>().GetById(localisation.LocalisationGroupId);

            if (localisation is not null && localisation.LocalisationGroup?.ApplicationUserId == request.IdUser)
            {

                _unitOfWork.Repository<LocalisationPhoto>().Update(localisation);
                int nb = _unitOfWork.SaveChanges();
                return nb == 1;
            }
            return false;
        }
    }
}
