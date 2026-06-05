using KronoGeo_Api.Applications.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class UpdateUserCommand : IRequest<RegisterIdentity>
    {
        public required RegisterDTO registerDTO { get; set; }
    }
}
