using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class AddPhotoHandler : IRequestHandler<AddPhotoCommand, PhotoDTO>
    {
        /// <summary>
        /// enregistre les images au niveau du site
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<PhotoDTO> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
