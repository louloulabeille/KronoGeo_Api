using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Manager
{
    public class GetProfileCommand : IRequest<ResponseApiManager>
    {
    }
}
