using OnlineStore.Models.Enums;

namespace OnlineStore.Models.FrontendModels
{
    public class UserModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public UserRoleEnum? Role { get; set; }
    }
}
