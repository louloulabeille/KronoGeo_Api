using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images
{
    public class DeletePhotoCommand : IRequest<bool>
    {
        public required int Id { get; set; }
    }
}
