using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Gps
{
    public class DeleteLocalisationsCommand : IRequest<ResponseApiLocalisations>
    {
        public required int IdLocalisationGroup { get; set; }
        public required string IdUser { get; set; }
    }
}
