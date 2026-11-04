using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager
{
    public class UpdateUserRoleCommand : IRequest<ResponseApiManager>
    {
        public required ProfilDTOAdmin Profil { get; set; }
    }
}
