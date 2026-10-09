using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps
{
    public class UpdateLocalisationGroupCommand :IRequest<ResponseApiLocalisations>
    {
        public required LocalisationGroupDTO LocalisationGroup { get; set; }
    }
}
