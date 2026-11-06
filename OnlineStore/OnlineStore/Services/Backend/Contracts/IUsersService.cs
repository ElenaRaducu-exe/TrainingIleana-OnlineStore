using OnlineStore.DBModels;
using OnlineStore.Models.DTOs;

namespace OnlineStore.Services.Backend.Contracts
{
    public interface IUsersService
    {
        Task<List<User>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(int id); 

        Task<User?> UpdateUserActiveMode(int userId);

        Task<bool> CreateUserAsync(UserDTO newUser); 
    }
}
