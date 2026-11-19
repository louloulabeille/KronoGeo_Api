using KronoGeo_Api.Models.Model.DTO.RegionBounds;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace KronoGeo_Api.Models.RegionBounds
{
    /// <summary>
    /// class qui regroupe les pins trop proche entre elle 
    /// </summary>
    public class ClusterPin
    {
        /// <summary>
        /// pins qui se trouvent autour du points central
        /// </summary>
        public List<Pin> Pins { get; set; } = [];
        /// <summary>
        /// coordonnée du centre pour le calcul des points autour
        /// </summary>
        public Coordonnee? CentralCoordonate { get; set; }
        /// <summary>
        /// nombre de Pins
        /// </summary>
        public int Count => Pins.Count;
        /// <summary>
        /// seul ou pas pour le refaire repasser en mode pin à la place de cluster
        /// </summary>
        public bool IsSingle => Count == 1;
    }
}
