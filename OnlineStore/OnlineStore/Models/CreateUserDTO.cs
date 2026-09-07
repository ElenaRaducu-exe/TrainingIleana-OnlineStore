using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models
{
    public class CreateUserDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public UserRoleEnum Role { get; set; }
    }
}
