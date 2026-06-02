using System.ComponentModel.DataAnnotations;

namespace KronoGeo_Api.Applications.Model.DTO
{
    public class RegisterDTO
    {
        [Required]
        public required string Login { get; set; }
        [Required]
        public required string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Token { get; set; } = null;
    }
}
