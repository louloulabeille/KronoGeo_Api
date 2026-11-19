using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.RegionBounds
{
    // -- coordonnée gps
    public sealed record Coordonnee
    {
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }


        #region override method
        public override string ToString()
        {
            return $"latitude: {Latitude} & longitude: {Longitude}";
        }
        #endregion
    }
}
