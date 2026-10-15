using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class UpdateUserPasswordCommand : IRequest<ProfilManager>
    {
        public required ProfilDTO Profil { get; set; }
    }
}
