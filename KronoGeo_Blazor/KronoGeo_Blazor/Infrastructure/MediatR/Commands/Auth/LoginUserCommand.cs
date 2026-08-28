using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth
{
    public class LoginUserCommand : IRequest<ResponseApiAuthenticate> // -- RegisterDTO retour de valeur
    {
        public required RegisterDTO Register { get; set; }
    }
}
