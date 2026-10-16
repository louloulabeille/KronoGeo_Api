using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class ProfilAllUsersCommand : IRequest<ResponseApiManager>
    {
        public required string IdUser { get; set; }
    }
}
