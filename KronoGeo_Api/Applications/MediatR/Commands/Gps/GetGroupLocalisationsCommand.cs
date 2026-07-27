using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Gps
{
    public class GetGroupLocalisationsCommand : IRequest<ResponseApiLocalisations>
    {
        public required string IdUser { get; set; }
    }
}
