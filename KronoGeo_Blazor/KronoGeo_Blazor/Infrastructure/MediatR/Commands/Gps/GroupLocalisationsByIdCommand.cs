using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps
{
    public class GroupLocalisationsByIdCommand : IRequest<ResponseApiLocalisations>
    {
        public required int Id { get; set; }
    }
}
