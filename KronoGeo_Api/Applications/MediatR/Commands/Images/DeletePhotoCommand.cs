using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Images
{
    public class DeletePhotoCommand : IRequest<bool>
    {
        public required int IdPhoto { get; set; }
    }
}
