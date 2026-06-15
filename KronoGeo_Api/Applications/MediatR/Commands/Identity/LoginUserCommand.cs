using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class LoginUserCommand: IRequest<RegisterIdentity>
    {
        public required RegisterDTO Register { get; set; }
    }
}
