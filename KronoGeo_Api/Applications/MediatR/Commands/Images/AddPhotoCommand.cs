using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Images
{
    public class AddPhotoCommand : IRequest<PhotoDTO> 
    {
        public required IFormFile FormFile { get; set; }
    }
}
