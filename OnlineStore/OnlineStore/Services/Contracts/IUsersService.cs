using OnlineStore.DBModels;

namespace OnlineStore.Services.Contracts
{
    public interface IUsersService
    {
        Task<List<User>> GetUsersAsync();
    }
}
