using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Gps
{
    public class DeleteLocalisationGroupCommand : IRequest<bool>
    {
        public required int Id { get; set; }
    }
}
