using KronoGeo_Api.Models.Model.DTO.RegionBounds;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public class ResponseApiPinRegionBounds : ResponseApi
    {
        public List<PinDTO> PinRegion { get; set; } = [];
    }
}
