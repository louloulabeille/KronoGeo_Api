using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class DeleteUserCommand : IRequest<ProfilManager>
    {
        public required string Id { get; set; }
    }
}
