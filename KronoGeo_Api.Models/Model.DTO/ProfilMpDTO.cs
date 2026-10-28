using KronoGeo_Api.Models.ValidationsAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    /// <summary>
    /// class DTO pour modifier le mot de passe de l'utilisateur
    /// </summary>
    public class ProfilMpDTO
    {
        [Required(ErrorMessage ="L'ancien mot de passe est obligatoirepour modifier le mot de passe.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et au moins 12 caractères.")]
        public string PasswordOld { get; set; } = string.Empty;
        [Required(ErrorMessage ="Le nouveau mot de passe doit être saisie si vous voulez le modifier.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et au moins 12 caractères.")]
        // -- ValidationAttribute spécifique
        [NotEqualTo(nameof(PasswordOld), ErrorMessage = "L'ancien mot de passe et le nouveau sont identiques, veuillez modifier votre nouveau mot de passe.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage ="Vous devez ressaisir le mot de passe pour confirmer la modification.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et au moins 12 caractères.")]
        [Compare(nameof(Password), ErrorMessage = "Les 2 mots de passe sont différents.")]
        public string PasswordRepeat { get; set; } = string.Empty;

    }
}
