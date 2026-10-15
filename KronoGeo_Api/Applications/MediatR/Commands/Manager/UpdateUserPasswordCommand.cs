using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class UpdateUserPasswordCommand : IRequest<ResponseApiManager>
    {
        public required ProfilDTO Profil { get; set; }
    }
}
