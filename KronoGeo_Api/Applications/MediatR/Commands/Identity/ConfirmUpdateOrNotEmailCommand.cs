using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class ConfirmUpdateOrNotEmailCommand: IRequest<RegisterIdentity>
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required bool Recup { get; set; } = false;
    }
}
