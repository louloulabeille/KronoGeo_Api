using KronoGeo_Api.Models.Infrastructure.Http;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Manager
{
    public class DeleteUserCommand : IRequest<ResponseApiManager>
    {
        public required string Id { get; set; }
    }
}
