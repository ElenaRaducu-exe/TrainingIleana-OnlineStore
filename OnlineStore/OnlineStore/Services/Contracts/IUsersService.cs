using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Contracts
{
    public interface IUsersService
    {
        Task<List<User>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(int id); 

        Task<User> UpdateUserActiveMode(User user);

        Task<bool> CreateUserAsync(UserDTO newUser); 
    }
}
