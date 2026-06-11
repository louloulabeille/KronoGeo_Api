using KronoGeo_Api.Applications.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class UpdateUserPasswordCommand : IRequest<RegisterIdentity>
    {
        public required RegisterDTO Register { get; set; }
    }
}
