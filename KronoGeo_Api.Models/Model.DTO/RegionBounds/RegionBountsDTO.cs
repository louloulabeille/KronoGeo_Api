using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO.RegionBounds
{
    /// <summary>
    /// class de coodonnées Latitude et Longitude
    /// </summary>
    public class CoordonneeDTO
    {
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
    }

    /// <summary>
    /// class region map limite min et max - bounds
    /// </summary>
    public class RegionBountsDTO
    {
        public required CoordonneeDTO MinBounds { get; set; }
        public required CoordonneeDTO MaxBounds { get; set; }
    }
}
