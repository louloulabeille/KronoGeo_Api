using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models
{
    public class LocalisationPhoto : Localisation
    {
        public required string Name { get; set; }
        public string? PathPhoto { get; set; }
    }
}
