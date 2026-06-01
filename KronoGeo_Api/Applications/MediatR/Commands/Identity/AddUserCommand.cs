using KronoGeo_Api.Applications.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace KronoGeo_Api.Applications.MediatR.Commands.Identity
{
    public class AddUserCommand : IRequest<RegisterIdentity>
    {
        public required RegisterDTO Register { get; set; }
    }
}
