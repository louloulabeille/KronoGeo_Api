using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class DeleteUserCommand : IRequest<RegisterIdentity>
    {
        public required string Id { get; set; }
    }
}
