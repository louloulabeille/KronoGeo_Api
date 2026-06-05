using System.ComponentModel.DataAnnotations;

namespace KronoGeo_Api.Applications.Model.DTO
{
    public class RegisterDTO
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        public required string Login { get; set; }
        public string Password { get; set; } = string.Empty;
        //[EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Token { get; set; } = null;
    }
}
