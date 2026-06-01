namespace KronoGeo_Api.Applications.Model.DTO
{
    public class RegisterDTO
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Token { get; set; } = null;
    }
}
