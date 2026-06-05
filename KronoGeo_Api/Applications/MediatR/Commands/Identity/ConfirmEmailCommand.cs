using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class ConfirmEmailCommand : IRequest<RegisterIdentity>
    {
        public required string Id { get; set; } // - clé primaire
        public required string Token { get; set; }
    }
}
