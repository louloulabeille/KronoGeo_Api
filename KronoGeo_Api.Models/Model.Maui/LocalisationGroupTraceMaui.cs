using KronoGeo_Api.Models.Model.DTO;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.Maui
{
    public class LocalisationGroupTraceMaui
    {
        [SQLite.PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
        public string AffichageName => $"{Name} {Date.ToLocalTime().ToString("dd/MM/yyyy")}";

        #region public method conversion
        public virtual LocalisationGroupDTO ToLocalisationGroup(string applicationUserId 
            , List<LocalisationDTO> localisations, RouteTelemetryDTO? routeTelemetry)
        {
            return new LocalisationGroupDTO()
            {
                Id = this.Id,
                Name = this.Name,
                Date = this.Date,
                Localisations = localisations,
                ApplicationUserId = applicationUserId,
                RouteTelemetry = routeTelemetry
            };
        }
        #endregion
    }
}
