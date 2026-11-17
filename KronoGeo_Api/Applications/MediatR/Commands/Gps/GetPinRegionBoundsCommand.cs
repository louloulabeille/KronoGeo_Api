using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO.RegionBounds;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Gps
{
    public class GetPinRegionBoundsCommand : IRequest<ResponseApiPinRegionBounds>
    {
        public required RegionBountsDTO Region { get; set; }
        public required string IdUser { get; set; }
    }
}
