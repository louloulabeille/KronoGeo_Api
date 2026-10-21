using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class ProfilMpDTO
    {
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et au moins 12 caractères.")]
        public string Password { get; set; } = string.Empty;
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et au moins 12 caractères.")]
        [Compare(nameof(Password), ErrorMessage = "Les 2 mots de passe sont différents.")]
        public string PasswordRepeat { get; set; } = string.Empty;


        /// <summary>
        /// method qui vérifie si les mots de passe sont identiques
        /// </summary>
        /// <returns></returns>
        public bool IsIdentique()
        {
            return Password == PasswordRepeat;
        }
    }
}
