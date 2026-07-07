using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class LocalisationPhotoDTO
    {
        public required string Name { get; set; }
        //public string? PathPhoto { get; set; }
        public byte[]? Photo { get; set; } = null;  // -- stockage of the photo in bytes, can be null if not provided
    }
}
