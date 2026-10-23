using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images
{
    public class UpdatePhotoCommand : IRequest<ResponseApiBool>
    {
        public required LocalisationPhotoDTO Photo { get; set; }
    }
}
