namespace OnlineStore.JWTAuthentication.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string TokenJWT { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
