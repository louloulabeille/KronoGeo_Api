using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class LocalisationPhotoDTO : LocalisationDTO
    {
        public required string Name { get; set; }
        //public string? PathPhoto { get; set; }
        public string? PathPhoto { get; set; } = null;
        public string? Description { get; set; }

        /*public LocalisationPhotoDTO()
        {
            base.TypeObjet = TypeLocalisation.Photo;
        }*/
        public override LocalisationPhoto Get()
        {
            return new LocalisationPhoto()
            {
                Id = this.Id,
                OrderIndex = this.OrderIndex,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Accuracy = this.Accuracy,
                Altitude = this.Altitude,
                Course = this.Course,
                Speed = this.Speed,
                VerticalAccuracy = this.VerticalAccuracy,
                Timestamp = this.Timestamp.ToUniversalTime(),
                Name = this.Name,
                PathPhoto = this.PathPhoto,
                Description = this.Description,
                LocalisationGroupId = this.LocalisationGroupId ?? 0
            };
        }
    }
}
