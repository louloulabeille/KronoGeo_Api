using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class LocalisationGroupDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime Date { get; set; } = DateTime.Now;
        public List<LocalisationDTO>? Localisations { get; set; }

        public required string ApplicationUserId { get; set; }
    }
}
