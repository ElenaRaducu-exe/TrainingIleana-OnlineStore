using OnlineStore.DBModels;

namespace OnlineStore.Services.Contracts
{
    public interface IUsersService
    {
        Task<List<User>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(int id); 

        Task<User> UpdateUserActiveMode(User user);
    }
}
