using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps
{
    public class GroupLocationUserCommand : IRequest<ResponseApiLocalisations>
    {
        public required string UserId { get; set; }
    }
}
