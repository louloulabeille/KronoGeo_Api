using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps
{
    public class DeleteLocalisationGroupCommand : IRequest<ResponseApiBool>
    {
        public required int Id { get; set; }
    }
}
