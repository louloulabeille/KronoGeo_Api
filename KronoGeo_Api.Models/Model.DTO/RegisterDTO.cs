using System.ComponentModel.DataAnnotations;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class RegisterDTO
    {
        public string? Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Login ou votre email est obligatoire pour se connecter.")]
        public required string Login { get; set; }
        //[RegularExpression(@"^(?=.*\[a-z\])(?=.*\[A-Z\])(?=.*\\d)(?=.*\[^\\da-zA-Z\]).+$", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial")]
        [Required(ErrorMessage = "Le mot de passe est obligatoire pour se connecter.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=\S*?[a-z])(?=\S*?[0-9])(?=\S*?[²&~#{[|`\\^@\]}'()-_=?;.:/!])\S{12,}", ErrorMessage = "Le mot de passe doit contenir au moins une lettre minuscule , une lettre majuscule, un chiffre et un caractère spécial et au moins 12 caractères.")]
        public string Password { get; set; } = string.Empty;
        public string? NewPassord  {get;set; }
        //[EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Token { get; set; } = null;
    }
}
