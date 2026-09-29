using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Images
{
    public class UpdatePhotoCommand : IRequest<bool>
    {
        public required LocalisationPhotoDTO Photo { get; set; }
        public required string IdUser { get; set; }
    }
}
