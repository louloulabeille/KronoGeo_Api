using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class ProfilDTO
    {
        public string? Id { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        //[Required(ErrorMessage = "Le mot de passe est obligatoire pour se connecter.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[²&~#{[|`\\^@\]}'()-_=?;.:/!]).{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et plus de 12 caractères.")]
        public string NewPassord { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "L'adresse mail est invalide.")]
        public string Email { get; set; } = string.Empty;
        [Phone(ErrorMessage ="Numéro de téléphone est invalide.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
