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
        public required DateTime Date { get; set; } = DateTime.Now;
        public List<Localisation>? Localisations { get; set; }

        public required string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
