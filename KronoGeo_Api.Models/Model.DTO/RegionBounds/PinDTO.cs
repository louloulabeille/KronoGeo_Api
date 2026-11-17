using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO.RegionBounds
{
    /// <summary>
    /// Pin DTO 
    /// </summary>
    public class PinDTO
    {
        public required CoordonneeDTO PinCoordonee { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Description { get; set; }
    }
}
