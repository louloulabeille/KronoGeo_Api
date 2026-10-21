using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class ProfilMpDTO
    {
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[²&~#{[|`\\^@\]}'()-_=?;.:/!]).{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et plus de 12 caractères.")]
        public string Password { get; set; } = string.Empty;
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[²&~#{[|`\\^@\]}'()-_=?;.:/!]).{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et plus de 12 caractères.")]
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
