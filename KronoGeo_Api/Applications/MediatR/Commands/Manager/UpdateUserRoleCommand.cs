using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class UpdateUserRoleCommand : IRequest<ResponseApiManager>
    {
        public bool isManager { get; set; } = false;
        public required ProfilDTO Profil { get; set; }
    }
}
