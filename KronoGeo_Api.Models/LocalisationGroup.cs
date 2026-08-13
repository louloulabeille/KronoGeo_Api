using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models
{
    public class LocalisationGroup
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
        public List<Localisation>? Localisations { get; set; }

        public required string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        //public int? RouteTelemetryId { get; set; } 
        public RouteTelemetry? RouteTelemetry { get; set; }
    }
}
