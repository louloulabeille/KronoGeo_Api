using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

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
