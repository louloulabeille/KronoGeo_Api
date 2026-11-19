using KronoGeo_Api.Models.RegionBounds;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO.RegionBounds
{
    /// <summary>
    /// Pin DTO 
    /// </summary>
    public record PinDTO
    {
        public required CoordonneeDTO PinCoordonnee { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Description { get; set; }

        public Pin ToPin()
        {
            return new()
            {
                PinCoordonnee = PinCoordonnee.ToCoordonnee(),
                Name = Name,
                Path = Path,
                Description = Description
            };
        }
    }
}
