using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.Maui
{
    public class LocalisationTraceMaui
    {
        [SQLite.PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int OrderIndex { get; set; } = 0;   // -- compteur des localisations pour le tri
        public DateTimeOffset Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>
        /// en mètre sous android avec comme point de référence WGS 84 plus ou moins 45 à 50 mètres plus haut
        /// </summary>
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public double? VerticalAccuracy { get; set; }
        public double? Speed { get; set; }
        public double? Course { get; set; }

/*        // -- pour les photos héritage de LocalisationMauiPhoto
        public string? Name { get; set; }
        public string? PathPhoto { get; set; }
        public string? Description { get; set; }
*/
        public int? LocalisationGroupId { get; set; }

    }
}
