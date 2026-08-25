using System.ComponentModel.DataAnnotations;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class RegisterDTO
    {
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email est obligatoire pour se connecter.")]
        public required string Login { get; set; }
        [Required(ErrorMessage = "Le mot de passe est obligatoire pour se connecter."), StringLength(20, MinimumLength = 8, ErrorMessage = "Le mot de passe doit avoir 8 caractères minimun et 20 caractères maximun.")]
        public string Password { get; set; } = string.Empty;
        public string? NewPassord  {get;set; }
        //[EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Token { get; set; } = null;
    }
}
