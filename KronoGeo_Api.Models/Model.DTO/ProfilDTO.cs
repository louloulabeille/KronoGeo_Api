using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class ProfilDTO
    {
        public string? Id { get; set; }
        public string? Login { get; set; } 
        public string? Password { get; set; }
        //[Required(ErrorMessage = "Le mot de passe est obligatoire pour se connecter.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et plus de 12 caractères.")]
        public string? NewPassword { get; set; }
        [EmailAddress(ErrorMessage = "L'adresse mail est invalide.")]
        public string? Email { get; set; }
        //[Phone(ErrorMessage ="Numéro de téléphone est invalide.")]
        public string? PhoneNumber { get; set; }

        // -- liste des roles
        public List<string>? Roles { get; set; }
    }
}
