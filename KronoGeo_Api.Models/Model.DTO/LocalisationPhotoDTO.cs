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

        /*public LocalisationPhotoDTO()
        {
            base.TypeObjet = TypeLocalisation.Photo;
        }*/

    }
}
