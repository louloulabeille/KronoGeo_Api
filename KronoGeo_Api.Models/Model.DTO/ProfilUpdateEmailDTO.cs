using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class ProfilUpdateEmailDTO
    {
        public string? Id { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
        [Phone(ErrorMessage ="Numéro de téléphone est invalide.")]
        public string? PhoneNumber { get; set; }

        // -- liste des roles
        public List<string>? Roles { get; set; }
    }
}
