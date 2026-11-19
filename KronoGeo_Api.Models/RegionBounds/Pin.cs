using KronoGeo_Api.Models.Model.DTO.RegionBounds;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.RegionBounds
{
    /// <summary>
    /// Pin qui sert pour affichage des images ou photos sur une map
    /// </summary>
    public class Pin
    {
        public required Coordonnee PinCoordonnee { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? Description { get; set; }
    }
}
