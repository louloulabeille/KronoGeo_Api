using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class UpdateUserCommand : IRequest<ResponseApiManager>
    {
        public required string IdUser { get; set; }
        public required ProfilDTO Profil { get; set; }
    }
}
