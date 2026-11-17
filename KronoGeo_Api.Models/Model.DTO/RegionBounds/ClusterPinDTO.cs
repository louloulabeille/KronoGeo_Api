using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO.RegionBounds
{
    /// <summary>
    /// Cluster de Pin pour regrouper les pins trop proche
    /// </summary>
    public class ClusterPinDTO
    {
        public List<PinDTO> Cluster { get; set; } = [];
    }
}
