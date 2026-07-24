using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class DeleteLocalisationsHandler(ILogger<AddLocalisationsHandler> logger
        , KronoGeoDbContext context, IServiceGestionPhoto servicePhoto) : RepositoryHandler(logger, context),
        IRequestHandler<DeleteLocalisationsCommand, ResponseApiLocalisations>
    {

        #region private properties
        private readonly IServiceGestionPhoto _servicePhoto = servicePhoto;
        #endregion

        public async Task<ResponseApiLocalisations> Handle(DeleteLocalisationsCommand request, CancellationToken cancellationToken)
        {
            var localisations = _unitOfWork.Repository<LocalisationGroup>().GetById(request.IdLocalisationGroup);
            try
            {
                if (localisations is not null && localisations.ApplicationUserId == request.IdUser)
                {
                    var photo = _unitOfWork.Repository<LocalisationPhoto>().Where(p => p.LocalisationGroupId == request.IdLocalisationGroup).FirstOrDefault();
                    if (photo is not null) _servicePhoto.DeletePhotos(photo.PathPhoto ?? string.Empty);

                    _unitOfWork.Repository<LocalisationGroup>().Delete(localisations);
                    _unitOfWork.SaveChanges();
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la supression des localisations");
                return new ResponseApiLocalisations()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Internal error."
                };
            }
            
        }
    }
}
