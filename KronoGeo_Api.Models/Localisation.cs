using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models
{
    public class Localisation
    {
        
        public int Id { get; set; }
        public required int OrderIndex { get; set; } = 0;   // -- compteur des localisations pour le tri
        public required DateTimeOffset Timestamp { get; set; }
        public required double Latitude { get; set; }
        public required double Longitude { get; set;  }
        /// <summary>
        /// en mètre sous android avec comme point de référence WGS 84 plus ou moins 45 à 50 mètres plus haut
        /// </summary>
        public double? Altitude { get; set; }   
        public double? Accuracy { get; set; }
        public double? VerticalAccuracy { get; set; }
        public double? Speed { get; set; }
        public double? Course { get; set; }

        public int LocalisationGroupId { get; set; }
        public LocalisationGroup? LocalisationGroup  { get; set; }

        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
