using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images
{
    public class DeletePhotoCommand : IRequest<ResponseApiBool>
    {
        public required int Id { get; set; }
    }
}
