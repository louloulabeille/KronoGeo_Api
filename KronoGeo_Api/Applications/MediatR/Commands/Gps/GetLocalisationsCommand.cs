using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Gps
{
    public class GetLocalisationsCommand : IRequest<ResponseApiLocalisations>
    {
        public required int Id { get; set; }
    }
}
