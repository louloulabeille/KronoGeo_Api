using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models
{
    public class LocalisationPhoto : Localisation
    {
        public required string Name { get; set; }
        public string? PathPhoto { get; set; }
        public string? Description { get; set; }

        public override LocalisationPhotoDTO GetDTO()
        {
            
            return new LocalisationPhotoDTO()
            {
                Id = this.Id,
                OrderIndex = this.OrderIndex,
                Timestamp = this.Timestamp,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Altitude = this.Altitude,
                Accuracy = this.Accuracy,
                VerticalAccuracy = this.VerticalAccuracy,
                Speed = this.Speed,
                Course = this.Course,
                LocalisationGroupId = this.LocalisationGroupId,
                Name = this.Name,
                Description = this.Description,
                PathPhoto = this.PathPhoto
            };
           
        }
    }
}
