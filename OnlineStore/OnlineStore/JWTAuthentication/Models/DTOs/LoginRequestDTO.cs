//user credential - the request data from the login page 

using System.ComponentModel.DataAnnotations;

namespace OnlineStore.JWTAuthentication.Models.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty; 
    }
}
